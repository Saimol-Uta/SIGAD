using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IRangoRepository
    {
        // Métodos básicos - se implementarán más adelante
        Task<Rango?> GetByIdAsync(int id);
        Task<IEnumerable<Rango>> GetAllAsync();
    }
} 