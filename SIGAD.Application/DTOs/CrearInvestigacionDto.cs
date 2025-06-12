using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class CrearInvestigacionDto
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string RolEnInvestigacion { get; set; } = string.Empty;
        public int MesesDeInvestigacion { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
    }
}
