using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Services;


namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    public class ReportesController : ControllerBase
    {
        private readonly ReporteBackendService _reporteService;

        public ReportesController(ReporteBackendService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("solicitudes-por-estado")]
        public async Task<IActionResult> GetPorEstado()
        {
            var data = await _reporteService.ObtenerSolicitudesPorEstado();
            return Ok(data);
        }

        [HttpGet("solicitudes-por-nivel")]
        public async Task<IActionResult> GetPorNivel()
        {
            var data = await _reporteService.ObtenerSolicitudesPorNivel();
            return Ok(data);
        }

        [HttpGet("solicitudes-por-mes/{anio}")]
        public async Task<IActionResult> GetPorMes(int anio)
        {
            var data = await _reporteService.ObtenerSolicitudesPorMes(anio);
            return Ok(data);
        }
    }
}
