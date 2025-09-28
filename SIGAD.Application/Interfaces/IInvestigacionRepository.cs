using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IInvestigacionRepository
    {
        // Operaciones CRUD básicas
        Task<IEnumerable<Investigacion>> GetAllAsync();
        Task<Investigacion?> GetByIdAsync(int id);
        Task<IEnumerable<Investigacion>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<Investigacion>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(Investigacion investigacion);
        Task UpdateAsync(Investigacion investigacion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Operaciones de asociación con solicitudes
        Task AddToSolicitudAsync(Guid solicitudId, int investigacionId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int investigacionId);

        Task<bool> ExistePorHashAsync(string hash);
        Task AgregarAsync(Investigacion investigacion);

        // Métodos específicos para el reglamento:
        Task<int> GetTotalMesesInvestigacionAsync(string docenteCedula);
        Task<int> GetMesesInvestigacionEnPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin);
        Task<bool> CumpleRequisitoInvestigacionParaRangoAsync(string docenteCedula, int rangoSolicitadoId);
        Task<IEnumerable<Investigacion>> GetInvestigacionesConFilacionUTAAsync(string docenteCedula);
        Task<IEnumerable<Investigacion>> GetInvestigacionesComoCoordinadorAsync(string docenteCedula);
        Task<decimal> CalcularTiempoEquivalenteCoordinacionAsync(string docenteCedula);
    }
}