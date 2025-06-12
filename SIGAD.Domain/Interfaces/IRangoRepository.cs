namespace SIGAD.Domain.Interfaces
{
    public interface IRangoRepository
    {
        // Métodos básicos - se implementarán más adelante
        Task<object?> GetByIdAsync(int id);
        Task<IEnumerable<object>> GetAllAsync();
    }
} 