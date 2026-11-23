using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public sealed record NuevaReserva(Guid SolicitudId, int SalidaId, Moneda Moneda, List<Pasajero> Pasajeros,
    IReadOnlyCollection<int> Extras, string? Codigo, string Notas);

public interface IBookingService
{
    Task<int> Crear(NuevaReserva input);
    Task<TotalReserva> Estimar(NuevaReserva input);
    Task<List<Reserva>> Listar(bool staff = false);
    Task<Reserva?> Obtener(int id);
    Task CambiarEstado(int id, EstadoReserva estado);
    Task<List<ServicioAdicional>> Extras();
}

public sealed class BookingService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current,
    IPricingService precios) : IBookingService
{
    public async Task<int> Crear(NuevaReserva input)
    {
        var actor = await current.Exigir(Acceso.Viajero);
        Validar(input);
        for (var intento = 0; ; intento++)
        {
            await using var db = await factory.CreateDbContextAsync();
            var existe = await Propias(db.Reservas, actor).Where(x => x.SolicitudId == input.SolicitudId)
                .Select(x => (int?)x.Id).SingleOrDefaultAsync();
            if (existe.HasValue) return existe.Value;
            var salida = await BuscarSalida(db, input);
            var total = await precios.Calcular(db, salida, actor, input.Pasajeros.Count, input.Moneda, input.Extras, input.Codigo);
            salida.Ocupados += input.Pasajeros.Count;
            var porOrganizacion = actor.Tiene(Acceso.Representante);
            var reserva = new Reserva
            {
                SolicitudId = input.SolicitudId, SalidaId = salida.Id,
                ClienteId = porOrganizacion ? null : actor.ClienteId,
                OrganizacionId = porOrganizacion ? actor.OrganizacionId : null,
                Cantidad = input.Pasajeros.Count, Moneda = input.Moneda, Total = total.Total, Descuento = total.Descuento,
                Notas = input.Notas.Trim(), Extras = total.Extras,
                Pasajeros = input.Pasajeros.Select(p => new Pasajero
                {
                    Nombre = p.Nombre.Trim(), Documento = p.Documento.Trim(), Nacionalidad = p.Nacionalidad.Trim(),
                    Nacimiento = p.Nacimiento, Requerimientos = p.Requerimientos.Trim()
                }).ToList()
            };
            db.Add(reserva);
            try
            {
                // EF rolls back the insert and seat update together if the departure version is stale.
                await db.SaveChangesAsync();
                return reserva.Id;
            }
            catch (DbUpdateConcurrencyException) when (intento < 3) { }
            catch (DbUpdateConcurrencyException)
            {
                throw new ReglaException(Reglas.T("La disponibilidad cambió. Consulta los cupos e inténtalo nuevamente.", "Availability changed. Check seats and try again."));
            }
            catch (DbUpdateException)
            {
                await using var confirm = await factory.CreateDbContextAsync();
                var id = await Propias(confirm.Reservas, actor).Where(x => x.SolicitudId == input.SolicitudId)
                    .Select(x => (int?)x.Id).SingleOrDefaultAsync();
                if (id.HasValue) return id.Value;
                throw;
            }
        }
    }

    public async Task<TotalReserva> Estimar(NuevaReserva input)
    {
        var actor = await current.Exigir(Acceso.Viajero);
        await using var db = await factory.CreateDbContextAsync();
        ValidarCantidades(input);
        var salida = await BuscarSalida(db, input);
        return await precios.Calcular(db, salida, actor, input.Pasajeros.Count, input.Moneda, input.Extras, input.Codigo);
    }

    public async Task<List<Reserva>> Listar(bool staff = false)
    {
        var actor = staff ? await current.Exigir(Acceso.Ventas, Acceso.Contabilidad) : await current.Exigir(Acceso.Viajero);
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Reservas.AsNoTracking();
        if (!staff) query = Propias(query, actor);
        return await query.Include(x => x.Salida).ThenInclude(x => x.Tour)
            .Include(x => x.Cliente).Include(x => x.Organizacion).Include(x => x.Pagos)
            .OrderByDescending(x => x.Solicitada).ToListAsync();
    }

    public async Task<Reserva?> Obtener(int id)
    {
        var actor = await current.Exigir(Acceso.Viajero, Acceso.Ventas, Acceso.Contabilidad);
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Reservas.AsNoTracking();
        if (!actor.Tiene(Acceso.Ventas) && !actor.Tiene(Acceso.Contabilidad)) query = Propias(query, actor);
        var reserva = await query.AsSplitQuery().Include(x => x.Cliente).Include(x => x.Organizacion)
            .Include(x => x.Salida).ThenInclude(x => x.Tour).Include(x => x.Pasajeros).Include(x => x.Extras)
            .Include(x => x.Pagos.OrderByDescending(p => p.Fecha)).SingleOrDefaultAsync(x => x.Id == id);
        if (reserva is not null)
        {
            var pagos = reserva.Pagos.Select(x => x.Id).ToArray();
            var adjuntos = await db.Set<ComprobantePago>().Where(x => pagos.Contains(x.PagoId)).Select(x => x.PagoId).ToListAsync();
            foreach (var pago in reserva.Pagos) pago.TieneArchivo = adjuntos.Contains(pago.Id);
        }
        return reserva;
    }

    public static EstadoReserva[] Siguientes(EstadoReserva actual) => actual switch
    {
        EstadoReserva.Pendiente => [EstadoReserva.EnRevision, EstadoReserva.Cancelada],
        EstadoReserva.EnRevision => [EstadoReserva.Aprobada, EstadoReserva.Cancelada],
        EstadoReserva.Aprobada => [EstadoReserva.PendienteDePago, EstadoReserva.Cancelada],
        EstadoReserva.PendienteDePago => [EstadoReserva.Confirmada, EstadoReserva.Cancelada],
        EstadoReserva.Confirmada => [EstadoReserva.Finalizada, EstadoReserva.Cancelada],
        _ => []
    };

    public async Task CambiarEstado(int id, EstadoReserva estado)
    {
        var actor = await current.Exigir(Acceso.Ventas, Acceso.Viajero);
        await using var db = await factory.CreateDbContextAsync();
        var query = actor.Tiene(Acceso.Ventas) ? db.Reservas : Propias(db.Reservas, actor);
        var reserva = await query.Include(x => x.Salida).Include(x => x.Pagos).SingleOrDefaultAsync(x => x.Id == id)
            ?? throw new ReglaException("Reserva inexistente.");
        if (reserva.Estado == estado) return;
        if (!actor.Tiene(Acceso.Ventas) && (estado != EstadoReserva.Cancelada || reserva.Estado is not (EstadoReserva.Pendiente or EstadoReserva.EnRevision)))
            throw new UnauthorizedAccessException();
        if (!Siguientes(reserva.Estado).Contains(estado)) throw new ReglaException("La transición de estado no está permitida.");
        if (estado == EstadoReserva.Confirmada && (reserva.Pagado < reserva.Total ||
            reserva.Salida.Estado is not (EstadoSalida.Programada or EstadoSalida.Confirmada or EstadoSalida.Completa)))
            throw new ReglaException("La salida debe estar vigente y el saldo cubierto.");
        if (estado == EstadoReserva.Finalizada && reserva.Salida.Estado != EstadoSalida.Finalizada)
            throw new ReglaException("Operaciones debe cerrar la salida antes de finalizar la reserva.");
        if (estado == EstadoReserva.Cancelada)
        {
            reserva.Salida.Ocupados -= reserva.Cantidad;
            if (reserva.Salida.Estado == EstadoSalida.Completa) reserva.Salida.Estado = EstadoSalida.Confirmada;
        }
        reserva.Estado = estado;
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { throw new ReglaException("La reserva o la salida cambió. Actualiza y vuelve a intentarlo."); }
    }

    public async Task<List<ServicioAdicional>> Extras()
    {
        await current.Exigir(Acceso.Viajero);
        await using var db = await factory.CreateDbContextAsync();
        return await db.Extras.AsNoTracking().Where(x => x.Activo).OrderBy(x => x.Nombre).ToListAsync();
    }

    internal static IQueryable<Reserva> Propias(IQueryable<Reserva> query, Actor user) =>
        user.Tiene(Acceso.Representante) ? query.Where(x => x.OrganizacionId == user.OrganizacionId)
            : query.Where(x => x.ClienteId != null && x.ClienteId == user.ClienteId);

    private static async Task<Salida> BuscarSalida(ApplicationDbContext db, NuevaReserva input)
    {
        var salida = await db.Salidas.Include(x => x.Tour).SingleOrDefaultAsync(x => x.Id == input.SalidaId);
        if (salida is null || salida.Privada || !salida.Tour.Publicado || salida.Inicio < Reglas.Hoy ||
            salida.Estado is not (EstadoSalida.Programada or EstadoSalida.Confirmada))
            throw new ReglaException(Reglas.T("Esta salida no está disponible.", "This departure is unavailable."));
        if (input.Pasajeros.Count > Math.Min(salida.Disponibles, salida.Tour.Maximo))
            throw new ReglaException(Reglas.T("No hay cupos suficientes.", "There are not enough seats."));
        return salida;
    }

    private static void ValidarCantidades(NuevaReserva input)
    {
        if (!Enum.IsDefined(input.Moneda) || input.Pasajeros.Count is < 1 or > 500 ||
            input.Extras.Count > 25 || input.Extras.Distinct().Count() != input.Extras.Count)
            throw new ReglaException(Reglas.T("Revisa la moneda, pasajeros y servicios adicionales.", "Check currency, guests and add-ons."));
    }

    private static void Validar(NuevaReserva input)
    {
        ValidarCantidades(input);
        if (input.SolicitudId == Guid.Empty || input.Notas.Length > 1500 || input.Codigo?.Length > 40)
            throw new ReglaException(Reglas.T("Revisa los datos de la solicitud.", "Check the booking details."));
        foreach (var pasajero in input.Pasajeros)
        {
            Reglas.Validar(pasajero);
            if (pasajero.Nacimiento > Reglas.Hoy) throw new ReglaException("La fecha de nacimiento no puede ser futura.");
        }
    }
}
