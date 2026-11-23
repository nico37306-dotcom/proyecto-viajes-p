using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public record Perfil(Cliente Cliente, List<ClienteBeneficio> Beneficios, List<Promocion> Promociones);
public record FichaCliente(Cliente Cliente, List<Reserva> Reservas, List<ClienteBeneficio> Beneficios, List<Resena> Resenas);

public sealed class ProfileService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current) : IClientService
{
    public async Task<Perfil?> Obtener()
    {
        var actor = await current.Exigir(Acceso.Viajero);
        if (!actor.ClienteId.HasValue || actor.Tiene(Acceso.Representante)) return null;
        await using var db = await factory.CreateDbContextAsync();
        var hoy = Reglas.Hoy;
        var cliente = (await db.Clientes.FindAsync(actor.ClienteId))!;
        var beneficios = await db.Set<ClienteBeneficio>().Where(x => x.ClienteId == cliente.Id)
            .OrderBy(x => x.Utilizado).ThenBy(x => x.Vence).ToListAsync();
        var promociones = cliente.Frecuente ? await db.Promociones.AsNoTracking().Where(x => x.Activa &&
            x.Alcance == AlcancePromocion.Frecuentes && x.Desde <= hoy && x.Hasta >= hoy).ToListAsync() : [];
        return new(cliente, beneficios, promociones);
    }

    public async Task<FichaCliente?> Ficha(int id)
    {
        await current.Exigir(Acceso.Admin);
        await using var db = await factory.CreateDbContextAsync();
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return null;
        var reservas = db.Reservas.AsNoTracking().Where(x => x.ClienteId == id);
        return new(cliente, await reservas.Include(x => x.Salida).ThenInclude(x => x.Tour).OrderByDescending(x => x.Solicitada).ToListAsync(),
            await db.Set<ClienteBeneficio>().Where(x => x.ClienteId == id).ToListAsync(),
            await db.Set<Resena>().Where(x => reservas.Any(r => r.Id == x.ReservaId)).ToListAsync());
    }

    public async Task Guardar(Cliente input)
    {
        var actor = await current.Exigir(Acceso.Viajero);
        if (actor.ClienteId is null || actor.Tiene(Acceso.Representante)) throw new UnauthorizedAccessException();
        Reglas.Validar(input);
        if (input.Idioma is not ("es" or "en")) throw new ReglaException("Idioma inválido.");
        await using var db = await factory.CreateDbContextAsync();
        var cliente = (await db.Clientes.FindAsync(actor.ClienteId))!;
        cliente.Nombre = input.Nombre.Trim();
        cliente.Nacionalidad = input.Nacionalidad.Trim();
        cliente.Documento = input.Documento.Trim();
        cliente.Telefono = input.Telefono.Trim();
        cliente.Nit = input.Nit.Trim();
        cliente.RazonSocial = input.RazonSocial.Trim();
        cliente.Preferencias = input.Preferencias.Trim();
        cliente.Idioma = input.Idioma;
        var user = await db.Users.FindAsync(actor.Id);
        if (user is not null) user.Idioma = input.Idioma;
        await db.SaveChangesAsync();
    }

    public async Task Comentar(int reservaId, int puntuacion, string texto)
    {
        var actor = await current.Exigir(Acceso.Viajero);
        var resena = new Resena { ReservaId = reservaId, Puntuacion = puntuacion, Comentario = texto.Trim() };
        Reglas.Validar(resena);
        await using var db = await factory.CreateDbContextAsync();
        if (!await BookingService.Propias(db.Reservas, actor).AnyAsync(x => x.Id == reservaId && x.Estado == EstadoReserva.Finalizada))
            throw new UnauthorizedAccessException();
        if (await db.Set<Resena>().AnyAsync(x => x.ReservaId == reservaId)) throw new ReglaException("Ya existe una reseña para esta reserva.");
        db.Add(resena);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException) { throw new ReglaException("La reseña ya existe o la reserva cambió."); }
    }
}
