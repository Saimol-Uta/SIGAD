using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApelacionesController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _gestionSolicitudesService;

        public ApelacionesController(GestionSolicitudesAppService gestionSolicitudesService)
        {
            _gestionSolicitudesService = gestionSolicitudesService;
        }

        [HttpPost]
        public async Task<IActionResult> PresentarApelacion([FromForm] CrearApelacionRequestDto request)
        {
            try
            {
                // Obtener el usuario actual (implementar según tu sistema de autenticación)
                var currentUser = User.Identity?.Name ?? "";

                var resultado = await _gestionSolicitudesService.PresentarApelacionAsync(
                    request.SolicitudId, 
                    request.Justificacion,
                    currentUser,
                    request.DocumentoAdjunto);

                if (resultado.success)
                {
                    return Ok(new { message = resultado.message });
                }
                else
                {
                    return BadRequest(new { message = resultado.message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpGet("solicitud/{solicitudId}")]
        public async Task<IActionResult> GetApelacionesBySolicitud(Guid solicitudId)
        {
            try
            {
                var apelaciones = await _gestionSolicitudesService.GetApelacionesBySolicitudAsync(solicitudId);
                return Ok(apelaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }

    public class CrearApelacionRequestDto
    {
        public Guid SolicitudId { get; set; }
        public string Justificacion { get; set; } = "";
        public IFormFile? DocumentoAdjunto { get; set; }
    }
}
