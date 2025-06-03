using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIGAD.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRangosDataStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rangos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Rangos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Rangos",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { new Guid("c1a75764-3420-4e00-91c0-66917c0d3e6f"), "Profesor de Tiempo Completo en categoría Auxiliar.", "Profesor Auxiliar TC" },
                    { new Guid("d2b86889-81b2-4a3a-984e-127424d349af"), "Profesor de Tiempo Completo en categoría Asistente.", "Profesor Asistente TC" },
                    { new Guid("e3c97990-92c3-5b4b-a95f-238535e450b0"), "Profesor de Tiempo Completo en categoría Asociado.", "Profesor Asociado TC" },
                    { new Guid("f4d08aa1-a3d4-6c5c-ba60-349646f561c1"), "Profesor de Tiempo Completo en categoría Titular.", "Profesor Titular TC" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rangos",
                keyColumn: "Id",
                keyValue: new Guid("c1a75764-3420-4e00-91c0-66917c0d3e6f"));

            migrationBuilder.DeleteData(
                table: "Rangos",
                keyColumn: "Id",
                keyValue: new Guid("d2b86889-81b2-4a3a-984e-127424d349af"));

            migrationBuilder.DeleteData(
                table: "Rangos",
                keyColumn: "Id",
                keyValue: new Guid("e3c97990-92c3-5b4b-a95f-238535e450b0"));

            migrationBuilder.DeleteData(
                table: "Rangos",
                keyColumn: "Id",
                keyValue: new Guid("f4d08aa1-a3d4-6c5c-ba60-349646f561c1"));

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rangos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Rangos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }
    }
}
