using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IInvestigacionRepository
    {
        // Método que necesita el servicio de validación
        Task<IEnumerable<Investigacion>> GetByDocenteAsync(string cedula);

        // Puedes mantener o añadir otros métodos aquí si los necesitas
        Task<Investigacion?> GetByIdAsync(int id);
        Task<IEnumerable<Investigacion>> GetAllAsync();

        Task AddAsync(Investigacion investigacion);
    }
} 