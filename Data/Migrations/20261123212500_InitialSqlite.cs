using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agencia.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Nacionalidad = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Documento = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Idioma = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Preferencias = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Nit = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    RazonSocial = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Frecuente = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contenido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    TituloEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Cuerpo = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    CuerpoEn = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Publicado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contenido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Clave = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Extras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    PrecioBob = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    PrecioUsd = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Idiomas = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Especialidades = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Disponible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MensajesContacto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    Fecha = table.Column<long>(type: "INTEGER", nullable: false),
                    Atendido = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesContacto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Representante = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    DescripcionEn = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    PrecioBob = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    PrecioUsd = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Publicado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquetes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Ciudad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Contacto = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regiones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    DescripcionEn = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    Incluye = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IncluyeEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NoIncluye = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    NoIncluyeEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Requisitos = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    RequisitosEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Cancelacion = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CancelacionEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Dificultad = table.Column<int>(type: "INTEGER", nullable: false),
                    Dias = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioBob = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    PrecioUsd = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Minimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Maximo = table.Column<int>(type: "INTEGER", nullable: false),
                    Publicado = table.Column<bool>(type: "INTEGER", nullable: false),
                    Destacado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClienteBeneficio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Vence = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Utilizado = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteBeneficio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteBeneficio_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartamentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Cargo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Contacto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empleados_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganizacionId = table.Column<int>(type: "INTEGER", nullable: true),
                    NombreOrganizacion = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    FechaTentativa = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Personas = table.Column<int>(type: "INTEGER", nullable: false),
                    Destinos = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    Presupuesto = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: true),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitudes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solicitudes_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Convenios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProveedorId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganizacionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Condiciones = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    Desde = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Hasta = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convenios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Convenios_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Convenios_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServicioProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProveedorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Tarifa = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicioProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicioProveedor_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Placa = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Capacidad = table.Column<int>(type: "INTEGER", nullable: false),
                    ProveedorId = table.Column<int>(type: "INTEGER", nullable: true),
                    Disponible = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehiculo_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Destinos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    DescripcionEn = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    MejorEpoca = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    MejorEpocaEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Recomendaciones = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    RecomendacionesEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Altitud = table.Column<int>(type: "INTEGER", nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: true),
                    Longitud = table.Column<double>(type: "REAL", nullable: true),
                    Imagen = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinos_Regiones_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Regiones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItinerarioDia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false),
                    Dia = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    TituloEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Actividades = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ActividadesEn = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Alojamiento = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    AlojamientoEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Comidas = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ComidasEn = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItinerarioDia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItinerarioDia_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaqueteTour",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaqueteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaqueteTour", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaqueteTour_Paquetes_PaqueteId",
                        column: x => x.PaqueteId,
                        principalTable: "Paquetes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaqueteTour_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TourFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    DescripcionEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourFoto_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    EmpleadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganizacionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    Idioma = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<long>(type: "INTEGER", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SolicitudCotizacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    ValidaHasta = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizacion_Solicitudes_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConvenioTarifa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConvenioId = table.Column<int>(type: "INTEGER", nullable: false),
                    TourId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaqueteId = table.Column<int>(type: "INTEGER", nullable: true),
                    DescuentoPorcentaje = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Condiciones = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvenioTarifa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConvenioTarifa_Convenios_ConvenioId",
                        column: x => x.ConvenioId,
                        principalTable: "Convenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvenioTarifa_Paquetes_PaqueteId",
                        column: x => x.PaqueteId,
                        principalTable: "Paquetes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConvenioTarifa_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Promociones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    NombreEn = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true),
                    Alcance = table.Column<int>(type: "INTEGER", nullable: false),
                    ConvenioId = table.Column<int>(type: "INTEGER", nullable: true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: true),
                    PaqueteId = table.Column<int>(type: "INTEGER", nullable: true),
                    Desde = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Hasta = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Porcentaje = table.Column<bool>(type: "INTEGER", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promociones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promociones_Convenios_ConvenioId",
                        column: x => x.ConvenioId,
                        principalTable: "Convenios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Promociones_Paquetes_PaqueteId",
                        column: x => x.PaqueteId,
                        principalTable: "Paquetes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Promociones_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Salidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false),
                    Inicio = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Fin = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Cupos = table.Column<int>(type: "INTEGER", nullable: false),
                    Ocupados = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioBob = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: true),
                    PrecioUsd = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: true),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    GuiaId = table.Column<int>(type: "INTEGER", nullable: true),
                    VehiculoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Privada = table.Column<bool>(type: "INTEGER", nullable: false),
                    Checklist = table.Column<string>(type: "TEXT", maxLength: 2500, nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salidas", x => x.Id);
                    table.CheckConstraint("CK_Salida_Cupos", "[Cupos] > 0 AND [Ocupados] >= 0 AND [Ocupados] <= [Cupos] AND [Fin] >= [Inicio]");
                    table.ForeignKey(
                        name: "FK_Salidas_Guia_GuiaId",
                        column: x => x.GuiaId,
                        principalTable: "Guia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Salidas_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Salidas_Vehiculo_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TourDestino",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TourId = table.Column<int>(type: "INTEGER", nullable: false),
                    DestinoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourDestino", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourDestino_Destinos_DestinoId",
                        column: x => x.DestinoId,
                        principalTable: "Destinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TourDestino_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserPasskeys",
                columns: table => new
                {
                    CredentialId = table.Column<byte[]>(type: "BLOB", maxLength: 1024, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Data = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserPasskeys", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_AspNetUserPasskeys_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionOperativa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SalidaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProveedorId = table.Column<int>(type: "INTEGER", nullable: true),
                    GuiaId = table.Column<int>(type: "INTEGER", nullable: true),
                    VehiculoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionOperativa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionOperativa_Guia_GuiaId",
                        column: x => x.GuiaId,
                        principalTable: "Guia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionOperativa_Proveedor_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionOperativa_Salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "Salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsignacionOperativa_Vehiculo_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganizacionId = table.Column<int>(type: "INTEGER", nullable: true),
                    SalidaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Descuento = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: false),
                    Solicitada = table.Column<long>(type: "INTEGER", nullable: false),
                    SolicitudId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.CheckConstraint("CK_Reserva_Propietario", "([ClienteId] IS NULL AND [OrganizacionId] IS NOT NULL) OR ([ClienteId] IS NOT NULL AND [OrganizacionId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_Reservas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "Salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Viajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SolicitudCotizacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SalidaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Creado = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Viajes_Salidas_SalidaId",
                        column: x => x.SalidaId,
                        principalTable: "Salidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Viajes_Solicitudes_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false),
                    Moneda = table.Column<int>(type: "INTEGER", nullable: false),
                    Metodo = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Comprobante = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Fecha = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.CheckConstraint("CK_Pago_Monto", "[Monto] > 0");
                    table.ForeignKey(
                        name: "FK_Pagos_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pasajero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Documento = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Nacionalidad = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Nacimiento = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Requerimientos = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pasajero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pasajero_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resena",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Puntuacion = table.Column<int>(type: "INTEGER", nullable: false),
                    Comentario = table.Column<string>(type: "TEXT", maxLength: 1500, nullable: false),
                    Publicada = table.Column<bool>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resena", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resena_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservaServicioAdicional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicioAdicionalId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 140, nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 14, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservaServicioAdicional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservaServicioAdicional_Extras_ServicioAdicionalId",
                        column: x => x.ServicioAdicionalId,
                        principalTable: "Extras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservaServicioAdicional_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantePago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 180, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Datos = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantePago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprobantePago_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransaccionPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PagoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<long>(type: "INTEGER", nullable: false),
                    Evento = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    IdentificadorExterno = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransaccionPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransaccionPago_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionOperativa_GuiaId",
                table: "AsignacionOperativa",
                column: "GuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionOperativa_ProveedorId",
                table: "AsignacionOperativa",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionOperativa_SalidaId",
                table: "AsignacionOperativa",
                column: "SalidaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionOperativa_VehiculoId",
                table: "AsignacionOperativa",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserPasskeys_UserId",
                table: "AspNetUserPasskeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClienteId",
                table: "AspNetUsers",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EmpleadoId",
                table: "AspNetUsers",
                column: "EmpleadoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizacionId",
                table: "AspNetUsers",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClienteBeneficio_ClienteId",
                table: "ClienteBeneficio",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantePago_PagoId",
                table: "ComprobantePago",
                column: "PagoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contenido_Slug",
                table: "Contenido",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Convenios_OrganizacionId",
                table: "Convenios",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Convenios_ProveedorId",
                table: "Convenios",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioTarifa_ConvenioId",
                table: "ConvenioTarifa",
                column: "ConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioTarifa_PaqueteId",
                table: "ConvenioTarifa",
                column: "PaqueteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConvenioTarifa_TourId",
                table: "ConvenioTarifa",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizacion_SolicitudCotizacionId",
                table: "Cotizacion",
                column: "SolicitudCotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Clave",
                table: "Departamentos",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Destinos_RegionId_Nombre",
                table: "Destinos",
                columns: new[] { "RegionId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItinerarioDia_TourId_Dia",
                table: "ItinerarioDia",
                columns: new[] { "TourId", "Dia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_ReservaId_Referencia",
                table: "Pagos",
                columns: new[] { "ReservaId", "Referencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaqueteTour_PaqueteId_TourId",
                table: "PaqueteTour",
                columns: new[] { "PaqueteId", "TourId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaqueteTour_TourId",
                table: "PaqueteTour",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Pasajero_ReservaId",
                table: "Pasajero",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_Codigo",
                table: "Promociones",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_ConvenioId",
                table: "Promociones",
                column: "ConvenioId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_PaqueteId",
                table: "Promociones",
                column: "PaqueteId");

            migrationBuilder.CreateIndex(
                name: "IX_Promociones_TourId",
                table: "Promociones",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Regiones_Nombre",
                table: "Regiones",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resena_ReservaId",
                table: "Resena",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_OrganizacionId",
                table: "Reservas",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_SalidaId_Estado",
                table: "Reservas",
                columns: new[] { "SalidaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_SolicitudId",
                table: "Reservas",
                column: "SolicitudId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicioAdicional_ReservaId_ServicioAdicionalId",
                table: "ReservaServicioAdicional",
                columns: new[] { "ReservaId", "ServicioAdicionalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservaServicioAdicional_ServicioAdicionalId",
                table: "ReservaServicioAdicional",
                column: "ServicioAdicionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Salidas_GuiaId",
                table: "Salidas",
                column: "GuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Salidas_TourId_Inicio_Estado",
                table: "Salidas",
                columns: new[] { "TourId", "Inicio", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Salidas_VehiculoId",
                table: "Salidas",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicioProveedor_ProveedorId",
                table: "ServicioProveedor",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_OrganizacionId",
                table: "Solicitudes",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_TourDestino_DestinoId",
                table: "TourDestino",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TourDestino_TourId_DestinoId",
                table: "TourDestino",
                columns: new[] { "TourId", "DestinoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TourFoto_TourId",
                table: "TourFoto",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_Publicado_Tipo_Dias",
                table: "Tours",
                columns: new[] { "Publicado", "Tipo", "Dias" });

            migrationBuilder.CreateIndex(
                name: "IX_TransaccionPago_PagoId",
                table: "TransaccionPago",
                column: "PagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_Placa",
                table: "Vehiculo",
                column: "Placa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_ProveedorId",
                table: "Vehiculo",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_SalidaId",
                table: "Viajes",
                column: "SalidaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Viajes_SolicitudCotizacionId",
                table: "Viajes",
                column: "SolicitudCotizacionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionOperativa");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserPasskeys");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ClienteBeneficio");

            migrationBuilder.DropTable(
                name: "ComprobantePago");

            migrationBuilder.DropTable(
                name: "Contenido");

            migrationBuilder.DropTable(
                name: "ConvenioTarifa");

            migrationBuilder.DropTable(
                name: "Cotizacion");

            migrationBuilder.DropTable(
                name: "ItinerarioDia");

            migrationBuilder.DropTable(
                name: "MensajesContacto");

            migrationBuilder.DropTable(
                name: "PaqueteTour");

            migrationBuilder.DropTable(
                name: "Pasajero");

            migrationBuilder.DropTable(
                name: "Promociones");

            migrationBuilder.DropTable(
                name: "Resena");

            migrationBuilder.DropTable(
                name: "ReservaServicioAdicional");

            migrationBuilder.DropTable(
                name: "ServicioProveedor");

            migrationBuilder.DropTable(
                name: "TourDestino");

            migrationBuilder.DropTable(
                name: "TourFoto");

            migrationBuilder.DropTable(
                name: "TransaccionPago");

            migrationBuilder.DropTable(
                name: "Viajes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Convenios");

            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "Extras");

            migrationBuilder.DropTable(
                name: "Destinos");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Solicitudes");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Regiones");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Organizaciones");

            migrationBuilder.DropTable(
                name: "Salidas");

            migrationBuilder.DropTable(
                name: "Guia");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Vehiculo");

            migrationBuilder.DropTable(
                name: "Proveedor");
        }
    }
}
