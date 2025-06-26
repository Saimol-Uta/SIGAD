using SIGAD.Application.Common;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces.Integraciones;

namespace SIGAD.Application.Services
{
    public class DocenteSyncCoordinator : IDocenteSyncCoordinator
    {
        private readonly ISgthSyncService _sgthService;
        private readonly ISutSyncService _sutService;

        public DocenteSyncCoordinator(ISgthSyncService sgthService, ISutSyncService sutService)
        {
            _sgthService = sgthService;
            _sutService = sutService;
        }

        public async Task<HistorialDocenteDto> SincronizarDesdeFuenteAsync(string cedula, Fuente fuentePreferida)
        {
            if (fuentePreferida == Fuente.SGTH)
            {
                return new HistorialDocenteDto
                {
                    Articulos = (await _sgthService.ObtenerArticulosAsync(cedula)).ToList(),
                    Cursos = (await _sgthService.ObtenerCursosAsync(cedula)).ToList(),
                    Evaluaciones = (await _sgthService.ObtenerEvaluacionesAsync(cedula)).ToList(),
                    Investigaciones = (await _sgthService.ObtenerInvestigacionesAsync(cedula)).ToList(),
                    Experiencias = (await _sgthService.ObtenerExperienciasAsync(cedula)).ToList()
                };
            }

            return new HistorialDocenteDto
            {
                Articulos = (await _sutService.ObtenerArticulosAsync(cedula)).ToList(),
                Cursos = (await _sutService.ObtenerCursosAsync(cedula)).ToList(),
                Evaluaciones = (await _sutService.ObtenerEvaluacionesAsync(cedula)).ToList(),
                Investigaciones = (await _sutService.ObtenerInvestigacionesAsync(cedula)).ToList(),
                Experiencias = (await _sutService.ObtenerExperienciasAsync(cedula)).ToList()
            };
        }
    }
}
