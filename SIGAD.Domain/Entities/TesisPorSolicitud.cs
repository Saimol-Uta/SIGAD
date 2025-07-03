using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    /// <summary>
    /// Entidad de relación entre SolicitudAscenso y TesisDirigida
    /// para el requerimiento de tesis dirigidas en el proceso de ascenso
    /// </summary>
    public class TesisPorSolicitud
    {
        public Guid SolicitudId { get; set; } // Parte 1 de la Clave Primaria Compuesta
        public int TesisId { get; set; } // Parte 2 de la Clave Primaria Compuesta

        // Propiedades de navegación
        public virtual SolicitudAscenso? SolicitudAscenso { get; set; } = null!;
        public virtual TesisDirigida? TesisDirigida { get; set; } = null!;
    }
}
