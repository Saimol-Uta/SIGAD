using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class VerSolicitudDto
    {
        public Guid Id { get; set; }
        public string NombreDocente { get; set; }
        public string RangoActual { get; set; }
        public string RangoSolicitado { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

