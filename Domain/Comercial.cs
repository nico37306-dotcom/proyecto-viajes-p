using System.ComponentModel.DataAnnotations;

namespace Agencia.Web.Domain;

public class Convenio : IRegistro
{
    public int Id { get; set; }
    public int? ProveedorId { get; set; }
    public int? OrganizacionId { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [MaxLength(2500)] public string Condiciones { get; set; } = "";
    public DateOnly Desde { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly Hasta { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
    public bool Activo { get; set; } = true;
    public List<ConvenioTarifa> Tarifas { get; set; } = [];
}

public class ConvenioTarifa : IRegistro
{
    public int Id { get; set; }
    public int ConvenioId { get; set; }
    public int? TourId { get; set; }
    public int? PaqueteId { get; set; }
    [Range(0.01, 100)] public decimal DescuentoPorcentaje { get; set; }
    [MaxLength(500)] public string Condiciones { get; set; } = "";
}

public class Promocion : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(140)] public string NombreEn { get; set; } = "";
    [MaxLength(40)] public string? Codigo { get; set; }
    public AlcancePromocion Alcance { get; set; }
    public int? ConvenioId { get; set; }
    public int? TourId { get; set; }
    public int? PaqueteId { get; set; }
    public DateOnly Desde { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly Hasta { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
    public bool Porcentaje { get; set; } = true;
    [Range(0.01, 9999999)] public decimal Valor { get; set; }
    public Moneda Moneda { get; set; }
    public bool Activa { get; set; } = true;
}

public class SolicitudCotizacion : IRegistro
{
    public int Id { get; set; }
    public int? OrganizacionId { get; set; }
    [Required, MaxLength(180)] public string NombreOrganizacion { get; set; } = "";
    [Required, EmailAddress, MaxLength(250)] public string Correo { get; set; } = "";
    public DateOnly FechaTentativa { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));
    [Range(1, 500)] public int Personas { get; set; } = 12;
    [Required, MaxLength(2500)] public string Destinos { get; set; } = "";
    [Range(0, 99999999)] public decimal? Presupuesto { get; set; }
    public Moneda Moneda { get; set; }
    public EstadoCotizacion Estado { get; set; }
    public Cotizacion? Oferta { get; set; }
    public ViajeGrupal? Viaje { get; set; }
    [ConcurrencyCheck] public byte[] Version { get; set; } = [];
}

public class Cotizacion : IRegistro
{
    public int Id { get; set; }
    public int SolicitudCotizacionId { get; set; }
    [Required, MaxLength(2500)] public string Detalle { get; set; } = "";
    public DateOnly ValidaHasta { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
    [Range(0.01, 99999999)] public decimal Precio { get; set; }
    public Moneda Moneda { get; set; }
}

public class ViajeGrupal : IRegistro
{
    public int Id { get; set; }
    public int SolicitudCotizacionId { get; set; }
    public int SalidaId { get; set; }
    public Salida Salida { get; set; } = null!;
    public DateTimeOffset Creado { get; set; } = DateTimeOffset.UtcNow;
}
