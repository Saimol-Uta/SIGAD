using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class ExperienciaLaboral
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; } // Clave foránea
        public string DocenteCedula { get; set; } // Clave foránea
        public string Cargo { get; set; }
        public DateTime FechaInicio { get; set; } // El tipo DATE de SQL se mapea a DateTime en C#
        public DateTime? FechaFin { get; set; } // El '?' indica que puede ser nulo
        public string CertificadoRuta { get; set; }
        public string ContenidoHash { get; set; }

        // Propiedades de navegación para ambas claves foráneas
        public virtual Organizacion Organizacion { get; set; }
        public virtual Docente Docente { get; set; }
        public virtual ICollection<ExperienciaPorSolicitud> ExperienciasPorSolicitud { get; set; } = new List<ExperienciaPorSolicitud>();
    }
}
