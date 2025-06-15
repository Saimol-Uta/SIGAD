using SIGAD.Application.DTOs.IntegracionesExternas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.Application.Interfaces.Integraciones
{
    public interface ISgthSyncService
    {
        Task<IEnumerable<ArticuloDto>> ObtenerArticulosAsync(string cedula);
        Task<IEnumerable<CursoDto>> ObtenerCursosAsync(string cedula);
        Task<IEnumerable<EvaluacionDto>> ObtenerEvaluacionesAsync(string cedula);
        Task<IEnumerable<InvestigacionDto>> ObtenerInvestigacionesAsync(string cedula);
        Task<IEnumerable<ExperienciaDto>> ObtenerExperienciasAsync(string cedula);
    }
}
