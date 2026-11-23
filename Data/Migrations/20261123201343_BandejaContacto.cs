using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agencia.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class BandejaContacto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MensajesContacto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    Fecha = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Atendido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesContacto", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MensajesContacto");
        }
    }
}
