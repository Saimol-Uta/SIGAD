using SIGAD.Domain.Entities;
using SIGAD.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Interfaces
{
    public interface INotificacionService
    {
        Task EnviarNotificacionAprobacionAsync(SolicitudAscenso solicitud, string observaciones);
        Task EnviarNotificacionRechazoAsync(SolicitudAscenso solicitud, string observaciones);
        Task<int> GetUnreadCountByCedulaAsync(string cedula);
        Task<IEnumerable<NotificacionDto>> GetNotificacionesByCedulaAsync(string cedula);
        Task<bool> MarkAsReadAsync(int notificacionId, string userCedula);

    }
}
