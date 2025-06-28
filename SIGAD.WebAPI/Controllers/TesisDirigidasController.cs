using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TesisDirigidasController : ControllerBase
    {
        private readonly ITesisDirigidaService _service;
        private readonly ILogger<TesisDirigidasController> _logger;

        public TesisDirigidasController(ITesisDirigidaService service, ILogger<TesisDirigidasController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("docente/{cedula}")]
        public async Task<IActionResult> ObtenerPorDocente(string cedula)
        {
            var tesis = await _service.ObtenerPorDocenteAsync(cedula);
            return Ok(tesis);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateTesisDirigidaDto dto)
        {
            try
            {
                _logger.LogInformation("Intentando crear tesis dirigida para docente: {DocenteCedula}", dto.DocenteCedula);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Datos inválidos para crear tesis dirigida: {ModelState}", ModelState);
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos inválidos",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var nueva = await _service.CrearAsync(dto);

                _logger.LogInformation("Tesis dirigida creada exitosamente con ID: {TesisId}", nueva.Id);

                return CreatedAtAction(nameof(ObtenerPorDocente), new { cedula = nueva.DocenteCedula }, new
                {
                    success = true,
                    message = "Tesis dirigida creada exitosamente",
                    data = nueva
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tesis dirigida para docente: {DocenteCedula}", dto.DocenteCedula);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al crear la tesis dirigida",
                    error = ex.Message
                });
            }
        }

        [HttpPost("asociar")]
        public async Task<IActionResult> AsociarASolicitud(Guid solicitudId, int tesisId)
        {
            await _service.AsociarASolicitudAsync(solicitudId, tesisId);
            return Ok();
        }

        [HttpDelete("desasociar")]
        public async Task<IActionResult> DesasociarDeSolicitud(Guid solicitudId, int tesisId)
        {
            await _service.DesasociarDeSolicitudAsync(solicitudId, tesisId);
            return Ok();
        }

        [HttpGet("existe-por-hash/{hash}")]
        public async Task<IActionResult> ExistePorHash(string hash)
        {
            var existe = await _service.ExistePorHashAsync(hash);
            return Ok(existe);
        }
    }
}
//         }
//     }
// }
