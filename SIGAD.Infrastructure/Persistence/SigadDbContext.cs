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
        // DbSet para cada entidad que quieras que EF Core gestione.
        // Representan las tablas en la base de datos.
        public DbSet<Rango> Rangos { get; set; }
        // public DbSet<Docente> Docentes { get; set; } // Añadirás más DbSets a medida que crees más entidades
        // public DbSet<SolicitudAscenso> SolicitudesAscenso { get; set; }

        // Constructor que permite pasar opciones de configuración (como la cadena de conexión)
        // desde el exterior (usualmente desde la configuración de la Web API).
        public SigadDbContext(DbContextOptions<SigadDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rango>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Descripcion).HasMaxLength(500);
            });

            // Sembrar datos para la entidad Rango CON GUIDs ESTÁTICOS
            modelBuilder.Entity<Rango>().HasData(
                new Rango(new Guid("c1a75764-3420-4e00-91c0-66917c0d3e6f"), "Profesor Auxiliar TC", "Profesor de Tiempo Completo en categoría Auxiliar."),
                new Rango(new Guid("d2b86889-81b2-4a3a-984e-127424d349af"), "Profesor Asistente TC", "Profesor de Tiempo Completo en categoría Asistente."),
                new Rango(new Guid("e3c97990-92c3-5b4b-a95f-238535e450b0"), "Profesor Asociado TC", "Profesor de Tiempo Completo en categoría Asociado."),
                new Rango(new Guid("f4d08aa1-a3d4-6c5c-ba60-349646f561c1"), "Profesor Titular TC", "Profesor de Tiempo Completo en categoría Titular.")
            );
        }
    }
}