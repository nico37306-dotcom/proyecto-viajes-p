using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public sealed record Cuenta(string Id, string Correo, string Rol, bool Activa, int? Cliente, int? Empleado, int? Organizacion);

public sealed class AccountService(IServiceScopeFactory scopes, ICurrentUserService current)
{
    public async Task<ApplicationUser> Registrar(string nombre, string email, string password)
    {
        await using var scope = scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var perfil = new Cliente { Nombre = nombre.Trim(), Idioma = Reglas.English ? "en" : "es" };
        Reglas.Validar(perfil);
        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = new ApplicationUser { Cliente = perfil, UserName = email.Trim(), Email = email.Trim(), Idioma = perfil.Idioma };
        Verificar(await users.CreateAsync(user, password));
        Verificar(await users.AddToRoleAsync(user, Acceso.Cliente));
        await transaction.CommitAsync();
        return user;
    }

    public async Task<List<Cuenta>> Listar()
    {
        await current.Exigir(Acceso.Admin);
        await using var scope = scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await (from u in db.Users.AsNoTracking()
                      join ur in db.UserRoles on u.Id equals ur.UserId
                      join r in db.Roles on ur.RoleId equals r.Id
                      orderby u.Email
                      select new Cuenta(u.Id, u.Email!, r.Name!, u.Activo, u.ClienteId, u.EmpleadoId, u.OrganizacionId)).ToListAsync();
    }

    public async Task Guardar(string? id, string email, string password, string rol, bool activo, int? empleado, int? organizacion, int? cliente)
    {
        var actor = await current.Exigir(Acceso.Admin);
        await using var scope = scopes.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!Acceso.Roles.Contains(rol) || !await roles.RoleExistsAsync(rol))
            throw new ReglaException("Rol inválido.");
        if (actor.Id == id) throw new ReglaException("Usa otra cuenta administradora para editar esta cuenta.");
        if (rol == Acceso.Empleado && (empleado is null || !await db.Empleados.AnyAsync(x => x.Id == empleado)))
            throw new ReglaException("Selecciona un empleado válido.");
        if (rol == Acceso.Organizacion && (organizacion is null || !await db.Organizaciones.AnyAsync(x => x.Id == organizacion)))
            throw new ReglaException("Selecciona una organización válida.");
        var viajero = rol is Acceso.Cliente or Acceso.Frecuente;
        if (viajero && (cliente is null || !await db.Clientes.AnyAsync(x => x.Id == cliente)))
            throw new ReglaException("Selecciona un cliente válido.");

        await using var transaction = await db.Database.BeginTransactionAsync();
        var user = id is null ? new ApplicationUser { UserName = email.Trim(), Email = email.Trim() }
            : await users.FindByIdAsync(id) ?? throw new ReglaException("Cuenta inexistente.");
        // Preserve ownership links when changing access roles so historical reservations remain linked.
        if (viajero && user.ClienteId.HasValue && user.ClienteId != cliente)
            throw new ReglaException("La cuenta ya está vinculada a otro cliente.");
        if (viajero && await db.Users.AnyAsync(x => x.ClienteId == cliente && x.Id != user.Id))
            throw new ReglaException("El cliente ya tiene una cuenta.");
        if (rol == Acceso.Empleado && await db.Users.AnyAsync(x => x.EmpleadoId == empleado && x.Id != user.Id))
            throw new ReglaException("El empleado ya tiene una cuenta.");
        user.Activo = activo;
        if (viajero) user.ClienteId = cliente;
        user.EmpleadoId = rol == Acceso.Empleado ? empleado : null;
        user.OrganizacionId = rol == Acceso.Organizacion ? organizacion : null;

        if (id is null) Verificar(await users.CreateAsync(user, password));
        else Verificar(await users.UpdateAsync(user));
        var anteriores = await users.GetRolesAsync(user);
        if (!anteriores.SequenceEqual([rol]))
        {
            Verificar(await users.RemoveFromRolesAsync(user, anteriores));
            Verificar(await users.AddToRoleAsync(user, rol));
        }
        if (viajero)
        {
            var perfil = await db.Clientes.FindAsync(cliente);
            perfil!.Frecuente = rol == Acceso.Frecuente;
            await db.SaveChangesAsync();
        }
        Verificar(await users.UpdateSecurityStampAsync(user));
        await transaction.CommitAsync();
    }

    public static void Verificar(IdentityResult result)
    {
        if (!result.Succeeded) throw new ReglaException(string.Join(" ", result.Errors.Select(x => x.Description)));
    }
}
