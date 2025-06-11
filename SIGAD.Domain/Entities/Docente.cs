using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Docente
    {
        public string Cedula { get; set; } // Mapea a: Cedula VARCHAR(10) PRIMARY KEY
        public string Nombre1 { get; set; }
        public string? Nombre2 { get; set; } // El '?' indica que este campo puede ser nulo
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }

        // Propiedades de navegación para relaciones
        public virtual Cuenta Cuenta { get; set; } // Un docente tiene una cuenta
        public virtual ICollection<SolicitudAscenso> SolicitudesAscenso { get; set; } = new List<SolicitudAscenso>();
        public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();
        public virtual ICollection<ExperienciaLaboral> ExperienciasLaborales { get; set; } = new List<ExperienciaLaboral>();
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
        public virtual ICollection<EvaluacionDocente> EvaluacionesDocentes { get; set; } = new List<EvaluacionDocente>();
        public virtual ICollection<Investigacion> Investigaciones { get; set; } = new List<Investigacion>();
    }
}
