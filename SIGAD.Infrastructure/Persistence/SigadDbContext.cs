using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Infrastructure.Persistence
{
    public class SigadDbContext : DbContext
    {
        public SigadDbContext(DbContextOptions<SigadDbContext> options) : base(options)  { }

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
            });

            // Configuración de la entidad Cuenta
            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasKey(e => e.Correo);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.ClaveHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Rol).HasConversion<string>().IsRequired();

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
            });

            // Configuración de la entidad SolicitudAscenso
            modelBuilder.Entity<SolicitudAscenso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Estado).HasMaxLength(20).IsRequired();

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
                entity.Property(e => e.PuntajePromedioEvaluacionesRequerido).HasColumnType("decimal(5,2)").IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de la entidad Articulo
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
            });

            // Configuración de la entidad Curso
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.NumeroHoras).IsRequired();
                entity.Property(e => e.FechaFinalizacion).IsRequired();
                entity.Property(e => e.CertificadoRuta).IsRequired();
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();

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
            });

            // Configuración de la entidad Investigacion
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
                entity.Property(e => e.ContenidoHash).HasMaxLength(64).IsRequired();
                entity.Property(e => e.DocenteCedula).HasMaxLength(10).IsRequired();

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
                    .WithMany()
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
                    .WithMany()
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
                    .WithMany()
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
                    .WithMany()
                    .HasForeignKey(e => e.InvestigacionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
} 