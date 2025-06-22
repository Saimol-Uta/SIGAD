using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class AccionesDePersonal
    {
        public int Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string DocumentoRuta { get; set; } = string.Empty; // Para base interna
        public string CertificadoRuta { get; set; } = string.Empty; // Para bases externas
        public string ContenidoHash { get; set; } = string.Empty;        // Propiedad de navegación hacia el Docente
        public virtual Docente Docente { get; set; } = null!;

        // Relación con solicitudes
        public virtual ICollection<AccionesDePersonalPorSolicitud> AccionesDePersonalPorSolicitud { get; set; } = new List<AccionesDePersonalPorSolicitud>();
    }
}
