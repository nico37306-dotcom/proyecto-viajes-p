using System.ComponentModel.DataAnnotations;

namespace Agencia.Web.Domain;

public interface IRegistro { int Id { get; set; } }

public class Region : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Nombre { get; set; } = "";
    [Required, MaxLength(100)] public string NombreEn { get; set; } = "";
}

public class Destino : IRegistro
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(140)] public string NombreEn { get; set; } = "";
    [Required, MaxLength(2500)] public string Descripcion { get; set; } = "";
    [Required, MaxLength(2500)] public string DescripcionEn { get; set; } = "";
    [MaxLength(250)] public string MejorEpoca { get; set; } = "";
    [MaxLength(250)] public string MejorEpocaEn { get; set; } = "";
    [MaxLength(500)] public string Recomendaciones { get; set; } = "";
    [MaxLength(500)] public string RecomendacionesEn { get; set; } = "";
    [Range(0, 7000)] public int Altitud { get; set; }
    [Range(-90, 90)] public double? Latitud { get; set; }
    [Range(-180, 180)] public double? Longitud { get; set; }
    [MaxLength(500)] public string Imagen { get; set; } = "";
}

public class Tour : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(140)] public string NombreEn { get; set; } = "";
    [Required, MaxLength(2500)] public string Descripcion { get; set; } = "";
    [Required, MaxLength(2500)] public string DescripcionEn { get; set; } = "";
    [MaxLength(1000)] public string Incluye { get; set; } = "";
    [MaxLength(1000)] public string IncluyeEn { get; set; } = "";
    [MaxLength(1000)] public string NoIncluye { get; set; } = "";
    [MaxLength(1000)] public string NoIncluyeEn { get; set; } = "";
    [MaxLength(1000)] public string Requisitos { get; set; } = "";
    [MaxLength(1000)] public string RequisitosEn { get; set; } = "";
    [MaxLength(1000)] public string Cancelacion { get; set; } = "";
    [MaxLength(1000)] public string CancelacionEn { get; set; } = "";
    [MaxLength(500)] public string Imagen { get; set; } = "";
    public TipoTour Tipo { get; set; }
    public Dificultad Dificultad { get; set; }
    [Range(1, 60)] public int Dias { get; set; } = 1;
    [Range(0.01, 9999999)] public decimal PrecioBob { get; set; }
    [Range(0.01, 9999999)] public decimal PrecioUsd { get; set; }
    [Range(1, 500)] public int Minimo { get; set; } = 1;
    [Range(1, 500)] public int Maximo { get; set; } = 12;
    public bool Publicado { get; set; }
    public bool Destacado { get; set; }
    public List<TourDestino> Destinos { get; set; } = [];
    public List<ItinerarioDia> Itinerario { get; set; } = [];
    public List<Salida> Salidas { get; set; } = [];
    public List<TourFoto> Fotos { get; set; } = [];
}

public class TourDestino : IRegistro
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public int DestinoId { get; set; }
    public Destino Destino { get; set; } = null!;
    [Range(1, 60)] public int Orden { get; set; } = 1;
}

public class ItinerarioDia : IRegistro
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Range(1, 60)] public int Dia { get; set; } = 1;
    [Required, MaxLength(140)] public string Titulo { get; set; } = "";
    [Required, MaxLength(140)] public string TituloEn { get; set; } = "";
    [Required, MaxLength(2000)] public string Actividades { get; set; } = "";
    [Required, MaxLength(2000)] public string ActividadesEn { get; set; } = "";
    [MaxLength(250)] public string Alojamiento { get; set; } = "";
    [MaxLength(250)] public string AlojamientoEn { get; set; } = "";
    [MaxLength(250)] public string Comidas { get; set; } = "";
    [MaxLength(250)] public string ComidasEn { get; set; } = "";
}

public class TourFoto : IRegistro
{
    public int Id { get; set; }
    public int TourId { get; set; }
    [Required, MaxLength(500)] public string Imagen { get; set; } = "";
    [Required, MaxLength(140)] public string Descripcion { get; set; } = "";
    [Required, MaxLength(140)] public string DescripcionEn { get; set; } = "";
}

public class Salida : IRegistro
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;
    public DateOnly Inicio { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    public DateOnly Fin { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
    [Range(1, 500)] public int Cupos { get; set; } = 12;
    public int Ocupados { get; set; }
    [Range(0.01, 9999999)] public decimal? PrecioBob { get; set; }
    [Range(0.01, 9999999)] public decimal? PrecioUsd { get; set; }
    public EstadoSalida Estado { get; set; }
    public int? GuiaId { get; set; }
    public Guia? Guia { get; set; }
    public int? VehiculoId { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public bool Privada { get; set; }
    [MaxLength(2500)] public string Checklist { get; set; } = "";
    [Timestamp] public byte[] Version { get; set; } = [];
    public int Disponibles => Cupos - Ocupados;
}

public class Paquete : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(140)] public string Nombre { get; set; } = "";
    [Required, MaxLength(140)] public string NombreEn { get; set; } = "";
    [Required, MaxLength(2500)] public string Descripcion { get; set; } = "";
    [Required, MaxLength(2500)] public string DescripcionEn { get; set; } = "";
    [Range(0.01, 9999999)] public decimal PrecioBob { get; set; }
    [Range(0.01, 9999999)] public decimal PrecioUsd { get; set; }
    [MaxLength(500)] public string Imagen { get; set; } = "";
    public bool Publicado { get; set; }
    public List<PaqueteTour> Tours { get; set; } = [];
}

public class PaqueteTour : IRegistro
{
    public int Id { get; set; }
    public int PaqueteId { get; set; }
    public int TourId { get; set; }
    public Tour Tour { get; set; } = null!;
}

public class PaginaContenido : IRegistro
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Slug { get; set; } = "";
    [Required, MaxLength(140)] public string Titulo { get; set; } = "";
    [Required, MaxLength(140)] public string TituloEn { get; set; } = "";
    [Required, MaxLength(5000)] public string Cuerpo { get; set; } = "";
    [Required, MaxLength(5000)] public string CuerpoEn { get; set; } = "";
    public bool Publicado { get; set; }
}
