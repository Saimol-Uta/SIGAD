using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ExperienciasLaboralesController : ControllerBase
    {
        private readonly IExperienciaLaboralService _experienciaService;
        private readonly ILogger<ExperienciasLaboralesController> _logger;

        public ExperienciasLaboralesController(
            IExperienciaLaboralService experienciaService,
            ILogger<ExperienciasLaboralesController> logger)
        {
            _experienciaService = experienciaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las experiencias laborales
        /// </summary>
        /// <returns>Lista de experiencias laborales</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExperienciaLaboralDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllExperiencias()
        {
            try
            {
                var experiencias = await _experienciaService.GetAllExperienciasAsync();
                return Ok(new
                {
                    success = true,
                    message = "Experiencias laborales obtenidas exitosamente",
                    data = experiencias,
                    count = experiencias.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las experiencias laborales");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una experiencia laboral específica por ID
        /// </summary>
        /// <param name="id">ID de la experiencia laboral</param>
        /// <returns>Datos de la experiencia laboral</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExperienciaLaboralDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExperienciaById(int id)
        {
            try
            {
                var experiencia = await _experienciaService.GetExperienciaByIdAsync(id);
                if (experiencia == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Experiencia laboral no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Experiencia laboral obtenida exitosamente",
                    data = experiencia
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la experiencia laboral con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las experiencias laborales de un docente específico
        /// </summary>
        /// <param name="docenteCedula">Cédula del docente</param>
        /// <returns>Lista de experiencias laborales del docente</returns>
        [HttpGet("docente/{docenteCedula}")]
        [ProducesResponseType(typeof(IEnumerable<ExperienciaLaboralDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExperienciasByDocente(string docenteCedula)
        {
            try
            {
                var experiencias = await _experienciaService.GetExperienciasByDocenteAsync(docenteCedula);
                return Ok(new
                {
                    success = true,
                    message = "Experiencias laborales del docente obtenidas exitosamente",
                    data = experiencias,
                    count = experiencias.Count(),
                    docenteCedula = docenteCedula
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener experiencias laborales del docente {DocenteCedula}", docenteCedula);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las experiencias laborales asociadas a una solicitud específica
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Lista de experiencias laborales de la solicitud</returns>
        [HttpGet("solicitud/{solicitudId}")]
        [ProducesResponseType(typeof(IEnumerable<ExperienciaLaboralDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetExperienciasBySolicitud(Guid solicitudId)
        {
            try
            {
                var experiencias = await _experienciaService.GetExperienciasBySolicitudAsync(solicitudId);
                return Ok(new
                {
                    success = true,
                    message = "Experiencias laborales de la solicitud obtenidas exitosamente",
                    data = experiencias,
                    count = experiencias.Count(),
                    solicitudId = solicitudId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener experiencias laborales de la solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea una nueva experiencia laboral
        /// </summary>
        /// <param name="createDto">Datos de la experiencia laboral</param>
        /// <param name="archivo">Archivo del certificado (opcional)</param>
        /// <returns>Experiencia laboral creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExperienciaLaboralDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateExperiencia([FromForm] CreateExperienciaLaboralDto createDto, IFormFile? archivo)
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

                var experiencia = await _experienciaService.CreateExperienciaAsync(createDto, archivo);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Experiencia laboral creada exitosamente",
                    data = experiencia
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear experiencia laboral");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza una experiencia laboral existente
        /// </summary>
        /// <param name="id">ID de la experiencia laboral</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <param name="archivo">Nuevo archivo (opcional)</param>
        /// <returns>Experiencia laboral actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ExperienciaLaboralDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateExperiencia(int id, [FromForm] UpdateExperienciaLaboralDto updateDto, IFormFile? archivo)
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

                var experiencia = await _experienciaService.UpdateExperienciaAsync(id, updateDto, archivo);
                return Ok(new
                {
                    success = true,
                    message = "Experiencia laboral actualizada exitosamente",
                    data = experiencia
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar experiencia laboral con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina una experiencia laboral
        /// </summary>
        /// <param name="id">ID de la experiencia laboral</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteExperiencia(int id)
        {
            try
            {
                var result = await _experienciaService.DeleteExperienciaAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Experiencia laboral no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Experiencia laboral eliminada exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar experiencia laboral con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Asocia una experiencia laboral a una solicitud
        /// </summary>
        /// <param name="asociarDto">Datos de la asociación</param>
        /// <returns>Resultado de la asociación</returns>
        [HttpPost("asociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AsociarExperienciaASolicitud([FromBody] AsociarExperienciaSolicitudDto asociarDto)
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

                var result = await _experienciaService.AsociarExperienciaASolicitudAsync(asociarDto);
                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo asociar la experiencia laboral a la solicitud. Verifique que ambos existan."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Experiencia laboral asociada a la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar experiencia laboral {ExperienciaId} a solicitud {SolicitudId}", 
                    asociarDto.ExperienciaId, asociarDto.SolicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Desasocia una experiencia laboral de una solicitud
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="experienciaId">ID de la experiencia laboral</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpDelete("desasociar-solicitud/{solicitudId}/{experienciaId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarExperienciaDeSolicitud(Guid solicitudId, int experienciaId)
        {
            try
            {
                await _experienciaService.DesasociarExperienciaDeSolicitudAsync(solicitudId, experienciaId);
                return Ok(new
                {
                    success = true,
                    message = "Experiencia laboral desasociada de la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar experiencia laboral {ExperienciaId} de solicitud {SolicitudId}", 
                    experienciaId, solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descarga el archivo de una experiencia laboral
        /// </summary>
        /// <param name="id">ID de la experiencia laboral</param>
        /// <returns>Archivo de la experiencia laboral</returns>
        [HttpGet("{id}/archivo")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
            try
            {
                var archivo = await _experienciaService.GetArchivoExperienciaAsync(id);
                if (archivo == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Archivo no encontrado"
                    });
                }

                var nombreArchivo = await _experienciaService.GetNombreArchivoAsync(id);
                var fileName = nombreArchivo ?? $"experiencia_{id}.pdf";
                
                // Determinar el Content-Type basado en la extensión del archivo
                var contentType = GetContentType(fileName);
                
                return File(archivo, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar archivo de experiencia laboral {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Exporta todas las experiencias laborales a un formato específico
        /// </summary>
        /// <param name="formato">Formato de exportación (json, csv)</param>
        /// <returns>Archivo con los datos exportados</returns>
        [HttpGet("exportar")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportarExperiencias([FromQuery] string formato = "json")
        {
            try
            {
                var experiencias = await _experienciaService.GetAllExperienciasAsync();
                
                switch (formato.ToLower())
                {
                    case "json":
                        var jsonData = System.Text.Json.JsonSerializer.Serialize(experiencias, new JsonSerializerOptions 
                        { 
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        return File(jsonBytes, "application/json", $"experiencias_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                    case "csv":
                        var csvContent = GenerarCSV(experiencias);
                        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                        return File(csvBytes, "text/csv", $"experiencias_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                    default:
                        return BadRequest(new
                        {
                            success = false,
                            message = "Formato no soportado. Use: json, csv"
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar experiencias laborales en formato {Formato}", formato);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        private static string GenerarCSV(IEnumerable<ExperienciaLaboralDto> experiencias)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,OrganizacionId,OrganizacionNombre,OrganizacionTipo,DocenteCedula,DocenteNombreCompleto,Cargo,FechaInicio,FechaFin");
            
            foreach (var experiencia in experiencias)
            {
                csv.AppendLine($"{experiencia.Id}," +
                              $"{experiencia.OrganizacionId}," +
                              $"\"{experiencia.OrganizacionNombre}\"," +
                              $"\"{experiencia.OrganizacionTipo}\"," +
                              $"\"{experiencia.DocenteCedula}\"," +
                              $"\"{experiencia.Cargo}\"," +
                              $"{experiencia.FechaInicio:yyyy-MM-dd}," +
                              $"{(experiencia.FechaFin.HasValue ? experiencia.FechaFin.Value.ToString("yyyy-MM-dd") : "")}");
            }
            
            return csv.ToString();
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
} 