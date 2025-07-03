using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly ICursoService _cursoService;
        private readonly ILogger<CursosController> _logger;

        public CursosController(ICursoService cursoService, ILogger<CursosController> logger)
        {
            _cursoService = cursoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los cursos
        /// </summary>
        /// <returns>Lista de cursos</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CursoDto>>> GetAll()
        {
            try
            {
                var cursos = await _cursoService.GetAllAsync();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los cursos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un curso por su ID
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <returns>Curso encontrado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> GetById(int id)
        {
            try
            {
                var curso = await _cursoService.GetByIdAsync(id);
                if (curso == null)
                {
                    return NotFound($"Curso con ID {id} no encontrado");
                }

                return Ok(curso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener curso con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los cursos de un docente específico
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <returns>Lista de cursos del docente</returns>
        [HttpGet("docente/{cedula}")]
        public async Task<ActionResult<IEnumerable<CursoDto>>> GetByDocente(string cedula)
        {
            try
            {
                var cursos = await _cursoService.GetByDocenteCedulaAsync(cedula);
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos del docente {Cedula}", cedula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene todos los cursos asociados a una solicitud de ascenso
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Lista de cursos de la solicitud</returns>
        [HttpGet("solicitud/{solicitudId}")]
        public async Task<ActionResult<IEnumerable<CursoDto>>> GetBySolicitud(Guid solicitudId)
        {
            try
            {
                var cursos = await _cursoService.GetBySolicitudIdAsync(solicitudId);
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cursos de la solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo curso con certificado y lo asocia automáticamente a una solicitud
        /// </summary>
        /// <param name="crearCursoDto">Datos del curso (incluye SolicitudId para asociación automática)</param>
        /// <param name="certificado">Archivo del certificado</param>
        /// <returns>Curso creado y asociado a la solicitud</returns>
        [HttpPost]
        public async Task<ActionResult<CursoDto>> Create([FromForm] CrearCursoDto crearCursoDto, IFormFile certificado)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cursoCreado = await _cursoService.CreateAsync(crearCursoDto, certificado);
                return CreatedAtAction(nameof(GetById), new { id = cursoCreado.Id }, cursoCreado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al crear curso");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear curso");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un curso existente
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <param name="actualizarCursoDto">Datos actualizados del curso</param>
        /// <param name="certificado">Nuevo archivo del certificado (opcional)</param>
        /// <returns>Curso actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CursoDto>> Update(int id, [FromForm] ActualizarCursoDto actualizarCursoDto, IFormFile? certificado = null)
        {
            try
            {
                if (id != actualizarCursoDto.Id)
                {
                    return BadRequest("El ID del curso no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cursoActualizado = await _cursoService.UpdateAsync(actualizarCursoDto, certificado);
                return Ok(cursoActualizado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar curso {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar curso {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un curso
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var eliminado = await _cursoService.DeleteAsync(id);
                if (!eliminado)
                {
                    return NotFound($"Curso con ID {id} no encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar curso {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si existe un curso
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <returns>True si existe, false en caso contrario</returns>
        [HttpHead("{id}")]
        public async Task<ActionResult> Exists(int id)
        {
            try
            {
                var existe = await _cursoService.ExistsAsync(id);
                return existe ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia del curso {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Asocia un curso a una solicitud de ascenso
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <param name="request">Datos de la solicitud</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("{id}/asociar-solicitud")]
        public async Task<ActionResult> AsociarASolicitud(int id, [FromBody] AsociarCursoSolicitudDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de entrada inválidos",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                // Crear el DTO con los datos del request
                var asociarDto = new AsociarCursoSolicitudDto
                {
                    CursoId = id,
                    SolicitudId = request.SolicitudId
                };

                var asociado = await _cursoService.AddToSolicitudAsync(asociarDto);
                if (!asociado)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo asociar el curso a la solicitud. Verifique que ambos existan y no estén ya asociados."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Curso asociado exitosamente a la solicitud"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar curso {CursoId} a solicitud {SolicitudId}", 
                    id, request.SolicitudId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Desasocia un curso de una solicitud de ascenso
        /// </summary>
        /// <param name="asociarDto">Datos de la desasociación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("desasociar")]
        public async Task<ActionResult> DesasociarDeSolicitud([FromBody] AsociarCursoSolicitudDto asociarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var desasociado = await _cursoService.RemoveFromSolicitudAsync(asociarDto);
                if (!desasociado)
                {
                    return BadRequest("No se pudo desasociar el curso de la solicitud.");
                }

                return Ok("Curso desasociado exitosamente de la solicitud");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar curso {CursoId} de solicitud {SolicitudId}", 
                    asociarDto.CursoId, asociarDto.SolicitudId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Descarga el certificado de un curso
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <returns>Archivo del certificado</returns>
        [HttpGet("{id}/certificado")]
        public async Task<ActionResult> DownloadCertificado(int id)
        {
            try
            {
                var (fileContent, contentType, fileName) = await _cursoService.DownloadCertificadoAsync(id);
                
                // Establecer el header Content-Disposition para forzar la descarga con el nombre correcto
                Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                
                return File(fileContent, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "Certificado no encontrado para curso {Id}", id);
                return NotFound("Certificado no encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar certificado del curso {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }



        /// <summary>
        /// Obtiene vista simplificada de todos los cursos
        /// </summary>
        /// <returns>Lista simplificada de cursos</returns>
        [HttpGet("ver")]
        public async Task<ActionResult<IEnumerable<VerCursoDto>>> GetAllSimplified()
        {
            try
            {
                var cursos = await _cursoService.GetAllSimplifiedAsync();
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener vista simplificada de cursos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene vista simplificada de cursos de un docente
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <returns>Lista simplificada de cursos del docente</returns>
        [HttpGet("ver/docente/{cedula}")]
        public async Task<ActionResult<IEnumerable<VerCursoDto>>> GetByDocenteSimplified(string cedula)
        {
            try
            {
                var cursos = await _cursoService.GetByDocenteCedulaSimplifiedAsync(cedula);
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener vista simplificada de cursos del docente {Cedula}", cedula);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Desasocia un curso de una solicitud (version para frontend con ID en ruta)
        /// </summary>
        /// <param name="id">ID del curso</param>
        /// <param name="dto">Datos de la solicitud</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpPost("{id}/desasociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarCursoDeSolicitud(int id, [FromBody] AsociarCursoSolicitudDto dto)
        {
            try
            {
                // Validar que el DTO tenga los datos necesarios
                if (dto == null || dto.SolicitudId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "SolicitudId inválido o no proporcionado"
                    });
                }

                // Usar el ID de la ruta y el SolicitudId del DTO
                var desasociarDto = new AsociarCursoSolicitudDto
                {
                    CursoId = id,
                    SolicitudId = dto.SolicitudId
                };

                var desasociado = await _cursoService.RemoveFromSolicitudAsync(desasociarDto);
                if (!desasociado)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo desasociar el curso de la solicitud"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Curso desasociado exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar curso {CursoId} de solicitud", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }
    }
}