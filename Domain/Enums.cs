namespace Agencia.Web.Domain;

public enum TipoTour { Cultural, Aventura, Naturaleza, Gastronomico, Comunitario }
public enum Dificultad { Facil, Moderada, Alta }
public enum EstadoSalida { Programada, Confirmada, Completa, Cancelada, Finalizada, Reprogramada, FuerzaMayor }
public enum EstadoReserva { Pendiente, EnRevision, Aprobada, PendienteDePago, Confirmada, Cancelada, Finalizada }
public enum Moneda { BOB, USD }
public enum MetodoPago { Efectivo, Transferencia, QR, Tarjeta }
public enum EstadoPago { Pendiente, Confirmado, Fallido, Reembolsado }
public enum TipoProveedor { Alojamiento, Transporte, Aerolinea, Restaurante, Operador, Comunidad, Guia, Otro }
public enum TipoOrganizacion { Empresa, Universidad, Colegio, ONG, Institucion, Agencia, Otra }
public enum AlcancePromocion { General, Frecuentes, Organizaciones, Convenio }
public enum EstadoCotizacion { Recibida, EnElaboracion, Enviada, Aceptada, Rechazada, Vencida }
public enum EstadoAsignacion { Pendiente, Confirmada, Cancelada }
