using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Repositories
{
    public interface ISolicitudAscensoRepository
    {
        Task<SolicitudAscenso?> GetByIdAsync(Guid id);
        Task<IEnumerable<SolicitudAscenso>> GetAllAsync();
        Task AddAsync(SolicitudAscenso solicitud);
        Task UpdateAsync(SolicitudAscenso solicitud);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
} 