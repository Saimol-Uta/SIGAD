namespace SIGAD.Domain.Interfaces
{
    public interface IInvestigacionRepository
    {
        // Métodos básicos - se implementarán más adelante
        Task<object?> GetByIdAsync(int id);
        Task<IEnumerable<object>> GetAllAsync();
    }
} 