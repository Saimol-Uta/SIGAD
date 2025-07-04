using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Interfaces
{
    public interface ISolicitudAscensoRepository
    {
        Task<SolicitudAscenso?> GetByIdAsync(Guid id);
        Task<SolicitudAscenso?> GetTrackedByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync();
        Task<SolicitudAscenso?> GetByIdWithDetailsAsync(Guid id);
        Task AddAsync(SolicitudAscenso solicitud);
        Task UpdateAsync(SolicitudAscenso solicitud);
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<SolicitudAscenso>> GetAllAsync();

        // Métodos específicos para el proceso de promoción
        Task<IEnumerable<SolicitudAscenso>> GetByDocenteAsync(string docenteCedula);
        Task<SolicitudAscenso?> GetActiveSolicitudByDocenteAsync(string docenteCedula);
        Task<IEnumerable<SolicitudAscenso>> GetByEstadoAsync(EstadoSolicitud estado);
        Task<IEnumerable<SolicitudAscenso>> GetPendientesRevisionAsync();    // Métodos para validación de requisitos de promoción
        Task<bool> HasActiveSolicitudAsync(string docenteCedula);
        Task<int> GetTiempoEnRangoActualAsync(string docenteCedula);
        Task<bool> CumpleRequisitosParaRangoAsync(string docenteCedula, int rangoSolicitadoId);

        // Métodos para el workflow del reglamento
        Task EnviarSolicitudAsync(Guid solicitudId);
        Task AprobarSolicitudAsync(Guid solicitudId, string? observaciones = null);
        Task RechazarSolicitudAsync(Guid solicitudId, string observaciones);
        
        // Métodos específicos para el proceso de dos etapas según Reglamento UTA
        Task AprobarPorComisionAsync(Guid solicitudId, string? observaciones = null);
        Task AprobarPorConsejoAsync(Guid solicitudId, string? observaciones = null);
        Task FinalizarProcesoAsync(Guid solicitudId, string? observaciones = null);
        
        Task<IEnumerable<SolicitudAscenso>> GetHistorialByDocenteAsync(string docenteCedula);

        // Métodos para reportes y estadísticas
        Task<int> GetCantidadSolicitudesByEstadoAsync(EstadoSolicitud estado);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}