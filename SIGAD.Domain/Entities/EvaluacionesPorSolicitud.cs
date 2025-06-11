using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class EvaluacionesPorSolicitud
    {
        public Guid SolicitudId { get; set; }
        public int EvaluacionId { get; set; }

        public virtual SolicitudAscenso SolicitudAscenso { get; set; }
        public virtual EvaluacionDocente EvaluacionDocente { get; set; }
    }
}
