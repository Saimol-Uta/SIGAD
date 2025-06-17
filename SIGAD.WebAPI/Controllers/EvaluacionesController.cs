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
    public class EvaluacionesController : ControllerBase
    {
        private readonly IEvaluacionDocenteService _evaluacionService;
        private readonly ILogger<EvaluacionesController> _logger;

        public EvaluacionesController(
            IEvaluacionDocenteService evaluacionService,
            ILogger<EvaluacionesController> logger)
        {
            _evaluacionService = evaluacionService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las evaluaciones docentes
        /// </summary>
        /// <returns>Lista de evaluaciones</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EvaluacionDocenteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEvaluaciones()
        {
            try
            {
                var evaluaciones = await _evaluacionService.GetAllEvaluacionesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Evaluaciones obtenidas exitosamente",
                    data = evaluaciones,
                    count = evaluaciones.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las evaluaciones");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene una evaluación específica por ID
        /// </summary>
        /// <param name="id">ID de la evaluación</param>
        /// <returns>Datos de la evaluación</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EvaluacionDocenteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvaluacionById(int id)
        {
            try
            {
                var evaluacion = await _evaluacionService.GetEvaluacionByIdAsync(id);
                if (evaluacion == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Evaluación no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Evaluación obtenida exitosamente",
                    data = evaluacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la evaluación con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las evaluaciones de un docente específico
        /// </summary>
        /// <param name="docenteCedula">Cédula del docente</param>
        /// <returns>Lista de evaluaciones del docente</returns>
        [HttpGet("docente/{docenteCedula}")]
        [ProducesResponseType(typeof(IEnumerable<EvaluacionDocenteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvaluacionesByDocente(string docenteCedula)
        {
            try
            {
                var evaluaciones = await _evaluacionService.GetEvaluacionesByDocenteAsync(docenteCedula);
                return Ok(new
                {
                    success = true,
                    message = "Evaluaciones del docente obtenidas exitosamente",
                    data = evaluaciones,
                    count = evaluaciones.Count(),
                    docenteCedula = docenteCedula
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evaluaciones del docente {DocenteCedula}", docenteCedula);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene las evaluaciones asociadas a una solicitud específica
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Lista de evaluaciones de la solicitud</returns>
        [HttpGet("solicitud/{solicitudId}")]
        [ProducesResponseType(typeof(IEnumerable<EvaluacionDocenteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEvaluacionesBySolicitud(Guid solicitudId)
        {
            try
            {
                var evaluaciones = await _evaluacionService.GetEvaluacionesBySolicitudAsync(solicitudId);
                return Ok(new
                {
                    success = true,
                    message = "Evaluaciones de la solicitud obtenidas exitosamente",
                    data = evaluaciones,
                    count = evaluaciones.Count(),
                    solicitudId = solicitudId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evaluaciones de la solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea una nueva evaluación docente
        /// </summary>
        /// <param name="createDto">Datos de la evaluación</param>
        /// <param name="archivo">Archivo de la evaluación (opcional)</param>
        /// <returns>Evaluación creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(EvaluacionDocenteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEvaluacion([FromForm] CreateEvaluacionDocenteDto createDto, IFormFile? archivo)
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

                var evaluacion = await _evaluacionService.CreateEvaluacionAsync(createDto, archivo);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Evaluación creada exitosamente",
                    data = evaluacion
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
                _logger.LogError(ex, "Error al crear evaluación");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza una evaluación existente
        /// </summary>
        /// <param name="id">ID de la evaluación</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <param name="archivo">Nuevo archivo (opcional)</param>
        /// <returns>Evaluación actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(EvaluacionDocenteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEvaluacion(int id, [FromForm] UpdateEvaluacionDocenteDto updateDto, IFormFile? archivo)
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

                var evaluacion = await _evaluacionService.UpdateEvaluacionAsync(id, updateDto, archivo);
                return Ok(new
                {
                    success = true,
                    message = "Evaluación actualizada exitosamente",
                    data = evaluacion
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
                _logger.LogError(ex, "Error al actualizar evaluación con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina una evaluación
        /// </summary>
        /// <param name="id">ID de la evaluación</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEvaluacion(int id)
        {
            try
            {
                var result = await _evaluacionService.DeleteEvaluacionAsync(id);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Evaluación no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Evaluación eliminada exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar evaluación con ID {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Asocia una evaluación a una solicitud
        /// </summary>
        /// <param name="asociarDto">Datos de la asociación</param>
        /// <returns>Resultado de la asociación</returns>
        [HttpPost("asociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AsociarEvaluacionASolicitud([FromBody] AsociarEvaluacionSolicitudDto asociarDto)
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

                var result = await _evaluacionService.AsociarEvaluacionASolicitudAsync(asociarDto);
                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo asociar la evaluación a la solicitud. Verifique que ambos existan."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Evaluación asociada a la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar evaluación {EvaluacionId} a solicitud {SolicitudId}", 
                    asociarDto.EvaluacionId, asociarDto.SolicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Desasocia una evaluación de una solicitud
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="evaluacionId">ID de la evaluación</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpDelete("desasociar-solicitud/{solicitudId}/{evaluacionId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarEvaluacionDeSolicitud(Guid solicitudId, int evaluacionId)
        {
            try
            {
                await _evaluacionService.DesasociarEvaluacionDeSolicitudAsync(solicitudId, evaluacionId);
                return Ok(new
                {
                    success = true,
                    message = "Evaluación desasociada de la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar evaluación {EvaluacionId} de solicitud {SolicitudId}", 
                    evaluacionId, solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descarga el archivo de una evaluación
        /// </summary>
        /// <param name="id">ID de la evaluación</param>
        /// <returns>Archivo de la evaluación</returns>
        [HttpGet("{id}/archivo")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DescargarArchivo(int id)
        {
            try
            {
                var archivo = await _evaluacionService.GetArchivoEvaluacionAsync(id);
                if (archivo == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Archivo no encontrado"
                    });
                }

                var nombreArchivo = await _evaluacionService.GetNombreArchivoAsync(id);
                var fileName = nombreArchivo ?? $"evaluacion_{id}.pdf";
                
                // Determinar el Content-Type basado en la extensión del archivo
                var contentType = GetContentType(fileName);
                
                // Establecer el header Content-Disposition para forzar la descarga con el nombre correcto
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                
                return File(archivo, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar archivo de evaluación {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Exporta todas las evaluaciones a un formato específico
        /// </summary>
        /// <param name="formato">Formato de exportación (json, csv, excel)</param>
        /// <returns>Archivo con los datos exportados</returns>
        [HttpGet("exportar")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportarEvaluaciones([FromQuery] string formato = "json")
        {
            try
            {
                var evaluaciones = await _evaluacionService.GetAllEvaluacionesAsync();
                
                switch (formato.ToLower())
                {
                    case "json":
                        var jsonData = System.Text.Json.JsonSerializer.Serialize(evaluaciones, new JsonSerializerOptions 
                        { 
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        return File(jsonBytes, "application/json", $"evaluaciones_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                    case "csv":
                        var csvContent = GenerarCSV(evaluaciones);
                        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                        return File(csvBytes, "text/csv", $"evaluaciones_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

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
                _logger.LogError(ex, "Error al exportar evaluaciones en formato {Formato}", formato);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        private static string GenerarCSV(IEnumerable<EvaluacionDocenteDto> evaluaciones)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,PeriodoAcademico,FechaEvaluacion,PuntajePorcentual,DocenteCedula,DocenteNombreCompleto");
            
            foreach (var evaluacion in evaluaciones)
            {
                csv.AppendLine($"{evaluacion.Id}," +
                              $"\"{evaluacion.PeriodoAcademico}\"," +
                              $"{evaluacion.FechaEvaluacion:yyyy-MM-dd}," +
                              $"{evaluacion.PuntajePorcentual}," +
                              $"\"{evaluacion.DocenteCedula}\"," +
                              $"\"{evaluacion.DocenteNombreCompleto}\"");
            }
            
            return csv.ToString();
        }

        /// <summary>
        /// Determina el Content-Type basado en la extensión del archivo
        /// </summary>
        /// <param name="fileName">Nombre del archivo</param>
        /// <returns>Content-Type apropiado</returns>
        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".txt" => "text/plain",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".zip" => "application/zip",
                ".rar" => "application/vnd.rar",
                _ => "application/octet-stream"
            };
        }
    }
}