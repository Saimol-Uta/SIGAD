namespace SIGAD.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        // Métodos básicos - se implementarán más adelante
        Task<object?> GetByIdAsync(string doi);
        Task<IEnumerable<object>> GetAllAsync();
    }
} 