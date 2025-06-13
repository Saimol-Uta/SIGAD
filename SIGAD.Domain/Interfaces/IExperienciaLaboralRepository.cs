using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IExperienciaLaboralRepository
    {
        Task<IEnumerable<ExperienciaLaboral>> GetAllAsync();
        Task<ExperienciaLaboral?> GetByIdAsync(int id);
        Task<IEnumerable<ExperienciaLaboral>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<ExperienciaLaboral>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(ExperienciaLaboral experiencia);
        Task UpdateAsync(ExperienciaLaboral experiencia);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddToSolicitudAsync(Guid solicitudId, int experienciaId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int experienciaId);
    }
} 