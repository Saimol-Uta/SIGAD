using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Interfaces
{
    /// <summary>
    /// Servicio para manejar la lógica de negocio del proceso de promoción académica
    /// según el Reglamento para la Promoción del Personal Académico Titular de la UTA
    /// </summary>
    public interface IPromocionService
    {
        // Validación integral de requisitos
        Task<bool> ValidarRequisitosPromocionAsync(string docenteCedula, int rangoSolicitadoId);
        Task<Dictionary<string, bool>> GetEstadoRequisitosAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> PuedeAplicarPromocionAsync(string docenteCedula);

        // Validación específica por tipo de requisito
        Task<bool> ValidarTiempoMinimoEnRangoAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarObrasRelevantesAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarEvaluacionIntegralAsync(string docenteCedula);
        Task<bool> ValidarHorasCapacitacionAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarExperienciaInvestigacionAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarTesisDirigidasAsync(string docenteCedula, int rangoSolicitadoId);

        // Cálculos específicos del reglamento
        Task<int> CalcularTiempoEnRangoActualAsync(string docenteCedula);
        Task<decimal> CalcularPromedioEvaluacionesAsync(string docenteCedula, int periodosEvaluacion = 4);
        Task<int> CalcularHorasCapacitacionAsync(string docenteCedula, int ultimosAnios = 3);
        Task<int> CalcularMesesInvestigacionAsync(string docenteCedula);

        // Aplicar excepcionalidades del Art. 7
        Task<bool> AplicaExcepcionalidadAutoridadAsync(string docenteCedula);
        Task<bool> AplicaExcepcionalidadSabaticoAsync(string docenteCedula);

        // Procesamiento de solicitudes
        Task<SolicitudAscenso> CrearSolicitudPromocionAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> EnviarSolicitudAsync(Guid solicitudId);
        Task<bool> ProcesarSolicitudAsync(Guid solicitudId, EstadoSolicitud nuevoEstado, string? observaciones = null);

        // Reportes y estadísticas
        Task<Dictionary<string, object>> GenerarReporteRequisitosAsync(string docenteCedula, int rangoSolicitadoId);
        Task<IEnumerable<SolicitudAscenso>> GetSolicitudesPendientesRevisionAsync();
    }
}
