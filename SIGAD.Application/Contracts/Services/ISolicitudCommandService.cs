using SIGAD.Application.DTOs;

namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para operaciones de comando (escritura) de solicitudes de ascenso.
    /// Principio ISP: los clientes que solo modifican datos no dependen de métodos de lectura.
    /// </summary>
    public interface ISolicitudCommandService
    {
        /// <summary>
        /// Crea una nueva solicitud de ascenso en estado borrador.
        /// </summary>
        Task<SolicitudDetalleDto> CrearSolicitudAsync(CrearSolicitudDto dto);

        /// <summary>
        /// Envía una solicitud para revisión (cambia de borrador a en proceso).
        /// </summary>
        Task<bool> EnviarSolicitudAsync(Guid solicitudId);

        /// <summary>
        /// Aprueba una solicitud por parte de la comisión.
        /// </summary>
        Task<bool> AprobarPorComisionAsync(Guid solicitudId, string? observaciones = null);

        /// <summary>
        /// Aprueba una solicitud por parte del consejo.
        /// </summary>
        Task<bool> AprobarPorConsejoAsync(Guid solicitudId, string? observaciones = null);

        /// <summary>
        /// Rechaza una solicitud con observaciones obligatorias.
        /// </summary>
        Task<bool> RechazarSolicitudAsync(Guid solicitudId, string observaciones);

        /// <summary>
        /// Cancela una solicitud (solo si está en borrador).
        /// </summary>
        Task<bool> CancelarSolicitudAsync(Guid solicitudId);

        /// <summary>
        /// Finaliza el proceso de una solicitud aprobada.
        /// </summary>
        Task<bool> FinalizarProcesoAsync(Guid solicitudId, string? observaciones = null);

        /// <summary>
        /// Asocia un artículo a una solicitud.
        /// </summary>
        Task<bool> AsociarArticuloAsync(AsociarArticuloSolicitudDto dto);

        /// <summary>
        /// Desasocia un artículo de una solicitud.
        /// </summary>
        Task<bool> DesasociarArticuloAsync(DesasociarArticuloSolicitudDto dto);

        /// <summary>
        /// Asocia un curso a una solicitud.
        /// </summary>
        Task<bool> AsociarCursoAsync(AsociarCursoSolicitudDto dto);

        /// <summary>
        /// Asocia una investigación a una solicitud.
        /// </summary>
        Task<bool> AsociarInvestigacionAsync(AsociarInvestigacionSolicitudDto dto);

        /// <summary>
        /// Asocia una tesis dirigida a una solicitud.
        /// </summary>
        Task<bool> AsociarTesisAsync(AsociarTesisSolicitudDto dto);

        /// <summary>
        /// Asocia una evaluación docente a una solicitud.
        /// </summary>
        Task<bool> AsociarEvaluacionAsync(AsociarEvaluacionSolicitudDto dto);
    }
}

