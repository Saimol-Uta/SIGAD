using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class EvaluacionDocente
    {
        public int Id { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Docente Docente { get; set; } = null!;
        public virtual ICollection<EvaluacionesPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionesPorSolicitud>();
    }
}
