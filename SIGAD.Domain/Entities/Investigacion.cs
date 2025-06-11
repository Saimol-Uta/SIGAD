using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Investigacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string RolEnInvestigacion { get; set; }
        public int MesesDeInvestigacion { get; set; }
        public string InformeRuta { get; set; }
        public string ContenidoHash { get; set; }
        public string DocenteCedula { get; set; }

        public virtual Docente Docente { get; set; }
    }
}
