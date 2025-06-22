using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Services;
using SIGAD.Application.DTOs;
using System.Security.Claims;

namespace SIGAD.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AscensosController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _solicitudService;
        private readonly ILogger<AscensosController> _logger;

        public AscensosController(
            GestionSolicitudesAppService solicitudService,
            ILogger<AscensosController> logger)
        {
            _solicitudService = solicitudService;
            _logger = logger;
        }

        /// <summary>
        /// Verifica si el docente tiene una solicitud activa
        /// </summary>
        [HttpGet("verificar-activa")]
        public async Task<IActionResult> VerificarSolicitudActiva()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var solicitudActiva = await _solicitudService.TieneSolicitudActivaAsync(cedula);
                return Ok(new
                {
                    success = true,
                    tieneSolicitudActiva = solicitudActiva,
                    message = solicitudActiva ? "El docente tiene una solicitud activa" : "El docente no tiene solicitudes activas"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar solicitud activa");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }        /// <summary>
                 /// Crea una nueva solicitud de ascenso (borrador)
                 /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudDto request)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var solicitud = await _solicitudService.CrearSolicitudSimpleAsync(cedula, request.RangoSolicitadoId);
                return Ok(new
                {
                    success = true,
                    message = "Solicitud creada exitosamente",
                    data = new
                    {
                        id = solicitud.Id,
                        estado = solicitud.Estado.ToString(),
                        fechaCreacion = solicitud.FechaCreacion,
                        rangoSolicitadoId = solicitud.RangoSolicitadoId
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al crear solicitud para cedula: {Cedula}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Docente no encontrado: {Cedula}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el borrador activo del docente
        /// </summary>
        [HttpGet("borrador")]
        public async Task<IActionResult> GetBorradorActivo()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var borrador = await _solicitudService.ObtenerBorradorActivoAsync(cedula);
                if (borrador == null)
                {
                    return Ok(new { success = true, message = "No hay borrador activo", data = (object?)null });
                }

                return Ok(new
                {
                    success = true,
                    message = "Borrador obtenido exitosamente",
                    data = new
                    {
                        id = borrador.Id,
                        estado = borrador.Estado.ToString(),
                        fechaCreacion = borrador.FechaCreacion,
                        rangoSolicitadoId = borrador.RangoSolicitadoId
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener borrador activo");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Envía una solicitud para evaluación
        /// </summary>
        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudDto request)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var solicitudId = await _solicitudService.EnviarSolicitudConEvidenciaAsync(request, cedula);
                return Ok(new
                {
                    success = true,
                    message = "Solicitud enviada exitosamente",
                    solicitudId = solicitudId
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al enviar solicitud");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Docente no encontrado: {Cedula}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}
