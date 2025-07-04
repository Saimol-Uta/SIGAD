using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface INotificacionRepository : IBaseRepository<Notificacion>
    {
        // Aquí podríamos añadir métodos específicos para notificaciones en el futuro, por ejemplo:
        // Task<IEnumerable<Notificacion>> GetUnreadNotificationsForUserAsync(string cedula);
    }
}
