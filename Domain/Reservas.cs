using System.ComponentModel.DataAnnotations;

namespace Agencia.Web.Domain;

public class Reserva : IRegistro
{
    public int Id { get; set; }
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public int? OrganizacionId { get; set; }
    public Organizacion? Organizacion { get; set; }
    public int SalidaId { get; set; }
    public Salida Salida { get; set; } = null!;
    public int Cantidad { get; set; }
    public EstadoReserva Estado { get; set; }
    public decimal Total { get; set; }
    public decimal Descuento { get; set; }
    public Moneda Moneda { get; set; }
    [MaxLength(1500)] public string Notas { get; set; } = "";
    public DateTimeOffset Solicitada { get; set; } = DateTimeOffset.UtcNow;
    public Guid SolicitudId { get; set; }
    public List<Pasajero> Pasajeros { get; set; } = [];
    public List<ReservaServicioAdicional> Extras { get; set; } = [];
    public List<Pago> Pagos { get; set; } = [];
    [ConcurrencyCheck] public byte[] Version { get; set; } = [];
    public decimal Pagado => Pagos.Where(x => x.Estado == EstadoPago.Confirmado).Sum(x => x.Monto);
}

public class Pasajero : IRegistro
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [MaxLength(40)] public string Documento { get; set; } = "";
    [MaxLength(80)] public string Nacionalidad { get; set; } = "";
    public DateOnly? Nacimiento { get; set; }
    [MaxLength(500)] public string Requerimientos { get; set; } = "";
}

public class ServicioAdicional : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(140)] public string NombreEn { get; set; } = "";
    [Range(0, 9999999)] public decimal PrecioBob { get; set; }
    [Range(0, 9999999)] public decimal PrecioUsd { get; set; }
    public bool Activo { get; set; } = true;
}

public class ReservaServicioAdicional : IRegistro
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public int ServicioAdicionalId { get; set; }
    [MaxLength(140)] public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
}

public class Pago : IRegistro
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public decimal Monto { get; set; }
    public Moneda Moneda { get; set; }
    public MetodoPago Metodo { get; set; }
    public EstadoPago Estado { get; set; }
    [Required, MaxLength(120)] public string Referencia { get; set; } = "";
    [MaxLength(500)] public string Comprobante { get; set; } = "";
    public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
    [System.ComponentModel.DataAnnotations.Schema.NotMapped] public bool TieneArchivo { get; set; }
    public ComprobantePago? Archivo { get; set; }
    public List<TransaccionPago> Transacciones { get; set; } = [];
}

public class TransaccionPago : IRegistro
{
    public int Id { get; set; }
    public int PagoId { get; set; }
    public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;
    [MaxLength(120)] public string Evento { get; set; } = "";
    [MaxLength(120)] public string IdentificadorExterno { get; set; } = "";
}

public class ComprobantePago : IRegistro
{
    public int Id { get; set; }
    public int PagoId { get; set; }
    [Required, MaxLength(180)] public string Nombre { get; set; } = "";
    [Required, MaxLength(40)] public string Tipo { get; set; } = "";
    [Required] public byte[] Datos { get; set; } = [];
}
