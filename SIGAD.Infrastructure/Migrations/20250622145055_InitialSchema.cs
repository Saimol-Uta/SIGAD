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
                name: "Organizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoOrganizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArticulosRequeridos = table.Column<int>(type: "int", nullable: false),
                    AniosExperienciaRequeridos = table.Column<int>(type: "int", nullable: false),
                    HorasCursoRequeridas = table.Column<int>(type: "int", nullable: false),
                    MesesInvestigacionRequeridos = table.Column<int>(type: "int", nullable: false),
                    TesisDirigidasRequeridas = table.Column<int>(type: "int", nullable: false),
                    PuntajePromedioEvaluacionesRequerido = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rangos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Cedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Apellido1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RangoActualId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Cedula);
                    table.ForeignKey(
                        name: "FK_Docentes_Rangos_RangoActualId",
                        column: x => x.RangoActualId,
                        principalTable: "Rangos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccionesDePersonal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DocumentoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificadoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesDePersonal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccionesDePersonal_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Articulos",
                columns: table => new
                {
                    DOI = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Revista = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: false),
                    ArchivoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    UnidadVerificadora = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Verificado = table.Column<bool>(type: "bit", nullable: false),
                    FechaVerificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulos", x => x.DOI);
                    table.ForeignKey(
                        name: "FK_Articulos_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cuentas",
                columns: table => new
                {
                    Correo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClaveHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoRecuperacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CodigoExpiracion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuentas", x => x.Correo);
                    table.CheckConstraint("CK_Cuentas_Rol", "Rol IN ('ADMINISTRADOR', 'DOCENTE')");
                    table.ForeignKey(
                        name: "FK_Cuentas_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrganizacionId = table.Column<int>(type: "int", nullable: false),
                    NumeroHoras = table.Column<int>(type: "int", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificadoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoCurso = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImpartidoPorDocente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cursos_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cursos_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvaluacionesDocentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodoAcademico = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuntajePorcentual = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InformeRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluacionesDocentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvaluacionesDocentes_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExperienciasLaborales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizacionId = table.Column<int>(type: "int", nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CertificadoRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciasLaborales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperienciasLaborales_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExperienciasLaborales_Organizaciones_OrganizacionId",
                        column: x => x.OrganizacionId,
                        principalTable: "Organizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Investigaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RolEnInvestigacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MesesDeInvestigacion = table.Column<int>(type: "int", nullable: false),
                    InformeRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TipoProyecto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MesesDeParticipacion = table.Column<int>(type: "int", nullable: false),
                    UnidadVerificadora = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investigaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Investigaciones_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAscenso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RangoActualId = table.Column<int>(type: "int", nullable: true),
                    RangoSolicitadoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ObservacionesAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AceptacionODemanda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaResolucionApelacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAscenso", x => x.Id);
                    table.CheckConstraint("CK_SolicitudesAscenso_Estado", "Estado IN ('Borrador', 'Enviada', 'En Revision', 'Aprobada', 'Rechazada')");
                    table.ForeignKey(
                        name: "FK_SolicitudesAscenso_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
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
                name: "TesisDirigidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NivelAcademico = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TituloTesis = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificacionRuta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContenidoHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesisDirigidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TesisDirigidas_Docentes_DocenteCedula",
                        column: x => x.DocenteCedula,
                        principalTable: "Docentes",
                        principalColumn: "Cedula",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccionesDePersonalPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccionDePersonalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesDePersonalPorSolicitud", x => new { x.SolicitudId, x.AccionDePersonalId });
                    table.ForeignKey(
                        name: "FK_AccionesDePersonalPorSolicitud_AccionesDePersonal_AccionDePersonalId",
                        column: x => x.AccionDePersonalId,
                        principalTable: "AccionesDePersonal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccionesDePersonalPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticulosPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticuloDOI = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticulosPorSolicitud", x => new { x.SolicitudId, x.ArticuloDOI });
                    table.ForeignKey(
                        name: "FK_ArticulosPorSolicitud_Articulos_ArticuloDOI",
                        column: x => x.ArticuloDOI,
                        principalTable: "Articulos",
                        principalColumn: "DOI",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluacionesPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExperienciasPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienciaId = table.Column<int>(type: "int", nullable: false),
                    ExperienciaLaboralId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperienciasPorSolicitud", x => new { x.SolicitudId, x.ExperienciaId });
                    table.ForeignKey(
                        name: "FK_ExperienciasPorSolicitud_ExperienciasLaborales_ExperienciaId",
                        column: x => x.ExperienciaId,
                        principalTable: "ExperienciasLaborales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperienciasPorSolicitud_ExperienciasLaborales_ExperienciaLaboralId",
                        column: x => x.ExperienciaLaboralId,
                        principalTable: "ExperienciasLaborales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExperienciasPorSolicitud_SolicitudesAscenso_SolicitudId",
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestigacionesPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TesisPorSolicitud",
                columns: table => new
                {
                    SolicitudId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TesisDirigidaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesisPorSolicitud", x => new { x.SolicitudId, x.TesisDirigidaId });
                    table.ForeignKey(
                        name: "FK_TesisPorSolicitud_SolicitudesAscenso_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAscenso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TesisPorSolicitud_TesisDirigidas_TesisDirigidaId",
                        column: x => x.TesisDirigidaId,
                        principalTable: "TesisDirigidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionesDePersonal_DocenteCedula",
                table: "AccionesDePersonal",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_AccionesDePersonalPorSolicitud_AccionDePersonalId",
                table: "AccionesDePersonalPorSolicitud",
                column: "AccionDePersonalId");

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
                name: "IX_Docentes_RangoActualId",
                table: "Docentes",
                column: "RangoActualId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesDocentes_DocenteCedula",
                table: "EvaluacionesDocentes",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluacionesPorSolicitud_EvaluacionId",
                table: "EvaluacionesPorSolicitud",
                column: "EvaluacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasLaborales_DocenteCedula",
                table: "ExperienciasLaborales",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasLaborales_OrganizacionId",
                table: "ExperienciasLaborales",
                column: "OrganizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasPorSolicitud_ExperienciaId",
                table: "ExperienciasPorSolicitud",
                column: "ExperienciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperienciasPorSolicitud_ExperienciaLaboralId",
                table: "ExperienciasPorSolicitud",
                column: "ExperienciaLaboralId");

            migrationBuilder.CreateIndex(
                name: "IX_Investigaciones_DocenteCedula",
                table: "Investigaciones",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_InvestigacionesPorSolicitud_InvestigacionId",
                table: "InvestigacionesPorSolicitud",
                column: "InvestigacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Rangos_Nombre",
                table: "Rangos",
                column: "Nombre",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_TesisDirigidas_DocenteCedula",
                table: "TesisDirigidas",
                column: "DocenteCedula");

            migrationBuilder.CreateIndex(
                name: "IX_TesisPorSolicitud_TesisDirigidaId",
                table: "TesisPorSolicitud",
                column: "TesisDirigidaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionesDePersonalPorSolicitud");

            migrationBuilder.DropTable(
                name: "ArticulosPorSolicitud");

            migrationBuilder.DropTable(
                name: "Cuentas");

            migrationBuilder.DropTable(
                name: "CursosPorSolicitud");

            migrationBuilder.DropTable(
                name: "EvaluacionesPorSolicitud");

            migrationBuilder.DropTable(
                name: "ExperienciasPorSolicitud");

            migrationBuilder.DropTable(
                name: "InvestigacionesPorSolicitud");

            migrationBuilder.DropTable(
                name: "TesisPorSolicitud");

            migrationBuilder.DropTable(
                name: "AccionesDePersonal");

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
                name: "TesisDirigidas");

            migrationBuilder.DropTable(
                name: "Organizaciones");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Rangos");
        }
    }
}
