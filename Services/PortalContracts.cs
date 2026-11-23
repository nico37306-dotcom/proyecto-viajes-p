using Agencia.Web.Domain;

namespace Agencia.Web.Services;

public interface IClientService
{
    Task<Perfil?> Obtener();
    Task<FichaCliente?> Ficha(int id);
    Task Guardar(Cliente input);
    Task Comentar(int reservaId, int puntuacion, string texto);
}

public interface IOperationsService
{
    Task<List<Salida>> Listar();
    Task<Operacion?> Obtener(int id);
    Task Guardar(int id, byte[] version, int? guia, int? vehiculo, string checklist);
    Task Cambiar(int id, byte[] version, EstadoSalida estado, DateOnly inicio, DateOnly fin);
}

public interface IOrganizationService
{
    Task<PortalOrganizacion> Portal();
    Task<List<Opcion>> Organizaciones();
}

public interface IConventionService
{
    Task<List<TarifaVisible>> Tarifas(int convenioId);
}

public interface IQuotationService
{
    Task Solicitar(SolicitudCotizacion input, bool portal);
    Task<List<SolicitudCotizacion>> Bandeja();
    Task<SolicitudCotizacion?> Obtener(int id, bool staff);
    Task Preparar(int id, byte[] version, int? organizacion, EstadoCotizacion estado, Cotizacion input);
    Task Responder(int id, byte[] version, bool aceptar);
}

public interface IGroupTripService
{
    Task Convertir(int id, byte[] version, int tourId, DateOnly inicio);
}

public interface IPromotionService
{
    Task<List<Promocion>> Promociones();
}
