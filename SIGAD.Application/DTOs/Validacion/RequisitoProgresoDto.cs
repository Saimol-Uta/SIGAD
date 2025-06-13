using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs.Validacion
{
    public class RequisitoProgresoDto
    {
        public decimal Requerido { get; set; }
        public decimal Actual { get; set; }
        public bool Cumple => Actual >= Requerido;
        public string Mensaje { get; set; } = string.Empty;
    }
}
