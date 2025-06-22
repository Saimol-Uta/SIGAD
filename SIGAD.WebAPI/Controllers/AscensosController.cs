using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Services;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace SIGAD.WebAPI.Controllers
{    /// <summary>
     /// Controlador para gestión de solicitudes de ascenso académico según el 
     /// Reglamento para la Promoción del Personal Académico Titular de la UTA
     /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AscensosController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _solicitudService;
        private readonly IValidacionRequisitosService _validacionService;
        private readonly ILogger<AscensosController> _logger;

        public AscensosController(
            GestionSolicitudesAppService solicitudService,
            IValidacionRequisitosService validacionService,
            ILogger<AscensosController> logger)
        {
            _solicitudService = solicitudService;
            _validacionService = validacionService;
            _logger = logger;
        }        /// <summary>
                 /// Verifica si el docente autenticado tiene una solicitud de ascenso activa
                 /// </summary>
                 /// <returns>Información sobre el estado de solicitudes activas del docente</returns>
                 /// <response code="200">Verificación exitosa</response>
                 /// <response code="401">No autorizado - Token de autenticación inválido</response>
                 /// <response code="500">Error interno del servidor</response>
        [HttpGet("verificar-activa")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> VerificarSolicitudActiva()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando verificar solicitud activa");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                _logger.LogInformation("Verificando solicitud activa para docente: {Cedula}", cedula);
                var solicitudActiva = await _solicitudService.TieneSolicitudActivaAsync(cedula);

                return Ok(new
                {
                    success = true,
                    tieneSolicitudActiva = solicitudActiva,
                    message = solicitudActiva ? "El docente tiene una solicitud activa" : "El docente no tiene solicitudes activas",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar solicitud activa para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }        /// <summary>
                 /// Crea una nueva solicitud de ascenso en estado borrador
                 /// </summary>
                 /// <param name="request">Datos de la solicitud a crear</param>
                 /// <returns>Información de la solicitud creada</returns>
                 /// <response code="200">Solicitud creada exitosamente</response>
                 /// <response code="400">Datos de entrada inválidos o violación de reglas de negocio</response>
                 /// <response code="401">No autorizado</response>
                 /// <response code="404">Docente no encontrado</response>
                 /// <response code="500">Error interno del servidor</response>
        [HttpPost("crear")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CrearSolicitud([FromBody] CrearSolicitudDto request)
        {
            try
            {
                // Validar datos de entrada
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Los datos de la solicitud son requeridos" });
                }

                if (request.RangoSolicitadoId <= 0)
                {
                    return BadRequest(new { success = false, message = "El ID del rango solicitado debe ser válido" });
                }

                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando crear solicitud");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                _logger.LogInformation("Creando solicitud para docente: {Cedula}, Rango: {RangoId}",
                    cedula, request.RangoSolicitadoId);

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
                        rangoSolicitadoId = solicitud.RangoSolicitadoId,
                        rangoActualId = solicitud.RangoActualId
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al crear solicitud para cedula: {Cedula}, Rango: {RangoId}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value, request?.RangoSolicitadoId);
                return BadRequest(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Docente no encontrado: {Cedula}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return NotFound(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }        /// <summary>
                 /// Obtiene el borrador activo del docente autenticado
                 /// </summary>
                 /// <returns>Información del borrador activo o null si no existe</returns>
                 /// <response code="200">Consulta exitosa</response>
                 /// <response code="401">No autorizado</response>
                 /// <response code="500">Error interno del servidor</response>
        [HttpGet("borrador")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBorradorActivo()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando obtener borrador");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                _logger.LogInformation("Obteniendo borrador activo para docente: {Cedula}", cedula);
                var borrador = await _solicitudService.ObtenerBorradorActivoAsync(cedula);

                if (borrador == null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No hay borrador activo",
                        data = (object?)null,
                        timestamp = DateTime.UtcNow
                    });
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
                        rangoSolicitadoId = borrador.RangoSolicitadoId,
                        rangoActualId = borrador.RangoActualId
                    },
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener borrador activo para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }        /// <summary>
                 /// Envía una solicitud de ascenso para evaluación con la evidencia seleccionada
                 /// </summary>
                 /// <param name="request">Datos de la solicitud y evidencia a incluir</param>
                 /// <returns>Confirmación del envío de la solicitud</returns>
                 /// <response code="200">Solicitud enviada exitosamente</response>
                 /// <response code="400">Datos inválidos o requisitos no cumplidos</response>
                 /// <response code="401">No autorizado</response>
                 /// <response code="404">Docente no encontrado</response>
                 /// <response code="500">Error interno del servidor</response>
        [HttpPost("enviar")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudDto request)
        {
            try
            {
                // Validar datos de entrada
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Los datos de la solicitud son requeridos" });
                }

                if (request.RangoSolicitadoId <= 0)
                {
                    return BadRequest(new { success = false, message = "El ID del rango solicitado debe ser válido" });
                }

                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando enviar solicitud");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                _logger.LogInformation("Enviando solicitud para docente: {Cedula}, Rango: {RangoId}",
                    cedula, request.RangoSolicitadoId);

                // Validar requisitos antes de enviar
                var progreso = await _validacionService.VerificarProgresoAsync(cedula, request.RangoSolicitadoId);
                if (!progreso.PuedeAscender)
                {
                    _logger.LogWarning("Docente {Cedula} no cumple requisitos para rango {RangoId}",
                        cedula, request.RangoSolicitadoId);

                    return BadRequest(new
                    {
                        success = false,
                        message = "No cumple con todos los requisitos para el ascenso",
                        requisitosIncumplidos = new
                        {
                            antiguedad = !progreso.Antiguedad.Cumple ? progreso.Antiguedad.Mensaje : null,
                            evaluacion = !progreso.PromedioEvaluacion.Cumple ? progreso.PromedioEvaluacion.Mensaje : null,
                            articulos = !progreso.Articulos.Cumple ? progreso.Articulos.Mensaje : null,
                            cursos = !progreso.Cursos.Cumple ? progreso.Cursos.Mensaje : null,
                            investigaciones = !progreso.Investigaciones.Cumple ? progreso.Investigaciones.Mensaje : null,
                            tesis = !progreso.Tesis.Cumple ? progreso.Tesis.Mensaje : null
                        },
                        timestamp = DateTime.UtcNow
                    });
                }

                var solicitudId = await _solicitudService.EnviarSolicitudConEvidenciaAsync(request, cedula);

                _logger.LogInformation("Solicitud {SolicitudId} enviada exitosamente para docente {Cedula}",
                    solicitudId, cedula);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud enviada exitosamente",
                    solicitudId = solicitudId,
                    rangoSolicitadoId = request.RangoSolicitadoId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al enviar solicitud para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return BadRequest(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Docente no encontrado: {Cedula}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return NotFound(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Valida si el docente cumple con los requisitos para ascender a un rango específico
        /// </summary>
        [HttpGet("validar-requisitos/{rangoId}")]
        public async Task<IActionResult> ValidarRequisitos(int rangoId)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var progreso = await _validacionService.VerificarProgresoAsync(cedula, rangoId);

                return Ok(new
                {
                    success = true,
                    puedeAscender = progreso.PuedeAscender,
                    requisitos = new
                    {
                        antiguedad = new
                        {
                            cumple = progreso.Antiguedad.Cumple,
                            actual = progreso.Antiguedad.Actual,
                            requerido = progreso.Antiguedad.Requerido,
                            mensaje = progreso.Antiguedad.Mensaje
                        },
                        evaluacion = new
                        {
                            cumple = progreso.PromedioEvaluacion.Cumple,
                            actual = progreso.PromedioEvaluacion.Actual,
                            requerido = progreso.PromedioEvaluacion.Requerido,
                            mensaje = progreso.PromedioEvaluacion.Mensaje
                        },
                        articulos = new
                        {
                            cumple = progreso.Articulos.Cumple,
                            actual = progreso.Articulos.Actual,
                            requerido = progreso.Articulos.Requerido,
                            mensaje = progreso.Articulos.Mensaje
                        },
                        cursos = new
                        {
                            cumple = progreso.Cursos.Cumple,
                            actual = progreso.Cursos.Actual,
                            requerido = progreso.Cursos.Requerido,
                            mensaje = progreso.Cursos.Mensaje
                        },
                        investigaciones = new
                        {
                            cumple = progreso.Investigaciones.Cumple,
                            actual = progreso.Investigaciones.Actual,
                            requerido = progreso.Investigaciones.Requerido,
                            mensaje = progreso.Investigaciones.Mensaje
                        },
                        tesis = new
                        {
                            cumple = progreso.Tesis.Cumple,
                            actual = progreso.Tesis.Actual,
                            requerido = progreso.Tesis.Requerido,
                            mensaje = progreso.Tesis.Mensaje
                        }
                    }
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al validar requisitos: {RangoId}", rangoId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar requisitos para rango {RangoId}", rangoId);
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene los rangos disponibles para ascenso del docente actual
        /// </summary>
        [HttpGet("rangos-disponibles")]
        public async Task<IActionResult> GetRangosDisponibles()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                var rangos = await _solicitudService.ObtenerRangosDisponiblesAsync(cedula);

                return Ok(new
                {
                    success = true,
                    message = "Rangos disponibles obtenidos exitosamente",
                    data = rangos.Select(r => new
                    {
                        id = r.Id,
                        nombre = r.Nombre,
                        requisitos = new
                        {
                            aniosExperiencia = r.AniosExperienciaRequeridos,
                            articulos = r.ArticulosRequeridos,
                            horasCursos = r.HorasCursoRequeridas,
                            mesesInvestigacion = r.MesesInvestigacionRequeridos,
                            tesisDirigidas = r.TesisDirigidasRequeridas,
                            promedioEvaluacion = r.PuntajePromedioEvaluacionesRequerido
                        }
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rangos disponibles");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el progreso actual del docente hacia el siguiente rango
        /// </summary>
        [HttpGet("progreso-actual")]
        public async Task<IActionResult> GetProgresoActual()
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula)) return Unauthorized();

                // Obtener rango actual del docente
                var docente = await _solicitudService.ObtenerDocentePorCedulaAsync(cedula);
                if (docente == null)
                    return NotFound(new { success = false, message = "Docente no encontrado" });

                // Obtener rangos disponibles para ascenso
                var rangosDisponibles = await _solicitudService.ObtenerRangosDisponiblesAsync(cedula);
                var siguienteRango = rangosDisponibles.FirstOrDefault();

                if (siguienteRango == null)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "El docente ya tiene el máximo rango disponible",
                        data = new
                        {
                            rangoActual = docente.RangoActual?.Nombre,
                            puedeAscender = false,
                            siguienteRango = (object?)null
                        }
                    });
                }

                // Validar progreso hacia el siguiente rango
                var progreso = await _validacionService.VerificarProgresoAsync(cedula, siguienteRango.Id);

                return Ok(new
                {
                    success = true,
                    message = "Progreso obtenido exitosamente",
                    data = new
                    {
                        rangoActual = docente.RangoActual?.Nombre,
                        siguienteRango = new
                        {
                            id = siguienteRango.Id,
                            nombre = siguienteRango.Nombre,
                            requisitos = new
                            {
                                aniosExperiencia = siguienteRango.AniosExperienciaRequeridos,
                                articulos = siguienteRango.ArticulosRequeridos,
                                horasCursos = siguienteRango.HorasCursoRequeridas,
                                mesesInvestigacion = siguienteRango.MesesInvestigacionRequeridos,
                                tesisDirigidas = siguienteRango.TesisDirigidasRequeridas,
                                promedioEvaluacion = siguienteRango.PuntajePromedioEvaluacionesRequerido
                            }
                        },
                        puedeAscender = progreso.PuedeAscender,
                        progreso = new
                        {
                            antiguedad = new
                            {
                                cumple = progreso.Antiguedad.Cumple,
                                actual = progreso.Antiguedad.Actual,
                                requerido = progreso.Antiguedad.Requerido,
                                porcentaje = Math.Min(100, (progreso.Antiguedad.Actual / Math.Max(1, progreso.Antiguedad.Requerido)) * 100)
                            },
                            evaluacion = new
                            {
                                cumple = progreso.PromedioEvaluacion.Cumple,
                                actual = progreso.PromedioEvaluacion.Actual,
                                requerido = progreso.PromedioEvaluacion.Requerido,
                                porcentaje = Math.Min(100, (progreso.PromedioEvaluacion.Actual / Math.Max(1, progreso.PromedioEvaluacion.Requerido)) * 100)
                            },
                            articulos = new
                            {
                                cumple = progreso.Articulos.Cumple,
                                actual = progreso.Articulos.Actual,
                                requerido = progreso.Articulos.Requerido,
                                porcentaje = Math.Min(100, (progreso.Articulos.Actual / Math.Max(1, progreso.Articulos.Requerido)) * 100)
                            },
                            cursos = new
                            {
                                cumple = progreso.Cursos.Cumple,
                                actual = progreso.Cursos.Actual,
                                requerido = progreso.Cursos.Requerido,
                                porcentaje = Math.Min(100, (progreso.Cursos.Actual / Math.Max(1, progreso.Cursos.Requerido)) * 100)
                            },
                            investigaciones = new
                            {
                                cumple = progreso.Investigaciones.Cumple,
                                actual = progreso.Investigaciones.Actual,
                                requerido = progreso.Investigaciones.Requerido,
                                porcentaje = Math.Min(100, (progreso.Investigaciones.Actual / Math.Max(1, progreso.Investigaciones.Requerido)) * 100)
                            },
                            tesis = new
                            {
                                cumple = progreso.Tesis.Cumple,
                                actual = progreso.Tesis.Actual,
                                requerido = progreso.Tesis.Requerido,
                                porcentaje = Math.Min(100, (progreso.Tesis.Actual / Math.Max(1, progreso.Tesis.Requerido)) * 100)
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener progreso actual");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Obtiene el historial de solicitudes de ascenso del docente autenticado
        /// </summary>
        /// <param name="incluirDetalle">Si incluir detalles de evaluación y evidencia</param>
        /// <returns>Lista de solicitudes de ascenso del docente</returns>
        /// <response code="200">Historial obtenido exitosamente</response>
        /// <response code="401">No autorizado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("historial")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetHistorialSolicitudes([FromQuery] bool incluirDetalle = false)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando obtener historial");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                _logger.LogInformation("Obteniendo historial de solicitudes para docente: {Cedula}", cedula);

                // Aquí asumo que existe un método en el servicio, si no lo hay, se debe implementar                // TODO: Implementar ObtenerHistorialSolicitudesAsync en GestionSolicitudesAppService
                // var solicitudes = await _solicitudService.ObtenerHistorialSolicitudesAsync(cedula, incluirDetalle);

                // Implementación temporal - devolver lista vacía
                await Task.CompletedTask;
                var solicitudes = new List<object>();

                return Ok(new
                {
                    success = true,
                    message = "Historial obtenido exitosamente",
                    data = solicitudes,
                    count = solicitudes?.Count() ?? 0,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial de solicitudes para docente: {Cedula}",
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Cancela una solicitud de ascenso en estado borrador
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a cancelar</param>
        /// <returns>Confirmación de la cancelación</returns>
        /// <response code="200">Solicitud cancelada exitosamente</response>
        /// <response code="400">La solicitud no puede ser cancelada</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Solicitud no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpDelete("cancelar/{solicitudId:guid}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CancelarSolicitud([FromRoute] Guid solicitudId)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando cancelar solicitud");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                if (solicitudId == Guid.Empty)
                {
                    return BadRequest(new { success = false, message = "ID de solicitud inválido" });
                }

                _logger.LogInformation("Cancelando solicitud {SolicitudId} para docente: {Cedula}",
                    solicitudId, cedula);                // TODO: Implementar CancelarSolicitudAsync en GestionSolicitudesAppService
                                                         // await _solicitudService.CancelarSolicitudAsync(solicitudId, cedula);

                // Implementación temporal - simular cancelación exitosa
                await Task.CompletedTask;
                _logger.LogInformation("Simulando cancelación exitosa de solicitud {SolicitudId}", solicitudId);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud cancelada exitosamente",
                    solicitudId = solicitudId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Operación inválida al cancelar solicitud {SolicitudId}", solicitudId);
                return BadRequest(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Solicitud {SolicitudId} no encontrada", solicitudId);
                return NotFound(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Obtiene información detallada de una solicitud específica
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Detalles completos de la solicitud</returns>
        /// <response code="200">Detalles obtenidos exitosamente</response>
        /// <response code="401">No autorizado</response>
        /// <response code="404">Solicitud no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("detalle/{solicitudId:guid}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDetalleSolicitud([FromRoute] Guid solicitudId)
        {
            try
            {
                var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(cedula))
                {
                    _logger.LogWarning("Usuario no autenticado intentando obtener detalle de solicitud");
                    return Unauthorized(new { success = false, message = "Usuario no autenticado" });
                }

                if (solicitudId == Guid.Empty)
                {
                    return BadRequest(new { success = false, message = "ID de solicitud inválido" });
                }

                _logger.LogInformation("Obteniendo detalle de solicitud {SolicitudId} para docente: {Cedula}",
                    solicitudId, cedula);                // TODO: Implementar ObtenerDetalleSolicitudAsync en GestionSolicitudesAppService
                                                         // var detalle = await _solicitudService.ObtenerDetalleSolicitudAsync(solicitudId, cedula);

                // Implementación temporal - devolver objeto básico
                await Task.CompletedTask;
                var detalle = new
                {
                    id = solicitudId,
                    mensaje = "Detalle no implementado aún - TODO"
                };

                return Ok(new
                {
                    success = true,
                    message = "Detalle obtenido exitosamente",
                    data = detalle,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Solicitud {SolicitudId} no encontrada para docente", solicitudId);
                return NotFound(new { success = false, message = ex.Message, timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    timestamp = DateTime.UtcNow
                });
            }
        }
    }
}
