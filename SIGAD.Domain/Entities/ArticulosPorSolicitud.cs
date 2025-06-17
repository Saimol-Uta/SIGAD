using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class ArticulosPorSolicitud
    {
        public Guid SolicitudId { get; set; } // Parte 1 de la Clave Primaria Compuesta
        public string ArticuloDOI { get; set; } // Parte 2 de la Clave Primaria Compuesta

        // Propiedades de navegación
        public virtual SolicitudAscenso SolicitudAscenso { get; set; }
        public virtual Articulo Articulo { get; set; }
    }
}
