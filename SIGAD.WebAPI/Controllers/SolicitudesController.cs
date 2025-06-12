/*
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _solicitudesService;

        public SolicitudesController(GestionSolicitudesAppService solicitudesService)
        {
            _solicitudesService = solicitudesService;
        }

        // GET /api/solicitudes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _solicitudesService.GetAllSolicitudesAsync();
            return Ok(solicitudes);
        }

        // POST /api/solicitudes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearSolicitudDto dto)
        {
            var docenteCedula = "1234567890"; // Temporal
            var id = await _solicitudesService.CrearSolicitudAsync(dto, docenteCedula);
            return Ok(new { Id = id });
        }

        // PUT /api/solicitudes/{id}/aprobar
        [HttpPut("{id}/aprobar")]
        public async Task<IActionResult> Aprobar(Guid id)
        {
            await _solicitudesService.AprobarSolicitudAsync(id);
            return Ok();
        }

        // PUT /api/solicitudes/{id}/rechazar
        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid id, [FromBody] string observaciones)
        {
            await _solicitudesService.RechazarSolicitudAsync(id, observaciones);
            return Ok();
        }
    }
}
*/
