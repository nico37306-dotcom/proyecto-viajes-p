using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public record Operacion(Salida Salida, List<Reserva> Reservas, List<AsignacionOperativa> Asignaciones, List<Guia> Guias, List<Vehiculo> Vehiculos, Actor Actor);

public sealed class OperationsService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current) : IOperationsService
{
    public async Task<List<Salida>> Listar()
    {
        await current.Exigir(Acceso.Operaciones, Acceso.Guias, Acceso.Transporte);
        await using var db = await factory.CreateDbContextAsync();
        var hoy = Reglas.Hoy;
        return await db.Salidas.AsNoTracking().Include(x => x.Tour).OrderByDescending(x => x.Inicio >= hoy).ThenBy(x => x.Inicio).ToListAsync();
    }

    public async Task<Operacion?> Obtener(int id)
    {
        var actor = await current.Exigir(Acceso.Operaciones, Acceso.Guias, Acceso.Transporte);
        await using var db = await factory.CreateDbContextAsync();
        var salida = await db.Salidas.AsNoTracking().Include(x => x.Tour).Include(x => x.Guia).Include(x => x.Vehiculo).SingleOrDefaultAsync(x => x.Id == id);
        if (salida is null) return null;
        var reservas = await db.Reservas.AsNoTracking().Where(x => x.SalidaId == id && x.Estado == EstadoReserva.Confirmada)
            .Include(x => x.Pasajeros).ToListAsync();
        return new(salida, reservas, await db.Set<AsignacionOperativa>().AsNoTracking().Where(x => x.SalidaId == id).ToListAsync(),
            await db.Set<Guia>().AsNoTracking().Where(x => x.Disponible).OrderBy(x => x.Nombre).ToListAsync(),
            await db.Set<Vehiculo>().AsNoTracking().Where(x => x.Disponible).OrderBy(x => x.Placa).ToListAsync(), actor);
    }

    public async Task Guardar(int id, byte[] version, int? guia, int? vehiculo, string checklist)
    {
        var actor = await current.Exigir(Acceso.Operaciones, Acceso.Guias, Acceso.Transporte);
        await using var db = await factory.CreateDbContextAsync();
        var salida = await Cargar(db, id, version);
        if (salida.Estado is EstadoSalida.Finalizada or EstadoSalida.Cancelada or EstadoSalida.FuerzaMayor)
            throw new ReglaException("La salida está cerrada.");
        if (actor.Tiene(Acceso.Guias) || actor.Tiene(Acceso.Operaciones))
        {
            if (guia != salida.GuiaId && guia.HasValue && !await db.Set<Guia>().AnyAsync(x => x.Id == guia && x.Disponible))
                throw new ReglaException("El guía no está disponible.");
            salida.GuiaId = guia;
        }
        if (actor.Tiene(Acceso.Transporte) || actor.Tiene(Acceso.Operaciones))
        {
            if (vehiculo != salida.VehiculoId && vehiculo.HasValue &&
                !await db.Set<Vehiculo>().AnyAsync(x => x.Id == vehiculo && x.Disponible && x.Capacidad >= salida.Cupos))
                throw new ReglaException("El vehículo no está disponible o no tiene capacidad suficiente.");
            salida.VehiculoId = vehiculo;
        }
        if (actor.Tiene(Acceso.Operaciones)) salida.Checklist = checklist.Trim();
        Reglas.Validar(salida);
        await Persistir(db);
    }

    public async Task Cambiar(int id, byte[] version, EstadoSalida estado, DateOnly inicio, DateOnly fin)
    {
        await current.Exigir(Acceso.Operaciones);
        await using var db = await factory.CreateDbContextAsync();
        var salida = await Cargar(db, id, version);
        if (salida.Estado is EstadoSalida.Finalizada or EstadoSalida.Cancelada or EstadoSalida.FuerzaMayor)
            throw new ReglaException("La salida ya está cerrada.");
        switch (estado)
        {
            case EstadoSalida.Cancelada or EstadoSalida.FuerzaMayor:
                var reservas = await db.Reservas.Where(x => x.SalidaId == id && x.Estado != EstadoReserva.Cancelada && x.Estado != EstadoReserva.Finalizada).ToListAsync();
                foreach (var r in reservas) r.Estado = EstadoReserva.Cancelada;
                salida.Ocupados = salida.Privada ? 0 : salida.Ocupados - reservas.Sum(x => x.Cantidad);
                break;
            case EstadoSalida.Finalizada:
                if (salida.Fin > Reglas.Hoy || await db.Reservas.AnyAsync(x => x.SalidaId == id && x.Estado != EstadoReserva.Cancelada && x.Estado != EstadoReserva.Confirmada))
                    throw new ReglaException("La fecha de fin debe haberse alcanzado y las solicitudes deben estar resueltas.");
                break;
            case EstadoSalida.Reprogramada:
                if (inicio < Reglas.Hoy || fin < inicio) throw new ReglaException("Revisa las fechas de reprogramación.");
                salida.Inicio = inicio;
                salida.Fin = fin;
                break;
            case EstadoSalida.Confirmada:
                if (salida.Inicio < Reglas.Hoy || salida.Ocupados < (await db.Tours.FindAsync(salida.TourId))!.Minimo)
                    throw new ReglaException("Verifica la fecha y el mínimo de pasajeros.");
                break;
            case EstadoSalida.Completa:
                if (salida.Disponibles != 0) throw new ReglaException("Todavía hay cupos disponibles.");
                break;
            default:
                throw new ReglaException("Estado operativo inválido.");
        }
        salida.Estado = estado;
        await Persistir(db);
    }

    private static async Task<Salida> Cargar(ApplicationDbContext db, int id, byte[] version)
    {
        var salida = await db.Salidas.FindAsync(id) ?? throw new ReglaException("Salida inexistente.");
        db.Entry(salida).Property(x => x.Version).OriginalValue = version;
        return salida;
    }

    private static async Task Persistir(ApplicationDbContext db)
    {
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { throw new ReglaException("La salida cambió. Recarga y vuelve a intentarlo."); }
    }
}
