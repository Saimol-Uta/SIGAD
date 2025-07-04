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
        Task<bool> ExistePorHashAsync(string hash);
        Task AgregarAsync(EvaluacionDocente evaluacion);

        // Métodos específicos del reglamento:
        Task<decimal> GetPromedioUltimas4EvaluacionesAsync(string docenteCedula);
        Task<IEnumerable<EvaluacionDocente>> GetUltimas4EvaluacionesAsync(string docenteCedula);
        Task<IEnumerable<EvaluacionDocente>> GetUltimas2EvaluacionesAsync(string docenteCedula); // Para excepcionalidades Art. 7
        Task<bool> CumpleRequisitoEvaluacionParaRangoAsync(string docenteCedula, decimal puntajeMinimo = 75);
        Task<bool> TieneEvaluacionesSuficientesAsync(string docenteCedula, int cantidadMinima = 4);
        Task<bool> EstaEvaluacionYaUsadaAsync(int evaluacionId);
        Task<IEnumerable<EvaluacionDocente>> GetEvaluacionesDisponiblesParaSolicitudAsync(string docenteCedula, Guid? solicitudActualId = null);
        Task<IEnumerable<EvaluacionDocente>> GetEvaluacionesUsadasEnSolicitudesAsync(string docenteCedula);
    }
}