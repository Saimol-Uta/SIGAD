using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System.Security.Claims;

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
                // Validar que los datos requeridos estén presentes
                if (request.SolicitudId == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID de la solicitud es requerido." });
                }

                if (string.IsNullOrWhiteSpace(request.Justificacion))
                {
                    return BadRequest(new { message = "La justificación es requerida." });
                }

                // Obtener la cédula del docente desde el token
                var docenteCedula = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(docenteCedula))
                {
                    return Unauthorized(new { message = "La cédula del docente no se encontró en el token." });
                }

                var resultado = await _gestionSolicitudesService.PresentarApelacionAsync(
                    request.SolicitudId, 
                    request.Justificacion,
                    docenteCedula,
                    request.DocumentosAdjuntos);

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
                // Log más detallado del error
                var errorDetails = new
                {
                    message = "Error inesperado al presentar la apelación",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                };
                
                Console.WriteLine($"Error en PresentarApelacion: {ex}");
                return StatusCode(500, errorDetails);
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

        [HttpGet("detalle/{solicitudId}")]
        public async Task<IActionResult> GetApelacionDetalle(Guid solicitudId)
        {
            try
            {
                var detalle = await _gestionSolicitudesService.GetApelacionDetalleAsync(solicitudId);
                
                if (detalle == null)
                {
                    return NotFound(new { message = "No se encontró una apelación pendiente para esta solicitud" });
                }
                
                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        [HttpPost("resolver/{apelacionId}")]
        public async Task<IActionResult> ResolverApelacion(int apelacionId, [FromBody] ResolverApelacionDto dto)
        {
            try
            {
                Console.WriteLine($"[API] POST /api/apelaciones/resolver/{{apelacionId}} llamado. apelacionId={apelacionId}");
                Console.WriteLine($"Payload recibido: {{ Aceptada = {dto?.Aceptada}, ObservacionesComision = '{dto?.ObservacionesComision}' }}");

                // Obtener la cédula del admin desde el token
                var adminCedula = User.FindFirst("cedula")?.Value;
                Console.WriteLine($"Cedula admin extraída del token: {adminCedula}");
                if (string.IsNullOrEmpty(adminCedula))
                {
                    Console.WriteLine("No se encontró la cédula del administrador en el token.");
                    return Unauthorized(new { message = "La cédula del administrador no se encontró en el token." });
                }

                var resultado = await _gestionSolicitudesService.ResolverApelacionAsync(apelacionId, dto, adminCedula);

                Console.WriteLine($"Resultado de ResolverApelacionAsync: success={resultado.success}, message={resultado.message}");

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
                var errorDetails = new
                {
                    message = "Error inesperado al resolver la apelación",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                };
                Console.WriteLine($"Error en ResolverApelacion: {ex}");
                return StatusCode(500, errorDetails);
            }
        }
    }

    public class CrearApelacionRequestDto
    {
        public Guid SolicitudId { get; set; }
        public string Justificacion { get; set; } = "";
        public IFormFileCollection? DocumentosAdjuntos { get; set; }
    }
}
