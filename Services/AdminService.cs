using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Agencia.Web.Authorization;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Services;

public sealed record Modulo(string Clave, string Titulo, Type Tipo, string[] Permisos, string[] Campos, string[] Columnas, bool SoloEditar = false);
public sealed record Opcion(int Id, string Texto);
public sealed record Campo(PropertyInfo Propiedad, List<Opcion>? Opciones)
{
    public string Clave => Propiedad.Name;
    public string Titulo => Clave.EndsWith("Id", StringComparison.Ordinal) ? Clave[..^2] : Clave;
    public Type Tipo => Nullable.GetUnderlyingType(Propiedad.PropertyType) ?? Propiedad.PropertyType;
    public bool Opcional => Nullable.GetUnderlyingType(Propiedad.PropertyType) is not null;
    public bool Largo => Propiedad.GetCustomAttribute<MaxLengthAttribute>()?.Length >= 500;
}

public sealed class AdminService(IDbContextFactory<ApplicationDbContext> factory, ICurrentUserService current)
{
    private static Modulo M<T>(string clave, string titulo, string permiso, string campos, string columnas, bool soloEditar = false)
        where T : IRegistro => new(clave, titulo, typeof(T), permiso.Split(','), campos.Split(' '), columnas.Split(' '), soloEditar);

    public static readonly Modulo[] Modulos =
    [
        M<Region>("regiones", "Regiones", Acceso.Marketing, "Nombre NombreEn", "Nombre NombreEn"),
        M<Destino>("destinos", "Destinos", Acceso.Marketing, "RegionId Nombre NombreEn Descripcion DescripcionEn Altitud MejorEpoca MejorEpocaEn Recomendaciones RecomendacionesEn Latitud Longitud Imagen", "Nombre Altitud MejorEpoca"),
        M<Tour>("tours", "Tours", Acceso.Marketing, "Nombre NombreEn Descripcion DescripcionEn Tipo Dificultad Dias PrecioBob PrecioUsd Minimo Maximo Incluye IncluyeEn NoIncluye NoIncluyeEn Requisitos RequisitosEn Cancelacion CancelacionEn Imagen Publicado Destacado", "Nombre Tipo Dias PrecioBob Publicado"),
        M<TourDestino>("rutas", "Destinos por tour", Acceso.Marketing, "TourId DestinoId Orden", "TourId DestinoId Orden"),
        M<ItinerarioDia>("itinerarios", "Itinerarios", Acceso.Marketing, "TourId Dia Titulo TituloEn Actividades ActividadesEn Alojamiento AlojamientoEn Comidas ComidasEn", "TourId Dia Titulo"),
        M<TourFoto>("fotos", "Galería de tours", Acceso.Marketing, "TourId Imagen Descripcion DescripcionEn", "TourId Descripcion"),
        M<Salida>("salidas", "Calendario de salidas", Acceso.Operaciones + "," + Acceso.Marketing, "TourId Inicio Fin Cupos PrecioBob PrecioUsd", "TourId Inicio Estado Cupos Ocupados"),
        M<Paquete>("paquetes", "Paquetes", Acceso.Marketing, "Nombre NombreEn Descripcion DescripcionEn PrecioBob PrecioUsd Imagen Publicado", "Nombre PrecioBob PrecioUsd Publicado"),
        M<PaqueteTour>("paquete-tours", "Tours por paquete", Acceso.Marketing, "PaqueteId TourId", "PaqueteId TourId"),
        M<PaginaContenido>("contenido", "Guía y noticias", Acceso.Marketing, "Slug Titulo TituloEn Cuerpo CuerpoEn Publicado", "Slug Titulo Publicado"),
        M<Promocion>("promociones", "Promociones", Acceso.Marketing, "Nombre NombreEn Codigo Alcance ConvenioId TourId PaqueteId Desde Hasta Porcentaje Valor Moneda Activa", "Nombre Codigo Alcance Hasta Activa"),
        M<ServicioAdicional>("extras", "Servicios adicionales", Acceso.Marketing, "Nombre NombreEn PrecioBob PrecioUsd Activo", "Nombre PrecioBob PrecioUsd Activo"),
        M<Cliente>("clientes", "Clientes", Acceso.Admin, "Nombre Nacionalidad Documento Telefono Idioma Preferencias Nit RazonSocial", "Nombre Nacionalidad Frecuente Telefono"),
        M<ClienteBeneficio>("beneficios", "Beneficios", Acceso.Admin, "ClienteId Descripcion Vence Utilizado", "ClienteId Descripcion Vence Utilizado"),
        M<Empleado>("empleados", "Empleados", Acceso.Admin, "DepartamentoId Nombre Cargo Contacto", "Nombre DepartamentoId Cargo"),
        M<Proveedor>("proveedores", "Proveedores", Acceso.Alianzas, "Nombre Tipo Ciudad Contacto Activo", "Nombre Tipo Ciudad Activo"),
        M<ServicioProveedor>("servicios", "Tarifas de proveedores", Acceso.Alianzas, "ProveedorId Nombre Tarifa Moneda", "ProveedorId Nombre Tarifa Moneda"),
        M<Guia>("guias", "Guías", Acceso.Operaciones + "," + Acceso.Guias, "Nombre Idiomas Especialidades Disponible", "Nombre Idiomas Disponible"),
        M<Vehiculo>("vehiculos", "Vehículos", Acceso.Operaciones + "," + Acceso.Transporte, "Placa Tipo Capacidad ProveedorId Disponible", "Placa Tipo Capacidad Disponible"),
        M<AsignacionOperativa>("asignaciones", "Asignaciones", Acceso.Operaciones, "SalidaId ProveedorId GuiaId VehiculoId Fecha Detalle Estado", "SalidaId Fecha Detalle Estado"),
        M<Organizacion>("organizaciones", "Organizaciones", Acceso.Alianzas, "Nombre Tipo Correo Representante Telefono", "Nombre Tipo Correo"),
        M<Convenio>("convenios", "Convenios", Acceso.Alianzas, "Nombre ProveedorId OrganizacionId Condiciones Desde Hasta Activo", "Nombre OrganizacionId Hasta Activo"),
        M<ConvenioTarifa>("tarifas", "Tarifas de convenio", Acceso.Alianzas, "ConvenioId TourId PaqueteId DescuentoPorcentaje Condiciones", "ConvenioId TourId PaqueteId DescuentoPorcentaje"),
        M<MensajeContacto>("contacto", "Bandeja de contacto", Acceso.Atencion, "Atendido", "Nombre Correo Mensaje Fecha Atendido", true),
        M<Resena>("resenas", "Reseñas de viajeros", Acceso.Atencion, "Publicada", "ReservaId Puntuacion Comentario Fecha Publicada", true)
    ];

    private static readonly Dictionary<string, Type> Referencias = new()
    {
        ["RegionId"] = typeof(Region), ["DestinoId"] = typeof(Destino), ["TourId"] = typeof(Tour),
        ["PaqueteId"] = typeof(Paquete), ["ClienteId"] = typeof(Cliente), ["DepartamentoId"] = typeof(Departamento),
        ["ProveedorId"] = typeof(Proveedor), ["GuiaId"] = typeof(Guia), ["VehiculoId"] = typeof(Vehiculo),
        ["SalidaId"] = typeof(Salida), ["OrganizacionId"] = typeof(Organizacion), ["ConvenioId"] = typeof(Convenio)
    };

    public async Task<(List<IRegistro> Filas, List<Campo> Campos, List<Campo> Columnas)> Abrir(string clave)
    {
        var modulo = Buscar(clave);
        await current.Exigir(modulo.Permisos);
        await using var db = await factory.CreateDbContextAsync();
        var filas = await Filas(db, modulo.Tipo);
        var campos = new Dictionary<string, Campo>();
        foreach (var nombre in modulo.Campos.Concat(modulo.Columnas).Distinct())
            campos[nombre] = new(modulo.Tipo.GetProperty(nombre)!, Referencias.TryGetValue(nombre, out var tipo) ? await Opciones(db, tipo) : null);
        return (filas, modulo.Campos.Select(x => campos[x]).ToList(), modulo.Columnas.Select(x => campos[x]).ToList());
    }

    public async Task<List<Opcion>> ReferenciasCuenta(Type tipo)
    {
        await current.Exigir(Acceso.Admin);
        if (tipo != typeof(Cliente) && tipo != typeof(Organizacion) && tipo != typeof(Empleado)) throw new UnauthorizedAccessException();
        await using var db = await factory.CreateDbContextAsync();
        return await Opciones(db, tipo);
    }

    public async Task Guardar(string clave, int id, Dictionary<string, string> valores, byte[]? version)
    {
        var modulo = Buscar(clave);
        await current.Exigir(modulo.Permisos);
        await using var db = await factory.CreateDbContextAsync();
        var entity = id == 0 && !modulo.SoloEditar ? (IRegistro)Activator.CreateInstance(modulo.Tipo)!
            : (IRegistro?)await db.FindAsync(modulo.Tipo, [id]) ?? throw new ReglaException("Registro inexistente.");
        foreach (var nombre in modulo.Campos)
        {
            var prop = modulo.Tipo.GetProperty(nombre)!;
            var campo = new Campo(prop, null);
            if (!valores.TryGetValue(nombre, out var texto)) throw new ReglaException("Faltan datos para guardar el registro.");
            var valor = Convertir(campo, texto.Trim());
            if (Referencias.TryGetValue(nombre, out var tipo) && valor is int fk && await db.FindAsync(tipo, [fk]) is null)
                throw new ReglaException($"Selecciona una opción válida para {campo.Titulo}.");
            if (valor is string url && nombre == "Imagen" && url.Length > 0 &&
                !(url.StartsWith("/images/", StringComparison.Ordinal) && !url.Contains('\\')) &&
                !(Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == "https"))
                throw new ReglaException("Usa una URL HTTPS o una ruta local de imágenes.");
            prop.SetValue(entity, valor);
        }
        Reglas.Validar(entity);
        await Validar(db, entity);
        if (entity is Salida salida && id > 0)
            db.Entry(salida).Property(x => x.Version).OriginalValue = version ?? throw new ReglaException("Actualiza la salida antes de editarla.");
        if (id == 0) db.Add(entity);
        await Persistir(db);
    }

    public async Task Eliminar(string clave, int id)
    {
        var modulo = Buscar(clave);
        await current.Exigir(modulo.Permisos);
        if (modulo.SoloEditar) throw new UnauthorizedAccessException();
        await using var db = await factory.CreateDbContextAsync();
        db.Remove(await db.FindAsync(modulo.Tipo, [id]) ?? throw new ReglaException("Registro inexistente."));
        await Persistir(db);
    }

    public static Modulo Buscar(string clave) => Modulos.SingleOrDefault(x => x.Clave == clave) ?? throw new ReglaException("Módulo inexistente.");

    public static string Valor(PropertyInfo prop, object obj) => prop.GetValue(obj) switch
    {
        DateOnly fecha => fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        bool check => check ? "true" : "false",
        IFormattable valor => valor.ToString(null, CultureInfo.InvariantCulture),
        object valor => valor.ToString() ?? "",
        _ => ""
    };

    public static string Texto(Campo campo, IRegistro registro) => campo.Propiedad.GetValue(registro) switch
    {
        int id when campo.Opciones is not null => campo.Opciones.Find(x => x.Id == id)?.Texto ?? $"#{id}",
        bool yes => yes ? "Sí" : "No",
        DateTimeOffset fecha => fecha.ToOffset(TimeSpan.FromHours(-4)).ToString("g"),
        Enum estado => Reglas.Nombre(estado),
        object dato => dato.ToString() ?? "",
        _ => "—"
    };

    private static object? Convertir(Campo campo, string valor)
    {
        if (valor == "" && campo.Opcional) return null;
        if (campo.Tipo == typeof(string)) return campo.Clave == "Codigo" && valor == "" ? null : valor;
        if (campo.Tipo == typeof(bool) && bool.TryParse(valor, out var check)) return check;
        if (campo.Tipo == typeof(int) && int.TryParse(valor, CultureInfo.InvariantCulture, out var id)) return id;
        if (campo.Tipo == typeof(decimal) && decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out var dinero)) return dinero;
        if (campo.Tipo == typeof(double) && double.TryParse(valor, CultureInfo.InvariantCulture, out var numero) && double.IsFinite(numero)) return numero;
        if (campo.Tipo == typeof(DateOnly) && DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha)) return fecha;
        if (campo.Tipo.IsEnum && Enum.TryParse(campo.Tipo, valor, out var opcion) && opcion is not null && Enum.IsDefined(campo.Tipo, opcion)) return opcion;
        throw new ReglaException($"Valor inválido para {campo.Titulo}.");
    }

    private static async Task Validar(ApplicationDbContext db, IRegistro entity)
    {
        switch (entity)
        {
            case Tour tour when tour.Minimo > tour.Maximo:
                throw new ReglaException("El mínimo de pasajeros supera el máximo.");
            case Tour tour when tour.Id > 0 && await db.Salidas.AnyAsync(x =>
                x.TourId == tour.Id && x.Cupos > tour.Maximo && !x.Privada && x.Inicio >= Reglas.Hoy):
                throw new ReglaException("El máximo es inferior a los cupos de una salida vigente.");
            case Destino d when d.Latitud.HasValue != d.Longitud.HasValue:
                throw new ReglaException("Ingresa ambas coordenadas.");
            case Salida salida:
                var tourSalida = await db.Tours.FindAsync(salida.TourId) ?? throw new ReglaException("Selecciona un tour.");
                if (salida.Privada || salida.Estado is EstadoSalida.Cancelada or EstadoSalida.Finalizada or EstadoSalida.FuerzaMayor)
                    throw new ReglaException("Esta salida se gestiona desde Operaciones.");
                if (salida.Fin < salida.Inicio || salida.Cupos < salida.Ocupados || salida.Cupos < tourSalida.Minimo || salida.Cupos > tourSalida.Maximo)
                    throw new ReglaException("Revisa las fechas, la capacidad del tour y los cupos ocupados.");
                if (salida.Id > 0 && await db.Reservas.AnyAsync(x => x.SalidaId == salida.Id))
                {
                    var anterior = await db.Salidas.AsNoTracking().SingleAsync(x => x.Id == salida.Id);
                    if (anterior.TourId != salida.TourId || anterior.Inicio != salida.Inicio || anterior.Fin != salida.Fin)
                        throw new ReglaException("La salida ya tiene reservas. Usa la reprogramación en Operaciones.");
                }
                break;
            case ItinerarioDia dia when (await db.Tours.FindAsync(dia.TourId))?.Dias < dia.Dia:
                throw new ReglaException("El día supera la duración del tour.");
            case Convenio c when c.Desde > c.Hasta || c.ProveedorId.HasValue == c.OrganizacionId.HasValue:
                throw new ReglaException("Selecciona una contraparte y revisa la vigencia.");
            case ConvenioTarifa t when t.TourId.HasValue == t.PaqueteId.HasValue:
                throw new ReglaException("Selecciona un tour o un paquete.");
            case Promocion p:
                if (p.Hasta < p.Desde || (p.Porcentaje && p.Valor > 100) ||
                    ((p.Alcance == AlcancePromocion.Convenio) != p.ConvenioId.HasValue) || (p.TourId.HasValue && p.PaqueteId.HasValue))
                    throw new ReglaException("Revisa la vigencia, el descuento y el alcance.");
                p.Codigo = p.Codigo?.ToUpperInvariant();
                break;
            case AsignacionOperativa a:
                var evento = await db.Salidas.FindAsync(a.SalidaId) ?? throw new ReglaException("Selecciona una salida.");
                if ((a.ProveedorId is null && a.GuiaId is null && a.VehiculoId is null) || a.Fecha < evento.Inicio || a.Fecha > evento.Fin)
                    throw new ReglaException("Selecciona un recurso y una fecha dentro de la salida.");
                break;
        }
    }

    private static async Task Persistir(ApplicationDbContext db)
    {
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { throw new ReglaException("El registro cambió. Recarga e inténtalo de nuevo."); }
        catch (DbUpdateException) { throw new ReglaException("Revisa los registros vinculados y los valores duplicados."); }
    }

    private static async Task<List<Opcion>> Opciones(ApplicationDbContext db, Type tipo)
        => (await Filas(db, tipo)).Select(x => new Opcion(x.Id, x switch
        {
            Vehiculo v => v.Placa,
            Salida s => $"#{s.Id} · {s.Inicio:yyyy-MM-dd}",
            _ => tipo.GetProperty("Nombre")?.GetValue(x)?.ToString() ?? $"#{x.Id}"
        })).OrderBy(x => x.Texto).ToList();

    private static Task<List<IRegistro>> Filas(ApplicationDbContext db, Type tipo) =>
        (Task<List<IRegistro>>)typeof(AdminService).GetMethod(nameof(Leer), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(tipo).Invoke(null, [db])!;

    private static async Task<List<IRegistro>> Leer<T>(ApplicationDbContext db) where T : class, IRegistro =>
        [.. await db.Set<T>().AsNoTracking().OrderByDescending(x => EF.Property<int>(x, "Id")).ToListAsync()];
}
