using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public record FiltroTour(int? Region = null, TipoTour? Tipo = null, Dificultad? Dificultad = null, int? Dias = null,
    decimal? Maximo = null, DateOnly? Fecha = null, Moneda Moneda = Moneda.BOB, string? Buscar = null, int? Destino = null);

public interface ITourService
{
    Task<List<Tour>> Buscar(FiltroTour filtro, bool destacados = false);
    Task<Tour?> Detalle(int id);
}
public interface IDestinationService
{
    Task<List<Destino>> Destinos();
    Task<List<Region>> Regiones();
}
public interface IDepartureService
{
    Task<List<Salida>> Salidas(int? tourId = null);
    Task<Salida?> Salida(int id);
}

public sealed class CatalogService(IDbContextFactory<ApplicationDbContext> factory) : ITourService, IDestinationService, IDepartureService, IPromotionService
{
    private static IQueryable<Salida> Disponibles(ApplicationDbContext db)
    {
        var hoy = Reglas.Hoy;
        return db.Salidas.AsNoTracking().Where(x => x.Tour.Publicado && !x.Privada && x.Inicio >= hoy &&
            (x.Estado == EstadoSalida.Programada || x.Estado == EstadoSalida.Confirmada || x.Estado == EstadoSalida.Completa));
    }

    public async Task<List<Tour>> Buscar(FiltroTour filtro, bool destacados = false)
    {
        await using var db = await factory.CreateDbContextAsync();
        var query = db.Tours.AsNoTracking().Where(x => x.Publicado);
        if (destacados) query = query.Where(x => x.Destacado);
        if (filtro.Region.HasValue) query = query.Where(x => x.Destinos.Any(d => d.Destino.RegionId == filtro.Region));
        if (filtro.Destino.HasValue) query = query.Where(x => x.Destinos.Any(d => d.DestinoId == filtro.Destino));
        if (filtro.Tipo.HasValue) query = query.Where(x => x.Tipo == filtro.Tipo);
        if (filtro.Dificultad.HasValue) query = query.Where(x => x.Dificultad == filtro.Dificultad);
        if (filtro.Dias.HasValue) query = query.Where(x => x.Dias <= filtro.Dias);
        if (filtro.Maximo.HasValue)
            query = filtro.Moneda == Moneda.BOB ? query.Where(x => x.PrecioBob <= filtro.Maximo) : query.Where(x => x.PrecioUsd <= filtro.Maximo);
        if (filtro.Fecha.HasValue)
        {
            var disponibles = Disponibles(db).Where(s => s.Inicio == filtro.Fecha && s.Ocupados < s.Cupos);
            query = query.Where(x => disponibles.Any(s => s.TourId == x.Id));
        }
        if (!string.IsNullOrWhiteSpace(filtro.Buscar))
        {
            var buscar = filtro.Buscar.Trim();
            query = query.Where(x => x.Nombre.Contains(buscar) || x.NombreEn.Contains(buscar) ||
                x.Destinos.Any(d => d.Destino.Nombre.Contains(buscar) || d.Destino.NombreEn.Contains(buscar)));
        }
        return await query.Include(x => x.Destinos.OrderBy(d => d.Orden)).ThenInclude(x => x.Destino).ThenInclude(x => x.Region)
            .OrderByDescending(x => x.Destacado).ThenBy(x => x.Id).ToListAsync();
    }

    public async Task<Tour?> Detalle(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Tours.AsNoTracking().AsSplitQuery().Include(x => x.Destinos.OrderBy(d => d.Orden))
            .ThenInclude(x => x.Destino).ThenInclude(x => x.Region)
            .Include(x => x.Itinerario.OrderBy(d => d.Dia)).Include(x => x.Fotos)
            .SingleOrDefaultAsync(x => x.Id == id && x.Publicado);
    }

    public async Task<List<Destino>> Destinos()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Destinos.AsNoTracking().Include(x => x.Region).OrderBy(x => x.Nombre).ToListAsync();
    }

    public async Task<List<Region>> Regiones()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Regiones.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();
    }

    public async Task<List<Salida>> Salidas(int? tourId = null)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await Disponibles(db).Where(x => !tourId.HasValue || x.TourId == tourId)
            .Include(x => x.Tour).OrderBy(x => x.Inicio).ToListAsync();
    }

    public async Task<Salida?> Salida(int id)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await Disponibles(db).Include(x => x.Tour).SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Paquete>> Paquetes()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Paquetes.AsNoTracking().Where(x => x.Publicado).Include(x => x.Tours.Where(t => t.Tour.Publicado))
            .ThenInclude(x => x.Tour).OrderBy(x => x.Nombre).ToListAsync();
    }

    public async Task<List<Promocion>> Promociones()
    {
        await using var db = await factory.CreateDbContextAsync();
        var hoy = Reglas.Hoy;
        return await db.Promociones.AsNoTracking().Where(x => x.Activa && x.Alcance == AlcancePromocion.General && x.Desde <= hoy && x.Hasta >= hoy)
            .OrderBy(x => x.Hasta).ToListAsync();
    }

    public async Task<List<PaginaContenido>> Contenido()
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Contenido.AsNoTracking().Where(x => x.Publicado).OrderBy(x => x.Titulo).ToListAsync();
    }

    public async Task Contacto(MensajeContacto mensaje)
    {
        Reglas.Validar(mensaje);
        await using var db = await factory.CreateDbContextAsync();
        db.Add(new MensajeContacto { Nombre = mensaje.Nombre.Trim(), Correo = mensaje.Correo.Trim(), Mensaje = mensaje.Mensaje.Trim() });
        await db.SaveChangesAsync();
    }
}
