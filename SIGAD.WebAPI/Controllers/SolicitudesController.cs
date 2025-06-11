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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VerSolicitudDto>>> GetAll()
        {
            // Llama al servicio (que por ahora devuelve datos vacíos)
            var solicitudes = await _solicitudesService.GetAllSolicitudesAsync();
            return Ok(solicitudes);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearSolicitudDto dto)
        {
            // Asumimos que obtenemos la cédula del usuario logueado. Por ahora, la ponemos fija.
            var docenteCedula = "1234567890";
            var id = await _solicitudesService.CrearSolicitudAsync(dto, docenteCedula);
            return CreatedAtAction(nameof(GetAll), new { id }, id); // Temporal
        }

        [HttpPut("{id}/aprobar")]
        public async Task<IActionResult> Aprobar(Guid id)
        {
            await _solicitudesService.AprobarSolicitudAsync(id);
            return NoContent(); // 204 No Content
        }

        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid id)
        {
            await _solicitudesService.RechazarSolicitudAsync(id);
            return NoContent(); // 204 No Content
        }
    }
}
