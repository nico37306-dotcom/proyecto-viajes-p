using System.Security.Claims;
using Agencia.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Authorization;

public static class Acceso
{
    public const string Admin = "Administrador";
    public const string Cliente = "Cliente";
    public const string Frecuente = "ClienteFrecuente";
    public const string Organizacion = "Organizacion";
    public const string Empleado = "Empleado";
    public const string Operativo = "Operativo";
    public const string Viajero = "Viajero";
    public const string Representante = "Representante";
    public const string Ventas = "Dept.Ventas";
    public const string Operaciones = "Dept.Operaciones";
    public const string Guias = "Dept.Guias";
    public const string Transporte = "Dept.Transporte";
    public const string Alianzas = "Dept.Alianzas";
    public const string Marketing = "Dept.Marketing";
    public const string Atencion = "Dept.Atencion";
    public const string Contabilidad = "Dept.Contabilidad";
    public const string Administracion = "Dept.Administracion";

    public static readonly string[] Roles = [Admin, Empleado, Cliente, Frecuente, Organizacion];
    public static readonly (string Clave, string Nombre)[] Departamentos =
    [
        (Ventas, "Ventas y Reservas"), (Operaciones, "Operaciones y Logística"), (Guias, "Guías"),
        (Transporte, "Transporte"), (Alianzas, "Alianzas y Convenios"), (Marketing, "Marketing y Promociones"),
        (Atencion, "Atención al Cliente"), (Contabilidad, "Contabilidad y Pagos"), (Administracion, "Administración")
    ];

    public static void Configurar(AuthorizationOptions options)
    {
        foreach (var permiso in Departamentos.Select(x => x.Clave).Concat([Admin, Operativo, Viajero, Representante]))
            options.AddPolicy(permiso, p => p.RequireAuthenticatedUser().AddRequirements(new Permiso(permiso)));
    }
}

public sealed record Actor(string Id, int? ClienteId, int? OrganizacionId, string? Departamento, string[] Roles)
{
    public bool EsAdmin => Roles.Contains(Acceso.Admin);
    public bool Tiene(string permiso) => permiso switch
    {
        Acceso.Viajero => (ClienteId.HasValue && (Roles.Contains(Acceso.Cliente) || Roles.Contains(Acceso.Frecuente))) ||
            (OrganizacionId.HasValue && Roles.Contains(Acceso.Organizacion)),
        Acceso.Representante => OrganizacionId.HasValue && Roles.Contains(Acceso.Organizacion),
        Acceso.Operativo => EsAdmin || (Roles.Contains(Acceso.Empleado) && Departamento is not null),
        Acceso.Admin => EsAdmin,
        _ => EsAdmin || (Roles.Contains(Acceso.Empleado) && Departamento == permiso)
    };
}

public sealed record Permiso(string Clave) : IAuthorizationRequirement;

public interface ICurrentUserService
{
    Task<Actor?> Actual();
    Task<Actor> Exigir(params string[] permisos);
}

public sealed class CurrentUserService(AuthenticationStateProvider provider, IDbContextFactory<ApplicationDbContext> factory) : ICurrentUserService
{
    public Task<Actor?> Actual() => Obtener();
    private async Task<Actor?> Obtener() => await Leer(factory, (await provider.GetAuthenticationStateAsync()).User);
    public async Task<Actor> Exigir(params string[] permisos)
    {
        var actor = await Actual();
        if (actor is null || !permisos.Any(actor.Tiene))
            throw new UnauthorizedAccessException("El usuario no tiene acceso a esta operación.");
        return actor;
    }

    internal static async Task<Actor?> Leer(IDbContextFactory<ApplicationDbContext> factory, ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id is null || principal.Identity?.IsAuthenticated != true) return null;
        await using var db = await factory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().Where(u => u.Id == id && u.Activo)
            .Select(u => new { u.Id, u.ClienteId, u.OrganizacionId, Departamento = u.Empleado == null ? null : u.Empleado.Departamento.Clave })
            .SingleOrDefaultAsync();
        if (user is null) return null;
        var roles = await (from link in db.UserRoles join role in db.Roles on link.RoleId equals role.Id
                           where link.UserId == id select role.Name!).ToArrayAsync();
        return new(user.Id, user.ClienteId, user.OrganizacionId, user.Departamento, roles);
    }
}

public sealed class PermisoHandler(IDbContextFactory<ApplicationDbContext> factory) : AuthorizationHandler<Permiso>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, Permiso requirement)
    {
        if ((await CurrentUserService.Leer(factory, context.User))?.Tiene(requirement.Clave) == true)
            context.Succeed(requirement);
    }
}
