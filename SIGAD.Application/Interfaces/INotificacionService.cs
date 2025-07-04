using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Interfaces
{
    public interface INotificacionService
    {
        /// <summary>
        /// Notifica al docente sobre la APROBACIÓN de su solicitud, usando una plantilla HTML.
        /// </summary>
        Task EnviarNotificacionAprobacionAsync(SolicitudAscenso solicitud, string observaciones);

        /// <summary>
        /// Notifica al docente sobre el RECHAZO de su solicitud, usando una plantilla HTML.
        /// </summary>
        Task EnviarNotificacionRechazoAsync(SolicitudAscenso solicitud, string observaciones);
    }
}
