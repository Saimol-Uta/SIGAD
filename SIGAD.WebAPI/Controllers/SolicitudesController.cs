using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _solicitudesService;
        private readonly ILogger<SolicitudesController> _logger;

        public SolicitudesController(GestionSolicitudesAppService solicitudesService, ILogger<SolicitudesController> logger)
        {
            _solicitudesService = solicitudesService;
            _logger = logger;
        }

        // GET: api/solicitudes
        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var solicitudes = await _solicitudesService.GetAllParaAdminAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las solicitudes");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/solicitudes/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var solicitud = await _solicitudesService.GetDetalleParaAdminAsync(id);
                return solicitud != null ? Ok(solicitud) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitud {SolicitudId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST: api/solicitudes
        [HttpPost]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudDto dto)
        {
            var docenteCedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(docenteCedula))
            {
                return Unauthorized("La cédula del docente no se encontró en el token.");
            }

            try
            {
                var id = await _solicitudesService.EnviarSolicitudConEvidenciaAsync(dto, docenteCedula);
                return CreatedAtAction(nameof(GetById), new { id }, new { SolicitudId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la solicitud para el docente {Cedula}", docenteCedula);
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        // PUT: api/solicitudes/{id}/aprobar
        [HttpPut("{id}/aprobar")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> Aprobar(Guid id, [FromBody] string observaciones)
        {
            try
            {
                await _solicitudesService.AprobarSolicitudAsync(id, observaciones ?? "");

                return Ok(new
                {
                    success = true,
                    message = "Solicitud aprobada exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Error de validación al aprobar solicitud {SolicitudId}: {Message}", id, ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Operación inválida al aprobar solicitud {SolicitudId}: {Message}", id, ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al aprobar solicitud {SolicitudId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al procesar la aprobación"
                });
            }
        }

        // PUT: api/solicitudes/{id}/rechazar
        [HttpPut("{id}/rechazar")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> Rechazar(Guid id, [FromBody] string observaciones)
        {
            try
            {
                // Validar que se proporcione una justificación obligatoria
                if (string.IsNullOrWhiteSpace(observaciones))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "La justificación es obligatoria para rechazar una solicitud",
                        field = "observaciones"
                    });
                }

                // Validar longitud mínima de la justificación
                if (observaciones.Trim().Length < 10)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "La justificación debe tener al menos 10 caracteres",
                        field = "observaciones"
                    });
                }

                await _solicitudesService.RechazarSolicitudAsync(id, observaciones.Trim());

                return Ok(new
                {
                    success = true,
                    message = "Solicitud rechazada exitosamente"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Error de validación al rechazar solicitud {SolicitudId}: {Message}", id, ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Operación inválida al rechazar solicitud {SolicitudId}: {Message}", id, ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al rechazar solicitud {SolicitudId}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor al procesar el rechazo"
                });
            }
        }

        // PUT: api/solicitudes/{id}/aprobar-comision
        [HttpPut("{id}/aprobar-comision")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> AprobarPorComision(Guid id, [FromBody] AprobacionRequest request)
        {
            try
            {
                await _solicitudesService.AprobarPorComisionAsync(id, request.Observaciones);
                return Ok(new { success = true, message = "Solicitud aprobada por Comisión Académica exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar por Comisión la solicitud {SolicitudId}: {Message}", id, ex.Message);

                // Obtener detalles del inner exception si existe
                var innerMessage = ex.InnerException?.Message ?? "Sin detalles adicionales";
                var fullMessage = $"Error: {ex.Message}. Detalles: {innerMessage}";

                return BadRequest(new { success = false, message = fullMessage });
            }
        }

        // PUT: api/solicitudes/{id}/aprobar-consejo
        [HttpPut("{id}/aprobar-consejo")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> AprobarPorConsejo(Guid id, [FromBody] AprobacionRequest request)
        {
            try
            {
                await _solicitudesService.AprobarPorConsejoAsync(id, request.Observaciones);
                return Ok(new { success = true, message = "Solicitud aprobada por Consejo Universitario exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar por Consejo la solicitud {SolicitudId}", id);
                return BadRequest(new { success = false, message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // PUT: api/solicitudes/{id}/finalizar
        [HttpPut("{id}/finalizar")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> FinalizarProceso(Guid id, [FromBody] AprobacionRequest request)
        {
            try
            {
                await _solicitudesService.FinalizarProcesoAsync(id, request.Observaciones);
                return Ok(new { success = true, message = "Proceso de ascenso finalizado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar el proceso de la solicitud {SolicitudId}", id);
                return BadRequest(new { success = false, message = $"Error interno del servidor: {ex.Message}" });
            }
        }

        // Endpoint de prueba sin autorización
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.Now });
        }

        // Endpoint de prueba con autorización básica
        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            var userClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            return Ok(new
            {
                message = "Autenticación exitosa",
                user = User.Identity?.Name,
                roles = User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                                  .Select(c => c.Value).ToArray(),
                allClaims = userClaims
            });
        }

        // GET: api/solicitudes/verif-solicitud-activa
        [HttpGet("verif-solicitud-activa")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> VerificarSolicitudActiva()
        {
            var docenteCedula = User.FindFirst("cedula")?.Value;
            if (string.IsNullOrEmpty(docenteCedula))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "No se pudo obtener la cédula del docente desde el token."
                });
            }

            try
            {
                var solicitud = await _solicitudesService.ObtenerBorradorActivoAsync(docenteCedula);

                if (solicitud == null)
                {
                    return Ok(new
                    {
                        tieneBorrador = false
                    });
                }

                return Ok(new
                {
                    tieneBorrador = true,
                    solicitudId = solicitud.Id,
                    fechaCreacion = solicitud.FechaCreacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar solicitud activa para docente {Cedula}", docenteCedula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        public class AprobacionRequest
        {
            public string Observaciones { get; set; } = string.Empty;
        }
        // GET: api/solicitudes/historial/{cedulaDocente}
        [HttpGet("historial/{cedulaDocente}")]
        [Authorize(Roles = "DOCENTE,ADMINISTRADOR")]
        public async Task<IActionResult> ObtenerHistorialPorCedula(string cedulaDocente)
        {
            try
            {
                var historial = await _solicitudesService.ObtenerHistorialPorDocenteAsync(cedulaDocente);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial del docente con cédula {Cedula}", cedulaDocente);
                return StatusCode(500, "Error interno del servidor");
            }
        }

    }

}