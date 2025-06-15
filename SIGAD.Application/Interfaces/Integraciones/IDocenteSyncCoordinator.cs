using SIGAD.Application.Common;
using SIGAD.Application.DTOs.IntegracionesExternas;

namespace SIGAD.Application.Interfaces.Integraciones
{
    public interface IDocenteSyncCoordinator
    {
        Task<HistorialDocenteDto> SincronizarDesdeFuenteAsync(string cedula, Fuente fuentePreferida);
    }
}
