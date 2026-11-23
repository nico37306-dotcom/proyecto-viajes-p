using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public record PortalOrganizacion(Organizacion Organizacion, List<Convenio> Convenios, List<SolicitudCotizacion> Solicitudes);
public record TarifaVisible(string Nombre, decimal Descuento, string Condiciones);

public sealed class OrganizationService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current) : IOrganizationService, IConventionService, IQuotationService, IGroupTripService
{
    public async Task Solicitar(SolicitudCotizacion input, bool portal)
    {
        var actor = portal ? await current.Exigir(Acceso.Representante) : null;
        await using var db = await factory.CreateDbContextAsync();
        var org = actor is null ? null : await db.Organizaciones.FindAsync(actor.OrganizacionId);
        var solicitud = new SolicitudCotizacion
        {
            OrganizacionId = org?.Id, NombreOrganizacion = org?.Nombre ?? input.NombreOrganizacion.Trim(),
            Correo = org?.Correo ?? input.Correo.Trim(), FechaTentativa = input.FechaTentativa,
            Personas = input.Personas, Destinos = input.Destinos.Trim(), Presupuesto = input.Presupuesto, Moneda = input.Moneda
        };
        Reglas.Validar(solicitud);
        if (solicitud.FechaTentativa < Reglas.Hoy) throw new ReglaException("La fecha tentativa no puede estar en el pasado.");
        db.Add(solicitud);
        await db.SaveChangesAsync();
    }

    public async Task<PortalOrganizacion> Portal()
    {
        var actor = await current.Exigir(Acceso.Representante);
        await using var db = await factory.CreateDbContextAsync();
        var hoy = Reglas.Hoy;
        var org = (await db.Organizaciones.FindAsync(actor.OrganizacionId))!;
        var convenios = await db.Convenios.AsNoTracking().Include(x => x.Tarifas).Where(x => x.OrganizacionId == org.Id &&
            x.Activo && x.Desde <= hoy && x.Hasta >= hoy).OrderBy(x => x.Hasta).ToListAsync();
        return new(org, convenios, await Consulta(db).Where(x => x.OrganizacionId == org.Id).ToListAsync());
    }

    public async Task<List<TarifaVisible>> Tarifas(int convenioId)
    {
        var actor = await current.Exigir(Acceso.Representante);
        await using var db = await factory.CreateDbContextAsync();
        var hoy = Reglas.Hoy;
        return await (from c in db.Convenios
            from t in c.Tarifas
            where c.Id == convenioId && c.OrganizacionId == actor.OrganizacionId && c.Activo && c.Desde <= hoy && c.Hasta >= hoy
            let tour = db.Tours.Where(x => x.Id == t.TourId).Select(x => x.Nombre).FirstOrDefault()
            let paquete = db.Paquetes.Where(x => x.Id == t.PaqueteId).Select(x => x.Nombre).FirstOrDefault()
            select new TarifaVisible(tour ?? paquete ?? "", t.DescuentoPorcentaje, t.Condiciones)).ToListAsync();
    }

    public async Task<List<SolicitudCotizacion>> Bandeja()
    {
        await current.Exigir(Acceso.Alianzas);
        await using var db = await factory.CreateDbContextAsync();
        return await Consulta(db).ToListAsync();
    }

    public async Task<SolicitudCotizacion?> Obtener(int id, bool staff)
    {
        var actor = await current.Exigir(staff ? Acceso.Alianzas : Acceso.Representante);
        await using var db = await factory.CreateDbContextAsync();
        var solicitud = await Consulta(db).SingleOrDefaultAsync(x => x.Id == id && (staff || x.OrganizacionId == actor.OrganizacionId));
        if (!staff && solicitud is { Estado: EstadoCotizacion.Recibida or EstadoCotizacion.EnElaboracion }) solicitud.Oferta = null;
        return solicitud;
    }

    public async Task Preparar(int id, byte[] version, int? organizacion, EstadoCotizacion estado, Cotizacion input)
    {
        await current.Exigir(Acceso.Alianzas);
        await using var db = await factory.CreateDbContextAsync();
        var solicitud = await Cargar(db, id, version);
        if (solicitud.Estado is not (EstadoCotizacion.Recibida or EstadoCotizacion.EnElaboracion or EstadoCotizacion.Enviada))
            throw new ReglaException("La cotización está cerrada.");
        if (solicitud.OrganizacionId is null && organizacion.HasValue)
            solicitud.OrganizacionId = (await db.Organizaciones.FindAsync(organizacion))?.Id ?? throw new ReglaException("Organización inexistente.");
        if (estado == EstadoCotizacion.Enviada)
        {
            if (!solicitud.OrganizacionId.HasValue) throw new ReglaException("Vincula una organización antes de enviar al portal.");
            Reglas.Validar(input);
            if (input.ValidaHasta < Reglas.Hoy) throw new ReglaException("La oferta debe estar vigente.");
            solicitud.Oferta ??= new Cotizacion();
            solicitud.Oferta.Detalle = input.Detalle.Trim();
            solicitud.Oferta.Precio = input.Precio;
            solicitud.Oferta.Moneda = input.Moneda;
            solicitud.Oferta.ValidaHasta = input.ValidaHasta;
        }
        else if (estado == EstadoCotizacion.Vencida)
        {
            if (solicitud.Estado != EstadoCotizacion.Enviada || solicitud.Oferta?.ValidaHasta >= Reglas.Hoy)
                throw new ReglaException("La oferta aún no ha vencido.");
        }
        else if (estado is not (EstadoCotizacion.EnElaboracion or EstadoCotizacion.Rechazada))
            throw new ReglaException("Acción no permitida.");
        solicitud.Estado = estado;
        db.Entry(solicitud).Property(x => x.Estado).IsModified = true;
        await Persistir(db);
    }

    public async Task Responder(int id, byte[] version, bool aceptar)
    {
        var actor = await current.Exigir(Acceso.Representante);
        await using var db = await factory.CreateDbContextAsync();
        var solicitud = await Cargar(db, id, version, actor.OrganizacionId);
        if (solicitud.Estado != EstadoCotizacion.Enviada || solicitud.Oferta is null || solicitud.Oferta.ValidaHasta < Reglas.Hoy)
            throw new ReglaException("La cotización ya no está disponible para responder.");
        solicitud.Estado = aceptar ? EstadoCotizacion.Aceptada : EstadoCotizacion.Rechazada;
        await Persistir(db);
    }

    public async Task Convertir(int id, byte[] version, int tourId, DateOnly inicio)
    {
        await current.Exigir(Acceso.Alianzas);
        await using var db = await factory.CreateDbContextAsync();
        var solicitud = await Cargar(db, id, version);
        if (solicitud.Viaje is not null) return;
        var tour = await db.Tours.FindAsync(tourId) ?? throw new ReglaException("Tour inexistente.");
        if (solicitud.Estado != EstadoCotizacion.Aceptada || inicio < Reglas.Hoy || inicio > Reglas.Hoy.AddYears(5))
            throw new ReglaException("Se requiere una oferta aceptada y una fecha válida.");
        if (solicitud.Personas < tour.Minimo || solicitud.Personas > tour.Maximo)
            throw new ReglaException("El grupo no coincide con la capacidad del tour.");
        solicitud.Viaje = new ViajeGrupal
        {
            Salida = new Salida { TourId = tourId, Inicio = inicio, Fin = inicio.AddDays(tour.Dias - 1),
                Cupos = solicitud.Personas, Ocupados = solicitud.Personas, Estado = EstadoSalida.Confirmada, Privada = true }
        };
        db.Entry(solicitud).Property(x => x.Estado).IsModified = true;
        await Persistir(db);
    }

    public async Task<List<Opcion>> Organizaciones()
    {
        await current.Exigir(Acceso.Alianzas);
        await using var db = await factory.CreateDbContextAsync();
        return await db.Organizaciones.AsNoTracking().OrderBy(x => x.Nombre).Select(x => new Opcion(x.Id, x.Nombre)).ToListAsync();
    }

    private static IQueryable<SolicitudCotizacion> Consulta(ApplicationDbContext db) =>
        db.Solicitudes.AsNoTracking().Include(x => x.Oferta).Include(x => x.Viaje).ThenInclude(x => x!.Salida).ThenInclude(x => x.Tour).OrderByDescending(x => x.Id);

    private static async Task<SolicitudCotizacion> Cargar(ApplicationDbContext db, int id, byte[] version, int? organizacion = null)
    {
        var solicitud = await db.Solicitudes.Include(x => x.Oferta).Include(x => x.Viaje)
            .SingleOrDefaultAsync(x => x.Id == id && (!organizacion.HasValue || x.OrganizacionId == organizacion))
            ?? throw new ReglaException("Solicitud inexistente.");
        db.Entry(solicitud).Property(x => x.Version).OriginalValue = version;
        return solicitud;
    }

    private static async Task Persistir(ApplicationDbContext db)
    {
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { throw new ReglaException("La cotización cambió. Recarga y vuelve a intentarlo."); }
        catch (DbUpdateException) { throw new ReglaException("La cotización ya fue procesada o la información vinculada cambió."); }
    }
}
