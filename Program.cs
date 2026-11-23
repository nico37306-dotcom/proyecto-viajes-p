using Agencia.Web.Authorization;
using Agencia.Web.Components;
using Agencia.Web.Components.Account;
using Agencia.Web.Data;
using Agencia.Web.Domain;
using Agencia.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
if (int.TryParse(builder.Configuration["PORT"], out var port) && port is > 0 and <= 65535)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
var keys = builder.Services.AddDataProtection().SetApplicationName("TerraAndina");
var keyPath = builder.Configuration["Agency:DataProtectionKeysPath"];
if (!string.IsNullOrWhiteSpace(keyPath))
{
    Directory.CreateDirectory(keyPath);
    keys.PersistKeysToFileSystem(new DirectoryInfo(keyPath));
}
if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Agency:EphemeralKeys"))
    keys.UseEphemeralDataProtectionProvider();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermisoHandler>();
builder.Services.AddAuthorization(Acceso.Configurar);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
    options.SetDefaultCulture("es").AddSupportedCultures("es", "en").AddSupportedUICultures("es", "en"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
builder.Services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Account/AccessDenied");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Configura ConnectionStrings:DefaultConnection.")));
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    options.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager<AgencySignInManager>().AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<ITourService>(s => s.GetRequiredService<CatalogService>());
builder.Services.AddScoped<IDestinationService>(s => s.GetRequiredService<CatalogService>());
builder.Services.AddScoped<IDepartureService>(s => s.GetRequiredService<CatalogService>());
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentGateway, ManualPaymentGateway>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<IOrganizationService>(s => s.GetRequiredService<OrganizationService>());
builder.Services.AddScoped<IConventionService>(s => s.GetRequiredService<OrganizationService>());
builder.Services.AddScoped<IQuotationService>(s => s.GetRequiredService<OrganizationService>());
builder.Services.AddScoped<IGroupTripService>(s => s.GetRequiredService<OrganizationService>());
builder.Services.AddScoped<IPromotionService>(s => s.GetRequiredService<CatalogService>());
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IClientService, ProfileService>();
builder.Services.AddScoped<IOperationsService, OperationsService>();
builder.Services.AddScoped<Preferences>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapGet("/preferencias", (HttpContext http, string? idioma, Moneda? moneda, string? volver) =>
{
    var options = new CookieOptions { IsEssential = true, HttpOnly = true, Secure = http.Request.IsHttps, SameSite = SameSiteMode.Lax, MaxAge = TimeSpan.FromDays(365) };
    if (idioma is "es" or "en")
        http.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(idioma)), options);
    if (moneda.HasValue && Enum.IsDefined(moneda.Value))
        http.Response.Cookies.Append(Preferences.CurrencyCookie, moneda.Value.ToString(), options);
    return Results.LocalRedirect(Preferences.LocalPath(volver));
});
app.MapGet("/reservas/{id:int}/comprobantes/{pago:int}", async (int id, int pago, HttpContext http,
    IDbContextFactory<ApplicationDbContext> factory) =>
{
    var actor = await CurrentUserService.Leer(factory, http.User);
    if (actor is null || (!actor.Tiene(Acceso.Contabilidad) && !actor.Tiene(Acceso.Viajero))) return Results.NotFound();
    await using var db = await factory.CreateDbContextAsync();
    var query = actor.Tiene(Acceso.Contabilidad) ? db.Reservas : BookingService.Propias(db.Reservas, actor);
    if (!await query.AnyAsync(x => x.Id == id && x.Pagos.Any(p => p.Id == pago))) return Results.NotFound();
    var recibo = await db.Set<ComprobantePago>().AsNoTracking().SingleOrDefaultAsync(x => x.PagoId == pago);
    if (recibo is null) return Results.NotFound();
    http.Response.Headers.CacheControl = "private, no-store";
    http.Response.Headers.XContentTypeOptions = "nosniff";
    return Results.File(recibo.Datos, recibo.Tipo, recibo.Nombre);
}).RequireAuthorization();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

if (app.Configuration.GetValue<bool>("Agency:InitializeDatabase"))
    await Seed.Initialize(app.Services, app.Configuration);

app.Run();
