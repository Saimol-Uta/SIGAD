using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IEvaluacionDocenteRepository
    {
        Task<IEnumerable<EvaluacionDocente>> GetAllAsync();
        Task<EvaluacionDocente?> GetByIdAsync(int id);
        Task<IEnumerable<EvaluacionDocente>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<EvaluacionDocente>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(EvaluacionDocente evaluacion);
        Task UpdateAsync(EvaluacionDocente evaluacion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task AddToSolicitudAsync(Guid solicitudId, int evaluacionId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int evaluacionId);
    }
} 