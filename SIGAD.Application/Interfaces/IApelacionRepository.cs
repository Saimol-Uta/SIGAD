using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Interfaces
{
    /// <summary>
    /// Repositorio para manejar las apelaciones según Artículo 6 del Reglamento UTA
    /// </summary>
    public interface IApelacionRepository : IBaseRepository<Apelacion>
    {
        /// <summary>
        /// Actualiza una apelación existente en la base de datos.
        /// </summary>
        /// <param name="apelacion">La apelación a actualizar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task UpdateAsync(Apelacion apelacion);

        /// <summary>
        /// Obtiene todas las apelaciones pendientes de resolución
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesPendientesAsync();

        /// <summary>
        /// Obtiene las apelaciones de una solicitud específica
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesPorSolicitudAsync(Guid solicitudId);

        /// <summary>
        /// Obtiene las apelaciones vencidas (más de 3 días sin resolución)
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesVencidasAsync();

        /// <summary>
        /// Verifica si una solicitud tiene apelaciones pendientes
        /// </summary>
        Task<bool> TieneApelacionPendienteAsync(Guid solicitudId);

        /// <summary>
        /// Obtiene apelaciones por estado
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesPorEstadoAsync(EstadoApelacion estado);

        /// <summary>
        /// Obtiene apelaciones dentro de un rango de fechas
        /// </summary>
        Task<IEnumerable<Apelacion>> GetApelacionesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene la última apelación de una solicitud
        /// </summary>
        Task<Apelacion?> GetUltimaApelacionPorSolicitudAsync(Guid solicitudId);
    }
}
