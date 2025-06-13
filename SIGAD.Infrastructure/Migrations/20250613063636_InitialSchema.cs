using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIGAD.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RangoActualId",
                table: "Docentes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasPorSolicitud_ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud",
                column: "ExperienciaLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_RangoActualId",
                table: "Docentes",
                column: "RangoActualId");

            migrationBuilder.AddForeignKey(
                name: "FK_Docentes_Rangos_RangoActualId",
                table: "Docentes",
                column: "RangoActualId",
                principalTable: "Rangos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperienciasPorSolicitud_ExperienciasLaborales_ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud",
                column: "ExperienciaLaboralId",
                principalTable: "ExperienciasLaborales",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Docentes_Rangos_RangoActualId",
                table: "Docentes");

            migrationBuilder.DropForeignKey(
                name: "FK_ExperienciasPorSolicitud_ExperienciasLaborales_ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud");

            migrationBuilder.DropIndex(
                name: "IX_ExperienciasPorSolicitud_ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud");

            migrationBuilder.DropIndex(
                name: "IX_Docentes_RangoActualId",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud");

            migrationBuilder.DropColumn(
                name: "RangoActualId",
                table: "Docentes");
        }
    }
}
