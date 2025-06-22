using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ITesisDirigidaRepository
    {
        Task<IEnumerable<TesisDirigida>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<TesisDirigida>> GetBySolicitudIdAsync(Guid solicitudId);
        Task<TesisDirigida?> GetByIdAsync(int id);
        Task AddAsync(TesisDirigida tesis);
        Task UpdateAsync(TesisDirigida tesis);
        Task DeleteAsync(int id);
        Task<bool> ExistsByHashAsync(string hash);
        Task AddToSolicitudAsync(Guid solicitudId, int tesisId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int tesisId);

        // Métodos específicos para el reglamento de promoción
        Task<int> GetCantidadTesisDirigidasAsync(string docenteCedula, string? nivelAcademico = null);
        Task<IEnumerable<TesisDirigida>> GetTesisActivasAsync(string docenteCedula);
        Task<IEnumerable<TesisDirigida>> GetTesisByNivelAsync(string docenteCedula, string nivelAcademico);

        // Para validación de requisitos específicos del reglamento
        Task<int> GetCantidadTesisDoctoradoAsync(string docenteCedula);
        Task<bool> CumpleRequisitoTesisParaRangoAsync(string docenteCedula, int rangoSolicitadoId);

        // Para reportes y estadísticas
        Task<IEnumerable<TesisDirigida>> GetTesisByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin);
        Task<Dictionary<string, int>> GetEstadisticasPorNivelAsync(string docenteCedula);
    }
}
