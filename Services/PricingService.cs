using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public sealed record TotalReserva(decimal Subtotal, decimal Descuento, decimal Total, List<ReservaServicioAdicional> Extras);

public interface IPricingService
{
    Task<TotalReserva> Calcular(ApplicationDbContext db, Salida salida, Actor actor, int cantidad,
        Moneda moneda, IReadOnlyCollection<int> extras, string? codigo);
}

public sealed class PricingService : IPricingService
{
    public async Task<TotalReserva> Calcular(ApplicationDbContext db, Salida salida, Actor actor, int cantidad,
        Moneda moneda, IReadOnlyCollection<int> extras, string? codigo)
    {
        var precio = moneda == Moneda.BOB ? salida.PrecioBob ?? salida.Tour.PrecioBob : salida.PrecioUsd ?? salida.Tour.PrecioUsd;
        var subtotal = precio * cantidad;
        var hoy = Reglas.Hoy;
        var organ = actor.Tiene(Acceso.Representante) ? actor.OrganizacionId : null;
        var frecuente = actor.Roles.Contains(Acceso.Frecuente);
        var convenios = organ.HasValue ? await db.Convenios.AsNoTracking()
            .Where(x => x.Activo && x.OrganizacionId == organ && x.Desde <= hoy && x.Hasta >= hoy)
            .Include(x => x.Tarifas).ToListAsync() : [];
        var ids = convenios.Select(x => x.Id).ToArray();
        var codigoNormal = string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim().ToUpperInvariant();
        var promociones = await db.Promociones.AsNoTracking()
            .Where(x => x.Activa && x.Desde <= hoy && x.Hasta >= hoy && x.PaqueteId == null &&
                (x.TourId == null || x.TourId == salida.TourId) &&
                (x.Codigo == null || x.Codigo == codigoNormal) && (x.Porcentaje || x.Moneda == moneda)).ToListAsync();
        var aplicables = promociones.Where(x => x.Alcance switch
        {
            AlcancePromocion.General => true,
            AlcancePromocion.Frecuentes => frecuente,
            AlcancePromocion.Organizaciones => organ.HasValue,
            AlcancePromocion.Convenio => x.ConvenioId is int id && ids.Contains(id),
            _ => false
        }).ToList();
        if (codigoNormal is not null && !aplicables.Any(x => x.Codigo == codigoNormal))
            throw new ReglaException(Reglas.T("El código no está vigente para esta reserva.", "This code does not apply to this booking."));
        var descuentoConvenio = convenios.SelectMany(x => x.Tarifas).Where(x => x.TourId == salida.TourId)
            .Select(x => subtotal * x.DescuentoPorcentaje / 100).DefaultIfEmpty().Max();
        var descuentoPromo = aplicables.Select(x => x.Porcentaje ? subtotal * x.Valor / 100 : x.Valor).DefaultIfEmpty().Max();
        var descuento = Reglas.Redondear(Math.Clamp(Math.Max(descuentoConvenio, descuentoPromo), 0, subtotal));
        var servicios = await db.Extras.AsNoTracking().Where(x => x.Activo && extras.Contains(x.Id)).ToListAsync();
        if (servicios.Count != extras.Count) throw new ReglaException(Reglas.T("Un servicio adicional ya no está disponible.", "An add-on is no longer available."));
        var lineas = servicios.Select(x => new ReservaServicioAdicional
        {
            ServicioAdicionalId = x.Id, Nombre = x.Nombre, Precio = moneda == Moneda.BOB ? x.PrecioBob : x.PrecioUsd
        }).ToList();
        return new(subtotal, descuento, Reglas.Redondear(subtotal - descuento + lineas.Sum(x => x.Precio)), lineas);
    }
}
