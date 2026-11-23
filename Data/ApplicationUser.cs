using Agencia.Web.Domain;
using Microsoft.AspNetCore.Identity;

namespace Agencia.Web.Data;

public class ApplicationUser : IdentityUser
{
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }
    public int? OrganizacionId { get; set; }
    public Organizacion? Organizacion { get; set; }
    public bool Activo { get; set; } = true;
    public string Idioma { get; set; } = "es";
}
