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
    public class ArticulosController : ControllerBase
    {
        private readonly IArticuloService _articuloService;
        private readonly ILogger<ArticulosController> _logger;

        public ArticulosController(
            IArticuloService articuloService,
            ILogger<ArticulosController> logger)
        {
            _articuloService = articuloService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los artículos
        /// </summary>
        /// <returns>Lista de artículos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticuloDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllArticulos()
        {
            try
            {
                var articulos = await _articuloService.GetAllArticulosAsync();
                return Ok(new
                {
                    success = true,
                    message = "Artículos obtenidos exitosamente",
                    data = articulos,
                    count = articulos.Count()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los artículos");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un artículo específico por DOI
        /// </summary>
        /// <param name="doi">DOI del artículo</param>
        /// <returns>Datos del artículo</returns>
        [HttpGet("{doi}")]
        [ProducesResponseType(typeof(ArticuloDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticuloById(string doi)
        {
            try
            {
                var articulo = await _articuloService.GetArticuloByIdAsync(doi);
                if (articulo == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Artículo no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Artículo obtenido exitosamente",
                    data = articulo
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el artículo con DOI {DOI}", doi);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene los artículos de un docente específico
        /// </summary>
        /// <param name="docenteCedula">Cédula del docente</param>
        /// <returns>Lista de artículos del docente</returns>
        [HttpGet("docente/{docenteCedula}")]
        [ProducesResponseType(typeof(IEnumerable<ArticuloDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticulosByDocente(string docenteCedula)
        {
            try
            {
                var articulos = await _articuloService.GetArticulosByDocenteAsync(docenteCedula);
                return Ok(new
                {
                    success = true,
                    message = "Artículos del docente obtenidos exitosamente",
                    data = articulos,
                    count = articulos.Count(),
                    docenteCedula = docenteCedula
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículos del docente {DocenteCedula}", docenteCedula);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene los artículos asociados a una solicitud específica
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Lista de artículos de la solicitud</returns>
        [HttpGet("solicitud/{solicitudId}")]
        [ProducesResponseType(typeof(IEnumerable<ArticuloDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticulosBySolicitud(Guid solicitudId)
        {
            try
            {
                var articulos = await _articuloService.GetArticulosBySolicitudAsync(solicitudId);
                return Ok(new
                {
                    success = true,
                    message = "Artículos de la solicitud obtenidos exitosamente",
                    data = articulos,
                    count = articulos.Count(),
                    solicitudId = solicitudId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener artículos de la solicitud {SolicitudId}", solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea un nuevo artículo
        /// </summary>
        /// <param name="createDto">Datos del artículo</param>
        /// <param name="archivo">Archivo del artículo (opcional)</param>
        /// <returns>Artículo creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ArticuloDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateArticulo([FromForm] CrearArticuloDto createDto, IFormFile? archivo)
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

                var articulo = await _articuloService.CreateArticuloAsync(createDto, archivo);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Artículo creado exitosamente",
                    data = articulo
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
                _logger.LogError(ex, "Error al crear artículo");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza un artículo existente
        /// </summary>
        /// <param name="doi">DOI del artículo</param>
        /// <param name="updateDto">Datos actualizados</param>
        /// <param name="archivo">Nuevo archivo (opcional)</param>
        /// <returns>Artículo actualizado</returns>
        [HttpPut("{doi}")]
        [ProducesResponseType(typeof(ArticuloDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateArticulo(string doi, [FromForm] ActualizarArticuloDto updateDto, IFormFile? archivo)
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

                var articulo = await _articuloService.UpdateArticuloAsync(doi, updateDto, archivo);
                return Ok(new
                {
                    success = true,
                    message = "Artículo actualizado exitosamente",
                    data = articulo
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
                _logger.LogError(ex, "Error al actualizar artículo con DOI {DOI}", doi);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Elimina un artículo
        /// </summary>
        /// <param name="doi">DOI del artículo</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{doi}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteArticulo(string doi)
        {
            try
            {
                var result = await _articuloService.DeleteArticuloAsync(doi);
                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Artículo no encontrado"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Artículo eliminado exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar artículo con DOI {DOI}", doi);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Asocia un artículo a una solicitud
        /// </summary>
        /// <param name="asociarDto">Datos de la asociación</param>
        /// <returns>Resultado de la asociación</returns>
        [HttpPost("asociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AsociarArticuloASolicitud([FromBody] AsociarArticuloSolicitudDto asociarDto)
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

                var result = await _articuloService.AsociarArticuloASolicitudAsync(asociarDto);
                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo asociar el artículo a la solicitud. Verifique que ambos existan."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Artículo asociado a la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar artículo {ArticuloDOI} a solicitud {SolicitudId}", 
                    asociarDto.ArticuloDOI, asociarDto.SolicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Desasocia un artículo de una solicitud
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="articuloDoi">DOI del artículo</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpDelete("desasociar-solicitud/{solicitudId}/{articuloDoi}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarArticuloDeSolicitud(Guid solicitudId, string articuloDoi)
        {
            try
            {
                await _articuloService.DesasociarArticuloDeSolicitudAsync(solicitudId, articuloDoi);
                return Ok(new
                {
                    success = true,
                    message = "Artículo desasociado de la solicitud exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasociar artículo {ArticuloDOI} de solicitud {SolicitudId}", 
                    articuloDoi, solicitudId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Descarga el archivo de un artículo
        /// </summary>
        /// <param name="doi">DOI del artículo</param>
        /// <returns>Archivo del artículo</returns>
        [HttpGet("{doi}/archivo")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DescargarArchivo(string doi)
        {
            try
            {
                var archivo = await _articuloService.GetArchivoArticuloAsync(doi);
                if (archivo == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Archivo no encontrado"
                    });
                }

                var nombreArchivo = await _articuloService.GetNombreArchivoAsync(doi);
                var fileName = nombreArchivo ?? $"articulo_{doi}.pdf";
                
                // Determinar el Content-Type basado en la extensión del archivo
                var contentType = GetContentType(fileName);
                
                // Establecer el header Content-Disposition para forzar la descarga con el nombre correcto
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                
                return File(archivo, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar archivo de artículo {DOI}", doi);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Exporta todos los artículos a un formato específico
        /// </summary>
        /// <param name="formato">Formato de exportación (json, csv, excel)</param>
        /// <returns>Archivo con los datos exportados</returns>
        [HttpGet("exportar")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportarArticulos([FromQuery] string formato = "json")
        {
            try
            {
                var articulos = await _articuloService.GetAllArticulosAsync();
                
                switch (formato.ToLower())
                {
                    case "json":
                        var jsonData = System.Text.Json.JsonSerializer.Serialize(articulos, new JsonSerializerOptions 
                        { 
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });
                        var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        return File(jsonBytes, "application/json", $"articulos_{DateTime.Now:yyyyMMdd_HHmmss}.json");

                    case "csv":
                        var csvContent = GenerarCSV(articulos);
                        var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                        return File(csvBytes, "text/csv", $"articulos_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

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
                _logger.LogError(ex, "Error al exportar artículos en formato {Formato}", formato);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor",
                    error = ex.Message
                });
            }
        }

        private static string GenerarCSV(IEnumerable<ArticuloDto> articulos)
        {
            var csv = new StringBuilder();
            csv.AppendLine("DOI,Titulo,Revista,AnioPublicacion,DocenteCedula,DocenteNombreCompleto");
            
            foreach (var articulo in articulos)
            {
                csv.AppendLine($"\"{articulo.DOI}\"," +
                              $"\"{articulo.Titulo}\"," +
                              $"\"{articulo.Revista}\"," +
                              $"{articulo.AnioPublicacion}," +
                              $"\"{articulo.DocenteCedula}\"," +
                              $"\"{articulo.DocenteNombreCompleto}\"");
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
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }
    }
}
