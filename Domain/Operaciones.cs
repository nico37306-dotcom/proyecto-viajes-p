using System.ComponentModel.DataAnnotations;

namespace Agencia.Web.Domain;

public class Proveedor : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    public TipoProveedor Tipo { get; set; }
    [MaxLength(100)] public string Ciudad { get; set; } = "";
    [MaxLength(250)] public string Contacto { get; set; } = "";
    public bool Activo { get; set; } = true;
}

public class ServicioProveedor : IRegistro
{
    public int Id { get; set; }
    public int ProveedorId { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Range(0, 9999999)] public decimal Tarifa { get; set; }
    public Moneda Moneda { get; set; }
}

public class Guia : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [MaxLength(150)] public string Idiomas { get; set; } = "";
    [MaxLength(250)] public string Especialidades { get; set; } = "";
    public bool Disponible { get; set; } = true;
}

public class Vehiculo : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(25)] public string Placa { get; set; } = "";
    [Required, MaxLength(100)] public string Tipo { get; set; } = "";
    [Range(1, 120)] public int Capacidad { get; set; }
    public int? ProveedorId { get; set; }
    public bool Disponible { get; set; } = true;
}

public class AsignacionOperativa : IRegistro
{
    public int Id { get; set; }
    public int SalidaId { get; set; }
    public int? ProveedorId { get; set; }
    public int? GuiaId { get; set; }
    public int? VehiculoId { get; set; }
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required, MaxLength(500)] public string Detalle { get; set; } = "";
    public EstadoAsignacion Estado { get; set; }
}
