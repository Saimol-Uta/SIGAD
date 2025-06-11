using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class CursosPorSolicitud
    {
        public Guid SolicitudId { get; set; }
        public int CursoId { get; set; }

        public virtual SolicitudAscenso SolicitudAscenso { get; set; }
        public virtual Curso Curso { get; set; }
    }
}
