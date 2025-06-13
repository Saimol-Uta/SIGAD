using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IOrganizacionRepository
    {
        Task<IEnumerable<Organizacion>> GetAllAsync();
        Task<Organizacion?> GetByIdAsync(int id);
        Task AddAsync(Organizacion organizacion);
        Task UpdateAsync(Organizacion organizacion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 