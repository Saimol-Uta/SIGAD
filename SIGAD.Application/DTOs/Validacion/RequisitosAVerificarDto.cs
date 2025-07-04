using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs.Validacion
{
    public class RequisitosAVerificarDto
    {
        public string DocenteCedula { get; set; }
        public int RangoId { get; set; }
        public List<string>? ArticulosDOI { get; set; }
        public List<int>? CursosId { get; set; }
        public List<int>? ExperienciasId { get; set; }
        public List<int>? InvestigacionesId { get; set; }
        public List<int>? EvaluacionesId { get; set; }
        public List<int>? TesisId { get; set; }
    }

}
