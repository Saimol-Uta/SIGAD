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
        Task<int> CountUnreadByCedulaAsync(string cedula);
        Task<IEnumerable<Notificacion>> GetAllByCedulaOrderedByDateAsync(string cedula);
    }
}
