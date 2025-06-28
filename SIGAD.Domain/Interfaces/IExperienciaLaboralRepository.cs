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
        Task<bool> ExistePorHashAsync(string hash);
        Task AgregarAsync(ExperienciaLaboral experiencia);

        // Métodos específicos del reglamento:
        Task<int> GetAniosExperienciaDocenteAsync(string docenteCedula);
        Task<int> GetAniosExperienciaEnUTAAsync(string docenteCedula);
        Task<bool> CumpleRequisitoExperienciaParaRangoAsync(string docenteCedula, int rangoSolicitadoId);
        Task<IEnumerable<ExperienciaLaboral>> GetExperienciaAcademicaAsync(string docenteCedula);
        Task<DateTime?> GetFechaInicioEnUTAAsync(string docenteCedula);
    }
}