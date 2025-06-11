using System;
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
            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Cedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Apellido1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Cedula);
                });

            migrationBuilder.CreateTable(
                name: "Organizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoOrganizacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rangos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArticulosRequeridos = table.Column<int>(type: "int", nullable: false),
                    AniosExperienciaRequeridos = table.Column<int>(type: "int", nullable: false),
                    HorasCursoRequeridas = table.Column<int>(type: "int", nullable: false),
                    MesesInvestigacionRequeridos = table.Column<int>(type: "int", nullable: false),
                    PuntajePromedioEvaluacionesRequerido = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rangos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    DOI = table.Column<string>(type: "varchar(200)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revista = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: false),
                    ArchivoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.DOI);
                    table.ForeignKey(
                        name: "FK_Articulos_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Correo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaveHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Correo);
                    table.ForeignKey(
                        name: "FK_Cuentas_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesDocentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodoAcademico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuntajePorcentual = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InformeRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesDocentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionesDocentes_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Investigaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RolEnInvestigacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MesesDeInvestigacion = table.Column<int>(type: "int", nullable: false),
                    InformeRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investigaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Investigaciones_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizacionId = table.Column<int>(type: "int", nullable: false),
                    NumeroHoras = table.Column<int>(type: "int", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificadoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cursos_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cursos_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienciasLaborales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizacionId = table.Column<int>(type: "int", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificadoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciasLaborales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienciasLaborales_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienciasLaborales_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAscenso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RangoActualId = table.Column<int>(type: "int", nullable: true),
                    RangoSolicitadoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObservacionesAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAscenso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Rangos_RangoActualId",
                        column: x => x.RangoActualId,
                        principalTable: "Rangos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Rangos_RangoSolicitadoId",
                        column: x => x.RangoSolicitadoId,
                        principalTable: "Rangos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ArticulosPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticuloDOI = table.Column<string>(type: "varchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticulosPorSolicitud", x => new { x.SolicitudId, x.ArticuloDOI });
                    table.ForeignKey(
                        name: "FK_ArticulosPorSolicitud_Articulos_ArticuloDOI",
                        column: x => x.ArticuloDOI,
                        principalTable: "Articulos",
                        principalColumn: "DOI",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticulosPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CursosPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursosPorSolicitud", x => new { x.SolicitudId, x.CursoId });
                    table.ForeignKey(
                        name: "FK_CursosPorSolicitud_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CursosPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesPorSolicitud", x => new { x.SolicitudId, x.EvaluacionId });
                    table.ForeignKey(
                        name: "FK_EvaluacionesPorSolicitud_EvaluacionesDocentes_EvaluacionId",
                        column: x => x.EvaluacionId,
                        principalTable: "EvaluacionesDocentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EvaluacionesPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienciaPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienciaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciaPorSolicitud", x => new { x.SolicitudId, x.ExperienciaId });
                    table.ForeignKey(
                        name: "FK_ExperienciaPorSolicitud_ExperienciasLaborales_ExperienciaId",
                        column: x => x.ExperienciaId,
                        principalTable: "ExperienciasLaborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExperienciaPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestigacionesPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestigacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestigacionesPorSolicitud", x => new { x.SolicitudId, x.InvestigacionId });
                    table.ForeignKey(
                        name: "FK_InvestigacionesPorSolicitud_Investigaciones_InvestigacionId",
                        column: x => x.InvestigacionId,
                        principalTable: "Investigaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestigacionesPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articulos_DocenteCedula",
                table: "Articulos",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_ArticulosPorSolicitud_ArticuloDOI",
                table: "ArticulosPorSolicitud",
                column: "ArticuloDOI");

            migrationBuilder.CreateIndex(
                name: "IX_Cuentas_DocenteCedula",
                table: "Cuentas",
                column: "DocenteCedula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_DocenteCedula",
                table: "Cursos",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_OrganizacionId",
                table: "Cursos",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_CursosPorSolicitud_CursoId",
                table: "CursosPorSolicitud",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesDocentes_DocenteCedula",
                table: "EvaluacionesDocentes",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPorSolicitud_EvaluacionId",
                table: "EvaluacionesPorSolicitud",
                column: "EvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciaPorSolicitud_ExperienciaId",
                table: "ExperienciaPorSolicitud",
                column: "ExperienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasLaborales_DocenteCedula",
                table: "ExperienciasLaborales",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasLaborales_OrganizacionId",
                table: "ExperienciasLaborales",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Investigaciones_DocenteCedula",
                table: "Investigaciones",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_InvestigacionesPorSolicitud_InvestigacionId",
                table: "InvestigacionesPorSolicitud",
                column: "InvestigacionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_DocenteCedula",
                table: "SolicitudesAscenso",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_RangoActualId",
                table: "SolicitudesAscenso",
                column: "RangoActualId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAscenso_RangoSolicitadoId",
                table: "SolicitudesAscenso",
                column: "RangoSolicitadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticulosPorSolicitud");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "CursosPorSolicitud");

            migrationBuilder.DropTable(
                name: "EvaluacionesPorSolicitud");

            migrationBuilder.DropTable(
                name: "ExperienciaPorSolicitud");

            migrationBuilder.DropTable(
                name: "InvestigacionesPorSolicitud");

            migrationBuilder.DropTable(
                name: "Articulos");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "EvaluacionesDocentes");

            migrationBuilder.DropTable(
                name: "ExperienciasLaborales");

            migrationBuilder.DropTable(
                name: "Investigaciones");

            migrationBuilder.DropTable(
                name: "SolicitudesAscenso");

            migrationBuilder.DropTable(
                name: "Organizaciones");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Rangos");
        }
    }
}
