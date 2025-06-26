using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Docente
    {
        public string Cedula { get; set; } = string.Empty; // Mapea a: Cedula VARCHAR(10) PRIMARY KEY
        public string Nombre1 { get; set; } = string.Empty;
        public string? Nombre2 { get; set; } // El '?' indica que este campo puede ser nulo
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;

        public string NombreCompleto => $"{Nombre1} {Nombre2} {Apellido1} {Apellido2}".Replace("  ", " ").Trim();
        public int? RangoActualId { get; set; } // Clave foránea al rango actual
        public virtual Rango? RangoActual { get; set; } // Propiedad de navegación
        // ---------------------------------------------        // Propiedades de navegación para relaciones
        public virtual Cuenta? Cuenta { get; set; } // Un docente tiene una cuenta
        public virtual ICollection<SolicitudAscenso> Solicitudes { get; set; } = new List<SolicitudAscenso>();
        public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
        public virtual ICollection<ExperienciaLaboral> ExperienciasLaborales { get; set; } = new List<ExperienciaLaboral>();
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public virtual ICollection<EvaluacionDocente> Evaluaciones { get; set; } = new List<EvaluacionDocente>();
        public virtual ICollection<Investigacion> Investigaciones { get; set; } = new List<Investigacion>();
        public virtual ICollection<TesisDirigida> TesisDirigidas { get; set; } = new List<TesisDirigida>();
        public virtual ICollection<AccionesDePersonal> AccionesDePersonal { get; set; } = new List<AccionesDePersonal>();
    }
}
