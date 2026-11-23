using Agencia.Web.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agencia.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Region> Regiones => Set<Region>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Salida> Salidas => Set<Salida>();
    public DbSet<Paquete> Paquetes => Set<Paquete>();
    public DbSet<MensajeContacto> MensajesContacto => Set<MensajeContacto>();
    public DbSet<PaginaContenido> Contenido => Set<PaginaContenido>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ServicioAdicional> Extras => Set<ServicioAdicional>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Promocion> Promociones => Set<Promocion>();
    public DbSet<Convenio> Convenios => Set<Convenio>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Empleado> Empleados => Set<Empleado>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Organizacion> Organizaciones => Set<Organizacion>();
    public DbSet<SolicitudCotizacion> Solicitudes => Set<SolicitudCotizacion>();
    public DbSet<ViajeGrupal> Viajes => Set<ViajeGrupal>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<decimal>().HavePrecision(14, 2);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.Entity<ApplicationUser>().HasOne(x => x.Cliente).WithOne().HasForeignKey<ApplicationUser>(x => x.ClienteId);
        model.Entity<ApplicationUser>().HasOne(x => x.Empleado).WithOne().HasForeignKey<ApplicationUser>(x => x.EmpleadoId);
        model.Entity<ApplicationUser>().Property(x => x.Idioma).HasMaxLength(10);
        model.Entity<Departamento>().HasIndex(x => x.Clave).IsUnique();
        model.Entity<Region>().HasIndex(x => x.Nombre).IsUnique();
        model.Entity<Destino>().HasIndex(x => new { x.RegionId, x.Nombre }).IsUnique();
        model.Entity<PaginaContenido>().HasIndex(x => x.Slug).IsUnique();
        model.Entity<Vehiculo>().HasIndex(x => x.Placa).IsUnique();
        model.Entity<Tour>().HasIndex(x => new { x.Publicado, x.Tipo, x.Dias });
        model.Entity<TourDestino>().HasIndex(x => new { x.TourId, x.DestinoId }).IsUnique();
        model.Entity<ItinerarioDia>().HasIndex(x => new { x.TourId, x.Dia }).IsUnique();
        model.Entity<PaqueteTour>().HasIndex(x => new { x.PaqueteId, x.TourId }).IsUnique();
        model.Entity<TourDestino>().HasOne<Tour>().WithMany(x => x.Destinos).HasForeignKey(x => x.TourId);
        model.Entity<ItinerarioDia>().HasOne<Tour>().WithMany(x => x.Itinerario).HasForeignKey(x => x.TourId);
        model.Entity<TourFoto>().HasOne<Tour>().WithMany(x => x.Fotos).HasForeignKey(x => x.TourId);
        model.Entity<PaqueteTour>().HasOne<Paquete>().WithMany(x => x.Tours).HasForeignKey(x => x.PaqueteId);
        model.Entity<Salida>().HasIndex(x => new { x.TourId, x.Inicio, x.Estado });
        model.Entity<Salida>().ToTable(t => t.HasCheckConstraint("CK_Salida_Cupos", "[Cupos] > 0 AND [Ocupados] >= 0 AND [Ocupados] <= [Cupos] AND [Fin] >= [Inicio]"));
        model.Entity<Reserva>().HasIndex(x => x.SolicitudId).IsUnique();
        model.Entity<Reserva>().HasIndex(x => new { x.SalidaId, x.Estado });
        model.Entity<Reserva>().ToTable(t => t.HasCheckConstraint("CK_Reserva_Propietario", "([ClienteId] IS NULL AND [OrganizacionId] IS NOT NULL) OR ([ClienteId] IS NOT NULL AND [OrganizacionId] IS NULL)"));
        model.Entity<ClienteBeneficio>().HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId);
        model.Entity<Resena>().HasOne<Reserva>().WithOne().HasForeignKey<Resena>(x => x.ReservaId);
        model.Entity<ServicioProveedor>().HasOne<Proveedor>().WithMany().HasForeignKey(x => x.ProveedorId);
        model.Entity<Vehiculo>().HasOne<Proveedor>().WithMany().HasForeignKey(x => x.ProveedorId);
        model.Entity<AsignacionOperativa>().HasOne<Salida>().WithMany().HasForeignKey(x => x.SalidaId);
        model.Entity<AsignacionOperativa>().HasOne<Proveedor>().WithMany().HasForeignKey(x => x.ProveedorId);
        model.Entity<AsignacionOperativa>().HasOne<Guia>().WithMany().HasForeignKey(x => x.GuiaId);
        model.Entity<AsignacionOperativa>().HasOne<Vehiculo>().WithMany().HasForeignKey(x => x.VehiculoId);
        model.Entity<ReservaServicioAdicional>().HasOne<ServicioAdicional>().WithMany().HasForeignKey(x => x.ServicioAdicionalId);
        model.Entity<ReservaServicioAdicional>().HasIndex(x => new { x.ReservaId, x.ServicioAdicionalId }).IsUnique();
        model.Entity<Convenio>().HasOne<Proveedor>().WithMany().HasForeignKey(x => x.ProveedorId);
        model.Entity<Convenio>().HasOne<Organizacion>().WithMany().HasForeignKey(x => x.OrganizacionId);
        model.Entity<ConvenioTarifa>().HasOne<Tour>().WithMany().HasForeignKey(x => x.TourId);
        model.Entity<ConvenioTarifa>().HasOne<Paquete>().WithMany().HasForeignKey(x => x.PaqueteId);
        model.Entity<Promocion>().HasOne<Convenio>().WithMany().HasForeignKey(x => x.ConvenioId);
        model.Entity<Promocion>().HasOne<Tour>().WithMany().HasForeignKey(x => x.TourId);
        model.Entity<Promocion>().HasOne<Paquete>().WithMany().HasForeignKey(x => x.PaqueteId);
        model.Entity<Promocion>().HasIndex(x => x.Codigo).IsUnique();
        model.Entity<SolicitudCotizacion>().HasOne<Organizacion>().WithMany().HasForeignKey(x => x.OrganizacionId);
        model.Entity<SolicitudCotizacion>().HasOne(x => x.Oferta).WithOne().HasForeignKey<Cotizacion>(x => x.SolicitudCotizacionId);
        model.Entity<SolicitudCotizacion>().HasOne(x => x.Viaje).WithOne().HasForeignKey<ViajeGrupal>(x => x.SolicitudCotizacionId);
        model.Entity<ViajeGrupal>().HasIndex(x => x.SalidaId).IsUnique();
        model.Entity<Pago>().HasOne(x => x.Archivo).WithOne().HasForeignKey<ComprobantePago>(x => x.PagoId);
        model.Entity<Pago>().HasIndex(x => new { x.ReservaId, x.Referencia }).IsUnique();
        model.Entity<Pago>().ToTable(t => t.HasCheckConstraint("CK_Pago_Monto", "[Monto] > 0"));

        foreach (var relation in model.Model.GetEntityTypes().Where(t => t.ClrType.Namespace == typeof(Tour).Namespace || t.ClrType == typeof(ApplicationUser)).SelectMany(t => t.GetForeignKeys()))
            relation.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
