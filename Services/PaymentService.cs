using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public interface IPaymentGateway
{
    Task<bool> Verificar(string referencia, decimal monto, Moneda moneda);
}

public sealed class ManualPaymentGateway : IPaymentGateway
{
    public Task<bool> Verificar(string referencia, decimal monto, Moneda moneda) => Task.FromResult(false);
}

public interface IPaymentService
{
    Task Registrar(int reserva, decimal monto, MetodoPago metodo, string referencia, string comprobante, ReceiptUpload? archivo = null);
    Task CambiarEstado(int reserva, int pago, EstadoPago estado);
}

public sealed class PaymentService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current,
    IPaymentGateway gateway) : IPaymentService
{
    public async Task Registrar(int id, decimal monto, MetodoPago metodo, string referencia, string comprobante, ReceiptUpload? archivo = null)
    {
        await current.Exigir(Acceso.Contabilidad);
        if (monto != Reglas.Redondear(monto) || monto <= 0 || !Enum.IsDefined(metodo)) throw new ReglaException("Monto o método inválido.");
        var adjunto = archivo?.Validar();
        await Mutar(id, (db, reserva) =>
        {
            if (reserva.Estado is not (EstadoReserva.Aprobada or EstadoReserva.PendienteDePago or EstadoReserva.Confirmada))
                throw new ReglaException("La reserva aún no admite nuevos pagos.");
            if (reserva.Pagos.Any(x => x.Referencia.Equals(referencia.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new ReglaException("La referencia ya existe en la reserva.");
            if (reserva.Pagos.Where(x => x.Estado is EstadoPago.Confirmado or EstadoPago.Pendiente).Sum(x => x.Monto) + monto > reserva.Total)
                throw new ReglaException("El monto supera el saldo disponible.");
            var pago = new Pago
            {
                ReservaId = id, Monto = monto, Metodo = metodo, Moneda = reserva.Moneda,
                Referencia = referencia.Trim(), Comprobante = comprobante.Trim(), Archivo = adjunto
            };
            Reglas.Validar(pago);
            pago.Transacciones.Add(new() { Evento = "Registro manual" });
            db.Add(pago);
            return Task.CompletedTask;
        });
    }

    public async Task CambiarEstado(int id, int pagoId, EstadoPago estado)
    {
        await current.Exigir(Acceso.Contabilidad);
        await Mutar(id, async (db, reserva) =>
        {
            var pago = reserva.Pagos.SingleOrDefault(x => x.Id == pagoId) ?? throw new ReglaException("Pago inexistente.");
            if (pago.Estado == estado) return;
            var permitido = (pago.Estado, estado) switch
            {
                (EstadoPago.Pendiente, EstadoPago.Fallido) => true,
                (EstadoPago.Pendiente, EstadoPago.Confirmado) => reserva.Estado is EstadoReserva.Aprobada or EstadoReserva.PendienteDePago or EstadoReserva.Confirmada,
                (EstadoPago.Confirmado, EstadoPago.Reembolsado) => reserva.Estado == EstadoReserva.Cancelada,
                _ => false
            };
            if (!permitido) throw new ReglaException("La transición del pago no está permitida.");
            if (estado == EstadoPago.Confirmado && pago.Metodo == MetodoPago.Tarjeta &&
                !await gateway.Verificar(pago.Referencia, pago.Monto, pago.Moneda))
                throw new ReglaException("La pasarela todavía no ha verificado el pago con tarjeta.");
            pago.Estado = estado;
            pago.Transacciones.Add(new() { Evento = $"Estado: {estado}" });
        });
    }

    private async Task Mutar(int id, Func<ApplicationDbContext, Reserva, Task> accion)
    {
        await using var db = await factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();
        // The update obtains an exclusive lock and increments rowversion, synchronizing with booking transitions.
        if (await db.Reservas.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.Total, x => x.Total)) == 0)
            throw new ReglaException("Reserva inexistente.");
        var reserva = await db.Reservas.Include(x => x.Pagos).SingleAsync(x => x.Id == id);
        await accion(db, reserva);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
