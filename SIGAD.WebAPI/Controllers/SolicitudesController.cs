//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SIGAD.Application.DTOs;
//using SIGAD.Application.Services;
//using System;
//using System.Threading.Tasks;
//using System.Security.Claims; // Agregar esta línea al inicio del archivo

//namespace SIGAD.WebAPI.Controllers
//{    [ApiController]
//    [Route("api/[controller]")]
//    public class SolicitudesController : ControllerBase
//    {
//        // TODO: Implementar GestionSolicitudesAppService
//        // private readonly GestionSolicitudesAppService _solicitudesService;
//        private readonly ILogger<SolicitudesController> _logger;

//        public SolicitudesController(/*GestionSolicitudesAppService solicitudesService,*/ ILogger<SolicitudesController> logger)
//        {
//            // _solicitudesService = solicitudesService;
//            _logger = logger;
//        }

//        // GET: api/solicitudes
//        [HttpGet]

//        public async Task<IActionResult> GetAll()
//        {
//            var solicitudes = await _solicitudesService.GetAllParaAdminAsync();
//            return Ok(solicitudes);
//        }

//        // GET: api/solicitudes/{id}
//        [HttpGet("{id}")]

//        public async Task<IActionResult> GetById(Guid id)
//        {
//            var solicitud = await _solicitudesService.GetDetalleParaAdminAsync(id);
//            return solicitud != null ? Ok(solicitud) : NotFound();
//        }

//        // POST: api/solicitudes
//        [HttpPost]
//        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudDto dto)
//        {
//            var docenteCedula = User.FindFirst("cedula")?.Value;
//            if (string.IsNullOrEmpty(docenteCedula))
//            {
//                return Unauthorized("La cédula del docente no se encontró en el token.");
//            }

//            try
//            {
//                var id = await _solicitudesService.EnviarSolicitudConEvidenciaAsync(dto, docenteCedula);
//                return CreatedAtAction(nameof(GetById), new { id }, new { SolicitudId = id });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error al crear la solicitud para el docente {Cedula}", docenteCedula);
//                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
//            }
//        }

//        // PUT: api/solicitudes/{id}/aprobar
//        [HttpPut("{id}/aprobar")]
//        public async Task<IActionResult> Aprobar(Guid id, [FromBody] string observaciones)
//        {
//            try
//            {
//                await _solicitudesService.AprobarSolicitudAsync(id, observaciones ?? "");
                
//                return Ok(new 
//                { 
//                    success = true, 
//                    message = "Solicitud aprobada exitosamente" 
//                });
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning("Error de validación al aprobar solicitud {SolicitudId}: {Message}", id, ex.Message);
//                return BadRequest(new 
//                { 
//                    success = false, 
//                    message = ex.Message 
//                });
//            }
//            catch (InvalidOperationException ex)
//            {
//                _logger.LogWarning("Operación inválida al aprobar solicitud {SolicitudId}: {Message}", id, ex.Message);
//                return BadRequest(new 
//                { 
//                    success = false, 
//                    message = ex.Message 
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error inesperado al aprobar solicitud {SolicitudId}", id);
//                return StatusCode(500, new 
//                { 
//                    success = false, 
//                    message = "Error interno del servidor al procesar la aprobación" 
//                });
//            }
//        }

//        // PUT: api/solicitudes/{id}/rechazar
//        [HttpPut("{id}/rechazar")]
//        public async Task<IActionResult> Rechazar(Guid id, [FromBody] string observaciones)
//        {
//            try
//            {
//                // Validar que se proporcione una justificación obligatoria
//                if (string.IsNullOrWhiteSpace(observaciones))
//                {
//                    return BadRequest(new 
//                    { 
//                        success = false, 
//                        message = "La justificación es obligatoria para rechazar una solicitud",
//                        field = "observaciones"
//                    });
//                }

//                // Validar longitud mínima de la justificación
//                if (observaciones.Trim().Length < 10)
//                {
//                    return BadRequest(new 
//                    { 
//                        success = false, 
//                        message = "La justificación debe tener al menos 10 caracteres",
//                        field = "observaciones"
//                    });
//                }

//                await _solicitudesService.RechazarSolicitudAsync(id, observaciones.Trim());
                
//                return Ok(new 
//                { 
//                    success = true, 
//                    message = "Solicitud rechazada exitosamente" 
//                });
//            }
//            catch (ArgumentException ex)
//            {
//                _logger.LogWarning("Error de validación al rechazar solicitud {SolicitudId}: {Message}", id, ex.Message);
//                return BadRequest(new 
//                { 
//                    success = false, 
//                    message = ex.Message 
//                });
//            }
//            catch (InvalidOperationException ex)
//            {
//                _logger.LogWarning("Operación inválida al rechazar solicitud {SolicitudId}: {Message}", id, ex.Message);
//                return BadRequest(new 
//                { 
//                    success = false, 
//                    message = ex.Message 
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error inesperado al rechazar solicitud {SolicitudId}", id);
//                return StatusCode(500, new 
//                { 
//                    success = false, 
//                    message = "Error interno del servidor al procesar el rechazo" 
//                });
//            }
//        }

//        // Endpoint de prueba sin autorización
//        [HttpGet("test")]
//        public IActionResult Test()
//        {
//            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.Now });
//        }

//        // Endpoint de prueba con autorización básica
//        [HttpGet("test-auth")]
//        public IActionResult TestAuth()
//        {
//            var userInfo = new
//            {
//                IsAuthenticated = User.Identity.IsAuthenticated,
//                Name = User.Identity.Name,
//                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
//                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
//            };

//            return Ok(userInfo);
//        }


//        // GET: api/solicitudes/verif-solicitud-activa
//        [HttpGet("verif-solicitud-activa")]
//        public async Task<IActionResult> VerificarSolicitudActiva()
//        {
//            var docenteCedula = User.FindFirst("cedula")?.Value;
//            if (string.IsNullOrEmpty(docenteCedula))
//            {
//                return Unauthorized(new
//                {
//                    success = false,
//                    message = "No se pudo obtener la cédula del docente desde el token."
//                });
//            }

//            var solicitud = await _solicitudesService.ObtenerBorradorActivoAsync(docenteCedula);

//            if (solicitud == null)
//            {
//                return Ok(new
//                {
//                    tieneBorrador = false
//                });
//            }

//            return Ok(new
//            {
//                tieneBorrador = true,
//                solicitudId = solicitud.Id,
//                fechaCreacion = solicitud.FechaCreacion
//            });
//        }

//    }
//}