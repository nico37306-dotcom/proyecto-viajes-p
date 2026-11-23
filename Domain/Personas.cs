using System.ComponentModel.DataAnnotations;

namespace Agencia.Web.Domain;

public class Cliente : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [MaxLength(80)] public string Nacionalidad { get; set; } = "";
    [MaxLength(40)] public string Documento { get; set; } = "";
    [MaxLength(40)] public string Telefono { get; set; } = "";
    [MaxLength(10)] public string Idioma { get; set; } = "es";
    [MaxLength(500)] public string Preferencias { get; set; } = "";
    [MaxLength(40)] public string Nit { get; set; } = "";
    [MaxLength(180)] public string RazonSocial { get; set; } = "";
    public bool Frecuente { get; set; }
}

public class ClienteBeneficio : IRegistro
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    [Required, MaxLength(250)] public string Descripcion { get; set; } = "";
    public DateOnly? Vence { get; set; }
    public bool Utilizado { get; set; }
}

public class Departamento : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Nombre { get; set; } = "";
    [Required, MaxLength(40)] public string Clave { get; set; } = "";
}

public class Empleado : IRegistro
{
    public int Id { get; set; }
    public int DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(100)] public string Cargo { get; set; } = "";
    [MaxLength(100)] public string Contacto { get; set; } = "";
}

public class Organizacion : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(180)] public string Nombre { get; set; } = "";
    public TipoOrganizacion Tipo { get; set; }
    [Required, EmailAddress, MaxLength(250)] public string Correo { get; set; } = "";
    [MaxLength(100)] public string Representante { get; set; } = "";
    [MaxLength(40)] public string Telefono { get; set; } = "";
}

public class Resena : IRegistro
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    [Range(1, 5)] public int Puntuacion { get; set; } = 5;
    [Required, MaxLength(1500)] public string Comentario { get; set; } = "";
    public bool Publicada { get; set; }
    public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
}

public class MensajeContacto : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, EmailAddress, MaxLength(250)] public string Correo { get; set; } = "";
    [Required, MaxLength(2500)] public string Mensaje { get; set; } = "";
    public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
    public bool Atendido { get; set; }
}
