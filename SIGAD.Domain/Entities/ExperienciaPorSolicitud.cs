using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class ExperienciaPorSolicitud
    {
        public Guid SolicitudId { get; set; }
        public int ExperienciaId { get; set; }

        public virtual SolicitudAscenso SolicitudAscenso { get; set; }
        public virtual ExperienciaLaboral ExperienciaLaboral { get; set; }
    }
}
