using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs.Validacion
{
    public class ProgresoRequisitosDto
    {
        public RequisitoProgresoDto Antiguedad { get; set; } = new RequisitoProgresoDto();
        public RequisitoProgresoDto PromedioEvaluacion { get; set; }
        public RequisitoProgresoDto Articulos { get; set; } 
        public RequisitoProgresoDto Investigaciones { get; set; } 
        public RequisitoProgresoDto Cursos { get; set; }
        public RequisitoProgresoDto Tesis { get; set; } = new();

        public bool PuedeAscender { get; set; }
    }
}
