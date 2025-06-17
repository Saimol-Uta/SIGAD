using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Organizacion
    {
        public int Id { get; set; } // Mapea a: Id INT IDENTITY(1,1) PRIMARY KEY
        public string Nombre { get; set; } // Mapea a: Nombre VARCHAR(150) NOT NULL
        public string TipoOrganizacion { get; set; } // Mapea a: TipoOrganizacion VARCHAR(20) NOT NULL

        // Propiedad de navegación: Una organización puede tener muchas experiencias laborales asociadas.
        // EF Core usará esto para entender la relación.
        public virtual ICollection<ExperienciaLaboral> ExperienciasLaborales { get; set; } = new List<ExperienciaLaboral>();
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
