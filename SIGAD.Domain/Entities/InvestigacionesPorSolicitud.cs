using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class InvestigacionesPorSolicitud
    {
        public Guid SolicitudId { get; set; }
        public int InvestigacionId { get; set; }

        public virtual SolicitudAscenso SolicitudAscenso { get; set; }
        public virtual Investigacion Investigacion { get; set; }
    }
}
