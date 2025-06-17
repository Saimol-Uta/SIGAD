using SIGAD.Application.DTOs.IntegracionesExternas;

namespace SIGAD.Application.Interfaces.Integraciones
{
    public interface IDiticSyncService
    {
        Task<IEnumerable<DocenteDto>> ObtenerDocentesAsync();
    }
}
