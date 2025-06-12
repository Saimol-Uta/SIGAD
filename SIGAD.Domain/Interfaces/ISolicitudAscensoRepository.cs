using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface ISolicitudAscensoRepository
    {
        Task<SolicitudAscenso?> GetByIdAsync(Guid id);
        Task<IEnumerable<SolicitudAscenso>> GetAllAsync();
        Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync();
        Task AddAsync(SolicitudAscenso solicitud);
        Task UpdateAsync(SolicitudAscenso solicitud);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
