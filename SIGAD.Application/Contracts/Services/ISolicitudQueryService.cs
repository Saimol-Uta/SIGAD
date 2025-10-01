using SIGAD.Application.DTOs;
using SIGAD.Domain.Enums;

namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para operaciones de consulta (lectura) de solicitudes de ascenso.
    /// Principio ISP: los clientes que solo leen no dependen de métodos de escritura.
    /// </summary>
    public interface ISolicitudQueryService
    {
        /// <summary>
        /// Obtiene una solicitud por su identificador con todos sus detalles.
        /// </summary>
        Task<SolicitudDetalleDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene todas las solicitudes con sus detalles.
        /// </summary>
        Task<IEnumerable<SolicitudDetalleDto>> GetAllAsync();

        /// <summary>
        /// Obtiene el historial de solicitudes de un docente específico.
        /// </summary>
        Task<IEnumerable<SolicitudDetalleDto>> GetHistorialByDocenteAsync(string docenteCedula);

        /// <summary>
        /// Obtiene las solicitudes filtradas por estado.
        /// </summary>
        Task<IEnumerable<SolicitudDetalleDto>> GetByEstadoAsync(EstadoSolicitud estado);

        /// <summary>
        /// Obtiene las solicitudes pendientes de revisión.
        /// </summary>
        Task<IEnumerable<SolicitudDetalleDto>> GetPendientesRevisionAsync();

        /// <summary>
        /// Verifica si un docente tiene una solicitud activa.
        /// </summary>
        Task<bool> HasActiveSolicitudAsync(string docenteCedula);

        /// <summary>
        /// Obtiene la solicitud activa de un docente (en estado borrador o en proceso).
        /// </summary>
        Task<SolicitudDetalleDto?> GetActiveSolicitudByDocenteAsync(string docenteCedula);

        /// <summary>
        /// Obtiene estadísticas de solicitudes por estado.
        /// </summary>
        Task<Dictionary<EstadoSolicitud, int>> GetEstadisticasByEstadoAsync();

        /// <summary>
        /// Obtiene las solicitudes en un rango de fechas.
        /// </summary>
        Task<IEnumerable<SolicitudDetalleDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    }
}

