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
        public string PeriodoAcademico { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; }
        public string ContenidoHash { get; set; }
        public string DocenteCedula { get; set; }

        public virtual Docente Docente { get; set; }
    }
}
