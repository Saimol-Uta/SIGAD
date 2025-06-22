using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TesisDirigidasController : ControllerBase
    {
        private readonly TesisDirigidaService _service;

        public TesisDirigidasController(TesisDirigidaService service)
        {
            _service = service;
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
            var nueva = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorDocente), new { cedula = nueva.DocenteCedula }, nueva);
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
