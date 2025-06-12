using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Infrastructure.Persistence
{
    public class SigadDbContext : DbContext
    {
        public SigadDbContext(DbContextOptions<SigadDbContext> options) : base(options)
        {
        }

        // DbSets para las entidades
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<EvaluacionDocente> EvaluacionesDocentes { get; set; }
        public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
        public DbSet<EvaluacionPorSolicitud> EvaluacionesPorSolicitud { get; set; }

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

                entity.HasCheckConstraint("CK_SolicitudesAscenso_Estado", 
                    "Estado IN ('Borrador', 'Enviada', 'En Revision', 'Aprobada', 'Rechazada')");
            });

            // Configuración de la tabla intermedia EvaluacionPorSolicitud
            modelBuilder.Entity<EvaluacionPorSolicitud>(entity =>
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
        }
    }
} 