using SIGAD.Application.DTOs.IntegracionesExternas;


namespace SIGAD.Application.Interfaces.Integraciones
{
    public interface ISgthSyncService
    {
        Task<IEnumerable<ArticuloExternoDto>> ObtenerArticulosAsync(string cedula);
        Task<IEnumerable<CursoDto>> ObtenerCursosAsync(string cedula);
        Task<IEnumerable<EvaluacionDto>> ObtenerEvaluacionesAsync(string cedula);
        Task<IEnumerable<InvestigacionDto>> ObtenerInvestigacionesAsync(string cedula);
        Task<IEnumerable<ExperienciaDto>> ObtenerExperienciasAsync(string cedula);
        Task<IEnumerable<TesisDirigidaExternaDto>> ObtenerTesisDirigidasAsync(string cedula);

    }
}
