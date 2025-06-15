using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Common;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/docentes/historial")]
    public class DocenteHistorialController : ControllerBase
    {
        private readonly IDocenteSyncCoordinator _syncCoordinator;
        private readonly HistorialDocenteImporter _importer;

        public DocenteHistorialController(
            IDocenteSyncCoordinator syncCoordinator,
            HistorialDocenteImporter importer)
        {
            _syncCoordinator = syncCoordinator;
            _importer = importer;
        }

        [HttpPost("importar")]
        public async Task<IActionResult> ImportarHistorial(string cedula, Fuente fuente)
        {
            var historial = await _syncCoordinator.SincronizarDesdeFuenteAsync(cedula, fuente);

            await _importer.ImportarHistorialAsync(historial, cedula);

            return Ok(new
            {
                message = "Historial importado correctamente.",
                total = new
                {
                    Articulos = historial.Articulos.Count,
                    Cursos = historial.Cursos.Count,
                    Evaluaciones = historial.Evaluaciones.Count,
                    Investigaciones = historial.Investigaciones.Count,
                    Experiencias = historial.Experiencias.Count
                }
            });
        }
    }
}
