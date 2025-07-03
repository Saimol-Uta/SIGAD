using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Application.Interfaces;

namespace SIGAD.Infrastructure.Persistence
{
    public class SigadDbContext : DbContext, IApplicationDbContext
    {
        public SigadDbContext(DbContextOptions<SigadDbContext> options) : base(options) { }

        // DbSets para las entidades
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<EvaluacionDocente> EvaluacionesDocentes { get; set; }
        public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
        public DbSet<EvaluacionesPorSolicitud> EvaluacionesPorSolicitud { get; set; }
        public DbSet<Rango> Rangos { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<ExperienciaLaboral> ExperienciasLaborales { get; set; }
        public DbSet<Investigacion> Investigaciones { get; set; }
        public DbSet<Organizacion> Organizaciones { get; set; }
        public DbSet<ArticulosPorSolicitud> ArticulosPorSolicitud { get; set; }
        public DbSet<CursosPorSolicitud> CursosPorSolicitud { get; set; }
        public DbSet<ExperienciaPorSolicitud> ExperienciasPorSolicitud { get; set; }
        public DbSet<InvestigacionesPorSolicitud> InvestigacionesPorSolicitud { get; set; }
        public DbSet<TesisDirigida> TesisDirigidas { get; set; } = default!;
        public DbSet<TesisPorSolicitud> TesisPorSolicitud { get; set; } = default!;
        public DbSet<AccionesDePersonal> AccionesDePersonal { get; set; } = default!;
        public DbSet<AccionesDePersonalPorSolicitud> AccionesDePersonalPorSolicitud { get; set; } = default!;
        public DbSet<Apelacion> Apelaciones { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Docente
            modelBuilder.Entity<Docente>(entity =>
            {
                entity.HasKey(e => e.Cedula);
                entity.Property(e => e.Cedula).HasMaxLength(10);
                entity.Property(e => e.Nombre1).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Nombre2).HasMaxLength(50);
                entity.Property(e => e.Apellido1).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Apellido2).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RangoActualId); // ⚠️ AGREGAR ESTA LÍNEA

                // Relación con RangoActual
                entity.HasOne(e => e.RangoActual)
                    .WithMany()
                    .HasForeignKey(e => e.RangoActualId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Cuenta
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Correo);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.ClaveHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Rol).HasConversion<string>().IsRequired();
                entity.Property(e => e.CodigoRecuperacion).HasMaxLength(10);
                entity.Property(e => e.CodigoExpiracion);

                entity.HasIndex(e => e.DocenteCedula).IsUnique();

                entity.HasOne(e => e.Docente)
                    .WithOne(d => d.Cuenta)
                    .HasForeignKey<Cuenta>(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasCheckConstraint("CK_Cuentas_Rol", "Rol IN ('ADMINISTRADOR', 'DOCENTE')");
            });

            // Configuración de la entidad EvaluacionDocente
            modelBuilder.Entity<EvaluacionDocente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.PeriodoAcademico).HasMaxLength(50).IsRequired();
                entity.Property(e => e.FechaEvaluacion).IsRequired();
                entity.Property(e => e.PuntajePorcentual).HasColumnType("decimal(5,2)").IsRequired();
                entity.Property(e => e.InformeRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.Evaluaciones)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });            // Configuración de la entidad SolicitudAscenso
            modelBuilder.Entity<SolicitudAscenso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Estado)
                     .HasConversion<string>() // Esto convierte el enum a string en la base de datos
                        .HasMaxLength(20)
                        .IsRequired();
                entity.Property(e => e.AceptacionODemanda).HasMaxLength(50);

                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.Solicitudes)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con RangoActual (opcional)
                entity.HasOne(e => e.RangoActual)
                    .WithMany(r => r.SolicitudesComoRangoActual)
                    .HasForeignKey(e => e.RangoActualId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con RangoSolicitado (requerido)
                entity.HasOne(e => e.RangoSolicitado)
                    .WithMany(r => r.SolicitudesComoRangoSolicitado)
                    .HasForeignKey(e => e.RangoSolicitadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasCheckConstraint("CK_SolicitudesAscenso_Estado",
                    "Estado IN ('Borrador', 'Enviada', 'En Revision', 'Aprobada', 'Rechazada')");
            });

            // Configuración de la tabla intermedia EvaluacionesPorSolicitud
            modelBuilder.Entity<EvaluacionesPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.EvaluacionId });

                entity.HasOne(e => e.Solicitud)
                    .WithMany(s => s.EvaluacionesPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Evaluacion)
                    .WithMany(ev => ev.EvaluacionesPorSolicitud)
                    .HasForeignKey(e => e.EvaluacionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad Rango
            modelBuilder.Entity<Rango>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ArticulosRequeridos).IsRequired();
                entity.Property(e => e.AniosExperienciaRequeridos).IsRequired();
                entity.Property(e => e.HorasCursoRequeridas).IsRequired();
                entity.Property(e => e.MesesInvestigacionRequeridos).IsRequired();
                entity.Property(e => e.TesisDirigidasRequeridas).IsRequired(); // ⚠️ ESTA LÍNEA FALTA
                entity.Property(e => e.PuntajePromedioEvaluacionesRequerido).HasColumnType("decimal(5,2)").IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();
            });            // Configuración de la entidad Articulo
            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.HasKey(e => e.DOI);
                entity.Property(e => e.DOI).HasMaxLength(100);
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Revista).HasMaxLength(100).IsRequired();
                entity.Property(e => e.AnioPublicacion).IsRequired();
                entity.Property(e => e.ArchivoRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.UnidadVerificadora).HasMaxLength(100);
                entity.Property(e => e.EsVerificado);
                entity.Property(e => e.FechaVerificacion);
                entity.Property(e => e.ObservacionesVerificacion).HasMaxLength(500);
                entity.Property(e => e.EsIndexado);
                entity.Property(e => e.FechaCreacion);
                // Nuevo campo para migración
                entity.Property(e => e.IdiomaPublicacion).HasMaxLength(50).IsRequired(false);
                // Propiedad de compatibilidad (no mapear directamente)
                entity.Ignore(e => e.Verificado);
                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.Articulos)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Organizacion
            modelBuilder.Entity<Organizacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TipoOrganizacion).HasMaxLength(50).IsRequired();
            });            // Configuración de la entidad Curso
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NumeroHoras).IsRequired();
                entity.Property(e => e.FechaFinalizacion).IsRequired();
                entity.Property(e => e.CertificadoRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired(); entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.TipoCurso).HasConversion<int>();
                entity.Property(e => e.ImpartidoPorDocente);
                // Nuevo campo para migración
                entity.Property(e => e.HorasImpartidas).IsRequired(false);
                entity.HasOne(e => e.Organizacion)
                    .WithMany(o => o.Cursos)
                    .HasForeignKey(e => e.OrganizacionId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.Cursos)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad ExperienciaLaboral
            modelBuilder.Entity<ExperienciaLaboral>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Cargo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin);
                entity.Property(e => e.CertificadoRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();

                entity.HasOne(e => e.Organizacion)
                    .WithMany(o => o.ExperienciasLaborales)
                    .HasForeignKey(e => e.OrganizacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.ExperienciasLaborales)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });            // Configuración de la entidad Investigacion
            modelBuilder.Entity<Investigacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFinalizacion);
                entity.Property(e => e.RolEnInvestigacion).HasMaxLength(50).IsRequired();
                entity.Property(e => e.MesesDeInvestigacion).IsRequired();
                entity.Property(e => e.InformeRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired(); entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.TipoProyecto).HasConversion<int>();
                entity.Property(e => e.MesesDeParticipacion);
                entity.Property(e => e.UnidadVerificadora).HasMaxLength(100);
                // Nuevo campo para migración
                entity.Property(e => e.EsInternacional).HasDefaultValue(false);
                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.Investigaciones)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de las tablas intermedias
            modelBuilder.Entity<ArticulosPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.ArticuloDOI });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.ArticulosPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Articulo)
                    .WithMany(a => a.ArticulosPorSolicitud)
                    .HasForeignKey(e => e.ArticuloDOI)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CursosPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.CursoId });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.CursosPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Curso)
                    .WithMany(c => c.CursosPorSolicitud)
                    .HasForeignKey(e => e.CursoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExperienciaPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.ExperienciaId });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.ExperienciaPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ExperienciaLaboral)
                    .WithMany(e => e.ExperienciasPorSolicitud)
                    .HasForeignKey(e => e.ExperienciaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvestigacionesPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.InvestigacionId });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.InvestigacionesPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Investigacion)
                    .WithMany(i => i.InvestigacionesPorSolicitud)
                    .HasForeignKey(e => e.InvestigacionId)
                    .OnDelete(DeleteBehavior.Cascade);
            }); modelBuilder.Entity<TesisPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.TesisId });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.TesisPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.TesisDirigida)
                    .WithMany(t => t.TesisPorSolicitud)
                    .HasForeignKey(e => e.TesisId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de la entidad AccionesDePersonal
            modelBuilder.Entity<AccionesDePersonal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Cargo).HasMaxLength(100).IsRequired();
                entity.Property(e => e.TipoCargo).HasConversion<int>();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin);
                entity.Property(e => e.DocumentoRuta);
                entity.Property(e => e.CertificadoRuta);
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.AccionesDePersonal)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la tabla intermedia AccionesDePersonalPorSolicitud
            modelBuilder.Entity<AccionesDePersonalPorSolicitud>(entity =>
            {
                entity.HasKey(e => new { e.SolicitudId, e.AccionDePersonalId });

                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.AccionesDePersonalPorSolicitud)
                    .HasForeignKey(e => e.SolicitudId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.AccionDePersonal)
                    .WithMany(a => a.AccionesDePersonalPorSolicitud)
                    .HasForeignKey(e => e.AccionDePersonalId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Agregar esta configuración en OnModelCreating
            modelBuilder.Entity<TesisDirigida>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd(); entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.NivelAcademico).HasConversion<int>();
                entity.Property(e => e.TituloTesis).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Estado).HasConversion<int>();
                entity.Property(e => e.FechaInicio).IsRequired();
                entity.Property(e => e.FechaFin);
                entity.Property(e => e.Institucion).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CertificacionRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();

                entity.HasOne(e => e.Docente)
                    .WithMany(d => d.TesisDirigidas)
                    .HasForeignKey(e => e.DocenteCedula)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Apelacion
            modelBuilder.Entity<Apelacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.SolicitudAscensoId).IsRequired();
                entity.Property(e => e.Motivo).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.DocumentosRespaldo).HasMaxLength(500);
                entity.Property(e => e.Estado).HasConversion<int>().IsRequired();
                entity.Property(e => e.FechaPresentacion).IsRequired();
                entity.Property(e => e.FechaLimiteRespuesta).IsRequired();
                entity.Property(e => e.FechaResolucion);
                entity.Property(e => e.ObservacionesComision).HasMaxLength(1000);
                entity.Property(e => e.CreadoPor).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ModificadoPor).HasMaxLength(100);
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.FechaModificacion);
                entity.Property(e => e.Aceptada).IsRequired();

                // Relación con SolicitudAscenso
                entity.HasOne(e => e.SolicitudAscenso)
                    .WithMany(s => s.Apelaciones)
                    .HasForeignKey(e => e.SolicitudAscensoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}