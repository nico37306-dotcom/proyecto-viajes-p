using Agencia.Web.Domain;

namespace Agencia.Web.Services;

public static class LocalEnum
{
    public static string Show(Enum value) => value switch
    {
        TipoTour.Cultural => Reglas.T("Cultural", "Culture"),
        TipoTour.Aventura => Reglas.T("Aventura", "Adventure"),
        TipoTour.Naturaleza => Reglas.T("Naturaleza", "Nature"),
        TipoTour.Gastronomico => Reglas.T("Gastronómico", "Food"),
        TipoTour.Comunitario => Reglas.T("Comunitario", "Community"),
        Dificultad.Facil => Reglas.T("Fácil", "Easy"),
        Dificultad.Moderada => Reglas.T("Moderada", "Moderate"),
        Dificultad.Alta => Reglas.T("Alta", "Challenging"),
        EstadoReserva.Pendiente => Reglas.T("Pendiente", "Pending"),
        EstadoReserva.EnRevision => Reglas.T("En revisión", "Under review"),
        EstadoReserva.Aprobada => Reglas.T("Aprobada", "Approved"),
        EstadoReserva.PendienteDePago => Reglas.T("Pendiente de pago", "Payment due"),
        EstadoReserva.Confirmada => Reglas.T("Confirmada", "Confirmed"),
        EstadoReserva.Cancelada => Reglas.T("Cancelada", "Cancelled"),
        EstadoReserva.Finalizada => Reglas.T("Finalizada", "Completed"),
        EstadoPago.Pendiente => Reglas.T("Pendiente", "Pending"),
        EstadoPago.Confirmado => Reglas.T("Confirmado", "Confirmed"),
        EstadoPago.Fallido => Reglas.T("Fallido", "Failed"),
        EstadoPago.Reembolsado => Reglas.T("Reembolsado", "Refunded"),
        MetodoPago.Efectivo => Reglas.T("Efectivo", "Cash"),
        MetodoPago.Transferencia => Reglas.T("Transferencia", "Bank transfer"),
        MetodoPago.Tarjeta => Reglas.T("Tarjeta", "Card"),
        EstadoCotizacion.Recibida => Reglas.T("Recibida", "Received"),
        EstadoCotizacion.EnElaboracion => Reglas.T("En elaboración", "In preparation"),
        EstadoCotizacion.Enviada => Reglas.T("Enviada", "Sent"),
        EstadoCotizacion.Aceptada => Reglas.T("Aceptada", "Accepted"),
        EstadoCotizacion.Rechazada => Reglas.T("Rechazada", "Declined"),
        EstadoCotizacion.Vencida => Reglas.T("Vencida", "Expired"),
        EstadoSalida.Programada => Reglas.T("Programada", "Scheduled"),
        EstadoSalida.Confirmada => Reglas.T("Confirmada", "Confirmed"),
        EstadoSalida.Completa => Reglas.T("Completa", "Full"),
        EstadoSalida.Cancelada => Reglas.T("Cancelada", "Cancelled"),
        EstadoSalida.Finalizada => Reglas.T("Finalizada", "Completed"),
        EstadoSalida.Reprogramada => Reglas.T("Reprogramada", "Rescheduled"),
        EstadoSalida.FuerzaMayor => Reglas.T("Cancelada por fuerza mayor", "Cancelled due to force majeure"),
        _ => Reglas.Nombre(value)
    };
}
