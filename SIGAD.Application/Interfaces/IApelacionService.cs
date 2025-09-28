using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    /// <summary>
    /// Servicio para manejar el proceso de apelaciones según Artículo 6 del Reglamento UTA
    /// </summary>
    public interface IApelacionService
    {
        /// <summary>
        /// Crea una nueva apelación para una solicitud rechazada
        /// </summary>
        Task<Apelacion> CrearApelacionAsync(Guid solicitudId, string motivo, string creadoPor, string? documentosRespaldo = null);

        /// <summary>
        /// Resuelve una apelación (aceptar o rechazar)
        /// </summary>
        Task<bool> ResolverApelacionAsync(int apelacionId, bool aceptada, string observaciones, string resueltoPor);

        /// <summary>
        /// Obtiene todas las apelaciones pendientes de resolución
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesPendientesAsync();

        /// <summary>
        /// Verifica si una solicitud puede ser apelada
        /// </summary>
        Task<bool> PuedeApelarAsync(Guid solicitudId);

        /// <summary>
        /// Marca automáticamente las apelaciones vencidas (más de 3 días)
        /// </summary>
        Task MarcarApelacionesVencidasAsync();

        /// <summary>
        /// Obtiene el historial de apelaciones de una solicitud
        /// </summary>
        Task<IEnumerable<Apelacion>> GetHistorialApelacionesAsync(Guid solicitudId);

        /// <summary>
        /// Verifica si una solicitud está en plazo para apelar
        /// </summary>
        Task<bool> EstaEnPlazoParaApelarAsync(Guid solicitudId);

        /// <summary>
        /// Agrega documentos de respaldo a una apelación existente
        /// </summary>
        Task<bool> AgregarDocumentoRespaldoAsync(int apelacionId, string documento);
    }
}
