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

            // Convertir Enums a string en la base de datos para mayor legibilidad

            modelBuilder.Entity<Docente>()
                 .HasKey(d => d.Cedula);

            modelBuilder.Entity<Cuenta>()
                .HasKey(c => c.Correo);

            modelBuilder.Entity<Articulo>()
                .HasKey(a => a.DOI);

            modelBuilder.Entity<Cuenta>()
                .Property(c => c.Rol)
                .HasConversion<string>();

            modelBuilder.Entity<SolicitudAscenso>()
                .Property(s => s.Estado)
                .HasConversion<string>();

            modelBuilder.Entity<Docente>()
            .HasOne(d => d.Cuenta)
            .WithOne(c => c.Docente)
            .HasForeignKey<Cuenta>(c => c.DocenteCedula);

            modelBuilder.Entity<Rango>()
             .Property(r => r.PuntajePromedioEvaluacionesRequerido)
              .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<EvaluacionDocente>()
                .Property(e => e.PuntajePorcentual)
                .HasColumnType("decimal(5, 2)");

            // Configurar claves primarias compuestas para las tablas de vínculo
            modelBuilder.Entity<ArticulosPorSolicitud>().HasKey(aps => new { aps.SolicitudId, aps.ArticuloDOI });
            modelBuilder.Entity<CursosPorSolicitud>().HasKey(cps => new { cps.SolicitudId, cps.CursoId });
            modelBuilder.Entity<InvestigacionesPorSolicitud>().HasKey(ips => new { ips.SolicitudId, ips.InvestigacionId });
            modelBuilder.Entity<ExperienciaPorSolicitud>().HasKey(eps => new { eps.SolicitudId, eps.ExperienciaId });
            modelBuilder.Entity<EvaluacionesPorSolicitud>().HasKey(evps => new { evps.SolicitudId, evps.EvaluacionId });

            // Configurar la doble relación entre SolicitudAscenso y Rango
            modelBuilder.Entity<SolicitudAscenso>()
                .HasOne(s => s.RangoActual)
                .WithMany(r => r.SolicitudesComoRangoActual)
                .HasForeignKey(s => s.RangoActualId)
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminación en cascada si es necesario

            modelBuilder.Entity<SolicitudAscenso>()
                .HasOne(s => s.RangoSolicitado)
                .WithMany(r => r.SolicitudesComoRangoSolicitado)
                .HasForeignKey(s => s.RangoSolicitadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Aquí se pueden añadir más configuraciones Fluent API si son necesarias...
        }
    }
}