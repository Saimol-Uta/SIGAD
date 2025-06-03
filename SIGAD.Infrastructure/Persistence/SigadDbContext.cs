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

            // Aquí puedes configurar tus entidades usando Fluent API si las convenciones no son suficientes.
            // Por ejemplo, para la entidad Rango, EF Core por convención ya sabe que 'Id' es la clave primaria.
            // Pero si quisieras ser explícito o configurar otras cosas:
            // modelBuilder.Entity<Rango>(entity =>
            // {
            //     entity.HasKey(r => r.Id);
            //     entity.Property(r => r.Nombre).IsRequired().HasMaxLength(100);
            //     entity.Property(r => r.Descripcion).HasMaxLength(500);
            // });

            // Aplicar configuraciones desde ensamblados (forma más organizada para proyectos grandes)
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(SigadDbContext).Assembly);
        }
    }
}