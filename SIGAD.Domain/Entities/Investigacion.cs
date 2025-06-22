using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    public class Investigacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string RolEnInvestigacion { get; set; } = string.Empty;
        public int MesesDeInvestigacion { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;

        public TipoInvestigacion TipoProyecto { get; set; } = TipoInvestigacion.Aplicada;
        public int MesesDeParticipacion { get; set; }
        public string UnidadVerificadora { get; set; } = string.Empty;

        public virtual Docente Docente { get; set; } = default!;
    }
}
