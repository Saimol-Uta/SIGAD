using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Enums
{
    public enum EstadoSolicitud
    {
        Borrador = 1,
        Enviada = 2,
        EnRevision = 3,
        Aprobada = 4,
        Rechazada = 5,
        Apelada = 6
    }
}
