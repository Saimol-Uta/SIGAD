using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Persistence
{
    public class SigadDbContext : DbContext
    {
        public SigadDbContext(DbContextOptions<SigadDbContext> options) : base(options) { }

        // DbSets para todas las entidades
        public DbSet<Organizacion> Organizaciones { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Rango> Rangos { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<ExperienciaLaboral> ExperienciasLaborales { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<EvaluacionDocente> EvaluacionesDocentes { get; set; }
        public DbSet<Investigacion> Investigaciones { get; set; }
        public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }
        public DbSet<ArticulosPorSolicitud> ArticulosPorSolicitud { get; set; }
        public DbSet<CursosPorSolicitud> CursosPorSolicitud { get; set; }
        public DbSet<InvestigacionesPorSolicitud> InvestigacionesPorSolicitud { get; set; }
        public DbSet<ExperienciaPorSolicitud> ExperienciaPorSolicitud { get; set; }
        public DbSet<EvaluacionesPorSolicitud> EvaluacionesPorSolicitud { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuración de Claves Primarias ---
            modelBuilder.Entity<Docente>().HasKey(d => d.Cedula);
            modelBuilder.Entity<Cuenta>().HasKey(c => c.Correo);
            modelBuilder.Entity<Articulo>().HasKey(a => a.DOI);
            modelBuilder.Entity<Articulo>().Property(a => a.DOI).HasColumnType("varchar(200)");

            // --- Configuración de Precisión Decimal ---
            modelBuilder.Entity<Rango>().Property(r => r.PuntajePromedioEvaluacionesRequerido).HasColumnType("decimal(5, 2)");
            modelBuilder.Entity<EvaluacionDocente>().Property(e => e.PuntajePorcentual).HasColumnType("decimal(5, 2)");

            // --- Configuración de Relaciones Específicas ---
            modelBuilder.Entity<Docente>().HasOne(d => d.Cuenta).WithOne(c => c.Docente).HasForeignKey<Cuenta>(c => c.DocenteCedula);
            modelBuilder.Entity<SolicitudAscenso>().HasOne(s => s.RangoActual).WithMany(r => r.SolicitudesComoRangoActual).HasForeignKey(s => s.RangoActualId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SolicitudAscenso>().HasOne(s => s.RangoSolicitado).WithMany(r => r.SolicitudesComoRangoSolicitado).HasForeignKey(s => s.RangoSolicitadoId).OnDelete(DeleteBehavior.Restrict);

            // --- Configuración de Tablas de Vínculo (Junction Tables) ---

            modelBuilder.Entity<ArticulosPorSolicitud>(entity =>
            {
                entity.HasKey(aps => new { aps.SolicitudId, aps.ArticuloDOI });
                entity.Property(aps => aps.ArticuloDOI).HasColumnType("varchar(200)");
                // El borrado de una Solicitud borra el vínculo, pero el borrado de un Artículo no (Restrict).
                entity.HasOne(aps => aps.SolicitudAscenso).WithMany(s => s.ArticulosPorSolicitud).HasForeignKey(aps => aps.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(aps => aps.Articulo).WithMany().HasForeignKey(aps => aps.ArticuloDOI).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CursosPorSolicitud>(entity =>
            {
                entity.HasKey(cps => new { cps.SolicitudId, cps.CursoId });
                entity.HasOne(cps => cps.SolicitudAscenso).WithMany(s => s.CursosPorSolicitud).HasForeignKey(cps => cps.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(cps => cps.Curso).WithMany().HasForeignKey(cps => cps.CursoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InvestigacionesPorSolicitud>(entity =>
            {
                entity.HasKey(ips => new { ips.SolicitudId, ips.InvestigacionId });
                entity.HasOne(ips => ips.SolicitudAscenso).WithMany(s => s.InvestigacionesPorSolicitud).HasForeignKey(ips => ips.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(ips => ips.Investigacion).WithMany().HasForeignKey(ips => ips.InvestigacionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExperienciaPorSolicitud>(entity =>
            {
                entity.HasKey(eps => new { eps.SolicitudId, eps.ExperienciaId });
                entity.HasOne(eps => eps.SolicitudAscenso).WithMany(s => s.ExperienciaPorSolicitud).HasForeignKey(eps => eps.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(eps => eps.ExperienciaLaboral).WithMany().HasForeignKey(eps => eps.ExperienciaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EvaluacionesPorSolicitud>(entity =>
            {
                entity.HasKey(evps => new { evps.SolicitudId, evps.EvaluacionId });
                entity.HasOne(evps => evps.SolicitudAscenso).WithMany(s => s.EvaluacionesPorSolicitud).HasForeignKey(evps => evps.SolicitudId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(evps => evps.EvaluacionDocente).WithMany().HasForeignKey(evps => evps.EvaluacionId).OnDelete(DeleteBehavior.Restrict);
            });

            // ... (Conversión de Enums a string, si no la has movido)
            modelBuilder.Entity<Cuenta>().Property(c => c.Rol).HasConversion<string>();
            modelBuilder.Entity<SolicitudAscenso>().Property(s => s.Estado).HasConversion<string>();
        }
    }
}