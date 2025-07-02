using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestigacionesController : ControllerBase
    {
        private readonly IInvestigacionService _investigacionService;
        private readonly ILogger<InvestigacionesController> _logger;

        public InvestigacionesController(IInvestigacionService investigacionService, ILogger<InvestigacionesController> logger)
        {
            _investigacionService = investigacionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las investigaciones
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetAll()
        {
            var investigaciones = await _investigacionService.GetAllAsync();
            return Ok(investigaciones);
        }

        /// <summary>
        /// Obtiene una investigación por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var investigacion = await _investigacionService.GetByIdAsync(id);
                if (investigacion == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Investigación con ID {id} no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Investigación obtenida exitosamente",
                    data = investigacion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las investigaciones de un docente específico
        /// </summary>
        [HttpGet("docente/{cedula}")]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetByDocente(string cedula)
        {
            var investigaciones = await _investigacionService.GetByDocenteCedulaAsync(cedula);
            return Ok(investigaciones);
        }

        /// <summary>
        /// Obtiene las investigaciones asociadas a una solicitud específica
        /// </summary>
        [HttpGet("solicitud/{solicitudId}")]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetBySolicitud(Guid solicitudId)
        {
            var investigaciones = await _investigacionService.GetBySolicitudIdAsync(solicitudId);
            return Ok(investigaciones);
        }

        /// <summary>
        /// Crea una nueva investigación con archivo de informe (se asocia automáticamente a la solicitud)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InvestigacionDto>> Create([FromForm] CrearInvestigacionDto crearInvestigacionDto, IFormFile informe)
        {
            try
            {
                var investigacionCreada = await _investigacionService.CreateAsync(crearInvestigacionDto, informe);
                return CreatedAtAction(nameof(GetById), new { id = investigacionCreada.Id }, investigacionCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una investigación existente (datos y archivo)
        /// </summary>
        [HttpPut("{id}")]
        [RequestSizeLimit(25 * 1024 * 1024)]
        public async Task<IActionResult> UpdateInvestigacion(int id)
        {
            try
            {
                var form = await Request.ReadFormAsync();

                // Extraer campos del formulario
                var titulo = form["Titulo"];
                var fechaInicio = DateTime.Parse(form["FechaInicio"]);
                var fechaFinalizacion = DateTime.Parse(form["FechaFinalizacion"]);
                var rol = form["RolEnInvestigacion"];
                var meses = int.Parse(form["MesesDeInvestigacion"]);
                var docenteCedula = form["DocenteCedula"];

                var dto = new ActualizarInvestigacionDto
                {
                    Titulo = titulo,
                    FechaInicio = fechaInicio,
                    FechaFinalizacion = fechaFinalizacion,
                    RolEnInvestigacion = rol,
                    MesesDeInvestigacion = meses,
                    DocenteCedula = docenteCedula
                };

                // Archivo (opcional)
                var archivo = form.Files.FirstOrDefault();

                var result = await _investigacionService.UpdateAsync(id, dto, archivo);

                if (result == null)
                    return NotFound(new { success = false, message = "Investigación no encontrada" });

                return Ok(new
                {
                    success = true,
                    message = "Investigación actualizada exitosamente",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una investigación por ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var eliminada = await _investigacionService.DeleteAsync(id);
            if (!eliminada)
                return NotFound($"Investigación con ID {id} no encontrada");

            return NoContent();
        }

        /// <summary>
        /// Obtiene una vista simplificada de todas las investigaciones
        /// </summary>
        [HttpGet("ver")]
        public async Task<ActionResult<IEnumerable<VerInvestigacionDto>>> GetVerInvestigaciones()
        {
            var investigaciones = await _investigacionService.GetVerInvestigacionesAsync();
            return Ok(investigaciones);
        }

        /// <summary>
        /// Descarga el informe de una investigación
        /// </summary>
        [HttpGet("{id}/descargar-informe")]
        public async Task<ActionResult> DownloadInforme(int id)
        {
            try
            {
                var (fileContent, contentType, fileName) = await _investigacionService.DownloadInformeAsync(id);

                // Establecer el header Content-Disposition para forzar la descarga con el nombre correcto
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");

                return File(fileContent, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Asocia una investigación a una solicitud de ascenso
        /// </summary>
        /// <param name="id">ID de la investigación</param>
        /// <param name="request">Datos de la solicitud</param>
        /// <returns>Resultado de la asociación</returns>
        [HttpPost("{id}/asociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AsociarInvestigacionASolicitud(int id, [FromBody] AsociarInvestigacionSolicitudDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de entrada inválidos",
                        errors = errors
                    });
                }

                // Crear el DTO con los datos del request
                var asociarDto = new AsociarInvestigacionSolicitudDto
                {
                    InvestigacionId = id,
                    SolicitudId = request.SolicitudId
                };

                var result = await _investigacionService.AsociarInvestigacionASolicitudAsync(asociarDto);
                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo asociar la investigación a la solicitud. Verifique que ambos existan y no estén ya asociados."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Investigación asociada a la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar investigación {InvestigacionId} a solicitud {SolicitudId}", 
                    id, request.SolicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Desasocia una investigación de una solicitud de ascenso
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="investigacionId">ID de la investigación</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpDelete("desasociar-solicitud/{solicitudId}/{investigacionId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarInvestigacionDeSolicitud(Guid solicitudId, int investigacionId)
        {
            try
            {
                await _investigacionService.DesasociarInvestigacionDeSolicitudAsync(solicitudId, investigacionId);
                return Ok(new
                {
                    success = true,
                    message = "Investigación desasociada de la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar investigación {InvestigacionId} de solicitud {SolicitudId}", 
                    investigacionId, solicitudId);
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