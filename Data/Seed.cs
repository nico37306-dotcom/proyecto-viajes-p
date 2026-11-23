using Agencia.Web.Authorization;
using Agencia.Web.Domain;
using Agencia.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Data;

public static class Seed
{
    private const string DemoUserEmail = "viajero.demo@example.test";

    public const string Uyuni = "https://upload.wikimedia.org/wikipedia/commons/b/b5/Salar_de_Uyuni_lake.jpg";
    public const string Colorada = "https://upload.wikimedia.org/wikipedia/commons/3/38/Flamingos_Laguna_Colorada.jpg";
    public const string Titicaca = "https://upload.wikimedia.org/wikipedia/commons/a/a3/Terraced_fields_and_homes_on_Isla_Del_Sol%2C_Lake_Titicaca.jpg";
    public const string Hero = "https://upload.wikimedia.org/wikipedia/commons/8/84/Sunset_over_the_Salar_de_Uyuni.jpg";

    public static async Task Initialize(IServiceProvider services, IConfiguration config)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Acceso.Roles)
            if (!await roles.RoleExistsAsync(role)) AccountService.Verificar(await roles.CreateAsync(new(role)));

        var existentes = await db.Departamentos.Select(x => x.Clave).ToListAsync();
        db.AddRange(Acceso.Departamentos.Where(x => !existentes.Contains(x.Clave))
            .Select(x => new Departamento { Clave = x.Clave, Nombre = x.Nombre }));
        var regiones = new[] { Region("Altiplano", "Andean Highlands"), Region("Valles", "Valleys"), Region("Llanos y Oriente", "Eastern Lowlands"),
            Region("Amazonía", "Amazon"), Region("Chaco", "Chaco") };
        var regionesExistentes = await db.Regiones.Select(x => x.Nombre).ToListAsync();
        db.AddRange(regiones.Where(x => !regionesExistentes.Contains(x.Nombre, StringComparer.OrdinalIgnoreCase)));
        await db.SaveChangesAsync();
        if (config.GetValue<bool>("Agency:DemoCatalog") && !await db.Tours.AnyAsync()) await Demo(db);

        var email = config["Agency:AdminEmail"];
        var password = config["Agency:AdminPassword"];
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
        {
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var admin = await users.FindByEmailAsync(email);
            if (admin is null)
            {
                await using var tx = await db.Database.BeginTransactionAsync();
                admin = new() { UserName = email, Email = email, EmailConfirmed = true };
                AccountService.Verificar(await users.CreateAsync(admin, password));
                AccountService.Verificar(await users.AddToRoleAsync(admin, Acceso.Admin));
                await tx.CommitAsync();
            }
            else if (!await users.IsInRoleAsync(admin, Acceso.Admin))
                throw new InvalidOperationException("El correo de inicialización ya pertenece a una cuenta sin rol administrador.");
        }

        if (config.GetValue<bool>("Agency:DemoCatalog"))
            await SeedAuth(scope.ServiceProvider, config);
    }

    private static async Task SeedAuth(IServiceProvider services, IConfiguration config)
    {
        var users = services.GetRequiredService<UserManager<ApplicationUser>>();
        if (await users.FindByEmailAsync(DemoUserEmail) is not null) return;

        var demoUserPassword = config["Auth:DemoUserPassword"];
        if (string.IsNullOrWhiteSpace(demoUserPassword))
        {
            var log = services.GetRequiredService<ILoggerFactory>().CreateLogger("SeedAuth");
            log.LogWarning(
                "SeedAuth: no se sembraron los datos demo porque falta Auth:DemoUserPassword. " +
                "Definilo con la variable de entorno Auth__DemoUserPassword para crearlos.");
            return;
        }

        var db = services.GetRequiredService<ApplicationDbContext>();
        await using var tx = await db.Database.BeginTransactionAsync();
        var user = new ApplicationUser
        {
            UserName = DemoUserEmail,
            Email = DemoUserEmail,
            EmailConfirmed = true,
            Cliente = new Cliente { Nombre = "Viajero demo" }
        };
        AccountService.Verificar(await users.CreateAsync(user, demoUserPassword));
        AccountService.Verificar(await users.AddToRoleAsync(user, Acceso.Cliente));
        await tx.CommitAsync();
    }

    private static Region Region(string es, string en) => new() { Nombre = es, NombreEn = en };

    private static async Task Demo(ApplicationDbContext db)
    {
        var hoy = Reglas.Hoy;
        var altiplano = await db.Regiones.FirstAsync(r => r.Nombre == "Altiplano");
        var salar = new Destino { Region = altiplano, Nombre = "Salar de Uyuni", NombreEn = "Uyuni Salt Flats",
            Imagen = Uyuni, Altitud = 3656, Latitud = -20.13, Longitud = -67.49,
            Descripcion = "Una planicie de sal que refleja el cielo cuando la cubre una lámina de agua.",
            DescripcionEn = "A vast salt plain that reflects the sky when covered by a thin layer of water.",
            MejorEpoca = "Temporada seca para recorrer la costra; lluvias para buscar reflejos.",
            MejorEpocaEn = "Dry months for the salt crust; wet months for reflections.",
            Recomendaciones = "Lleva abrigo, agua y protección solar.", RecomendacionesEn = "Bring warm layers, water and sun protection." };
        var laguna = new Destino { Region = altiplano, Nombre = "Laguna Colorada", NombreEn = "Laguna Colorada",
            Imagen = Colorada, Altitud = 4278, Latitud = -22.20, Longitud = -67.78,
            Descripcion = "Lagunas altoandinas y flamencos en la Reserva Eduardo Avaroa.",
            DescripcionEn = "High-Andean lagoons and flamingos in the Eduardo Avaroa Reserve.",
            MejorEpoca = "Consulta el estado de caminos antes de partir.", MejorEpocaEn = "Check road conditions before departure.",
            Recomendaciones = "Prepárate para el frío y la altura.", RecomendacionesEn = "Prepare for cold and high altitude." };
        var lago = new Destino { Region = altiplano, Nombre = "Lago Titicaca", NombreEn = "Lake Titicaca",
            Imagen = Titicaca, Altitud = 3812, Latitud = -16.03, Longitud = -69.15,
            Descripcion = "Caminos de piedra, terrazas y miradores sobre el lago desde la Isla del Sol.",
            DescripcionEn = "Stone paths, terraces and lake viewpoints on Isla del Sol.",
            MejorEpoca = "Mayo a octubre para caminatas más secas.", MejorEpocaEn = "May to October for drier hikes.",
            Recomendaciones = "Usa calzado de senderismo.", RecomendacionesEn = "Wear hiking shoes." };
        var uyuni = Tour(salar, "Salar de Uyuni al atardecer", "Uyuni Salt Flats at Sunset",
            "Desde los salares de Colchani hasta el horizonte encendido de Uyuni.", "From Colchani's salt beds to Uyuni's glowing horizon.",
            TipoTour.Naturaleza, Dificultad.Facil, 1, 550, 80);
        uyuni.Fotos.Add(new() { Imagen = Hero, Descripcion = "Atardecer en el salar", DescripcionEn = "Sunset on the salt flats" });
        uyuni.Itinerario.Add(Dia(1, "Colchani y puesta del sol", "Colchani and sunset", "Visita el pueblo salinero, recorre el salar y espera el atardecer.", "Visit the salt village, cross the flats and stay for sunset."));
        var colorada = Tour(laguna, "Lagunas y desiertos del sur", "Southern Lagoons and Deserts",
            "Tres días entre salares, formaciones volcánicas y lagunas de colores.", "Three days among salt plains, volcanic formations and colorful lagoons.",
            TipoTour.Aventura, Dificultad.Moderada, 3, 1640, 240);
        colorada.Destinos.Add(new() { Destino = salar, Orden = 2 });
        colorada.Itinerario.AddRange([
            Dia(1, "Camino al salar", "Across the salt flats", "Salida desde Uyuni y visita a Colchani.", "Depart Uyuni and visit Colchani."),
            Dia(2, "Laguna Colorada", "Laguna Colorada", "Recorre el desierto de Siloli y observa flamencos.", "Cross the Siloli desert and look for flamingos."),
            Dia(3, "Paisajes volcánicos", "Volcanic landscapes", "Paradas panorámicas y retorno a Uyuni.", "Scenic stops and return to Uyuni.")
        ]);
        var titicaca = Tour(lago, "Titicaca e Isla del Sol", "Titicaca and Isla del Sol",
            "Cruza el lago desde Copacabana y camina entre terrazas y miradores.", "Cross from Copacabana and walk among terraces and viewpoints.",
            TipoTour.Cultural, Dificultad.Moderada, 2, 890, 130);
        titicaca.Itinerario.AddRange([
            Dia(1, "Copacabana", "Copacabana", "Traslado desde La Paz y llegada a la orilla del lago.", "Travel from La Paz to the lakeshore."),
            Dia(2, "Isla del Sol", "Isla del Sol", "Navegación y caminata con vistas al Titicaca.", "Boat crossing and a walk with views of Lake Titicaca.")
        ]);
        foreach (var tour in new[] { uyuni, colorada, titicaca })
        {
            foreach (var offset in new[] { 9, 24, 48, 76 })
                tour.Salidas.Add(new() { Inicio = hoy.AddDays(offset), Fin = hoy.AddDays(offset + tour.Dias - 1), Cupos = tour.Maximo });
            db.Add(tour);
        }
        db.Add(new Paquete { Nombre = "Circuito Altiplano", NombreEn = "Andean Highlands Circuit",
            Descripcion = "Une la Isla del Sol con las lagunas del sur.", DescripcionEn = "Combine Isla del Sol with the southern lagoons.",
            Imagen = Titicaca, PrecioBob = 2290, PrecioUsd = 335, Publicado = true, Tours = [new() { Tour = colorada }, new() { Tour = titicaca }] });
        db.Add(new ServicioAdicional { Nombre = "Traslado al punto de encuentro", NombreEn = "Transfer to meeting point", PrecioBob = 75, PrecioUsd = 11 });
        db.Add(new Promocion { Nombre = "Bienvenida al Altiplano", NombreEn = "Welcome to the Highlands", Codigo = "ALTIPLANO",
            Desde = hoy, Hasta = hoy.AddDays(90), Valor = 10, Alcance = AlcancePromocion.General });
        db.AddRange(
            Contenido("uyuni-en-cada-estacion", "Uyuni en cada estación", "Uyuni through the seasons",
                "En época seca se aprecia la textura de la sal. En temporada de lluvias puede aparecer el espejo de agua y cambiar el acceso a ciertos puntos. Consulta la ruta de la salida antes de viajar.",
                "The dry season reveals the texture of the salt. During the wet season, reflections may appear and access can change. Check the itinerary before booking."),
            Contenido("antes-de-viajar-al-altiplano", "Qué llevar al Altiplano", "Packing for the highlands",
                "Empaca capas de ropa, calzado cómodo, protección solar y una botella reutilizable. Revisa con el equipo las paradas y lo incluido en tu tour.",
                "Pack layers, comfortable shoes, sun protection and a reusable water bottle. Check stops and tour inclusions with the team."));
        await db.SaveChangesAsync();
    }

    private static PaginaContenido Contenido(string slug, string es, string en, string cuerpo, string cuerpoEn) =>
        new() { Slug = slug, Titulo = es, TituloEn = en, Cuerpo = cuerpo, CuerpoEn = cuerpoEn, Publicado = true };

    private static Tour Tour(Destino destino, string es, string en, string descripcion, string descripcionEn,
        TipoTour tipo, Dificultad dificultad, int dias, decimal bob, decimal usd) =>
        new()
        {
            Nombre = es, NombreEn = en, Descripcion = descripcion, DescripcionEn = descripcionEn, Imagen = destino.Imagen,
            Destinos = [new() { Destino = destino }], Tipo = tipo, Dificultad = dificultad, Dias = dias, PrecioBob = bob, PrecioUsd = usd,
            Publicado = true, Destacado = true, Incluye = "Transporte del itinerario y guía.", IncluyeEn = "Itinerary transport and guide.",
            NoIncluye = "Gastos personales y servicios adicionales.", NoIncluyeEn = "Personal expenses and add-ons.",
            Requisitos = "Documento de identidad y ropa adecuada para el clima.", RequisitosEn = "Identity document and weather-appropriate clothing."
        };

    private static ItinerarioDia Dia(int dia, string es, string en, string actividad, string actividadEn) =>
        new() { Dia = dia, Titulo = es, TituloEn = en, Actividades = actividad, ActividadesEn = actividadEn };
}
