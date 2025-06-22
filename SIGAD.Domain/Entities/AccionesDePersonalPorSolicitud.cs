using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class AccionesDePersonalPorSolicitud
    {
        public Guid SolicitudId { get; set; } // Parte 1 de la Clave Primaria Compuesta
        public int AccionDePersonalId { get; set; } // Parte 2 de la Clave Primaria Compuesta

        // Propiedades de navegación
        public virtual SolicitudAscenso SolicitudAscenso { get; set; } = null!;
        public virtual AccionesDePersonal AccionDePersonal { get; set; } = null!;
    }
}
