using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.WebAPI.Models;

namespace SIGAD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TesisDirigidasController : ControllerBase
    {
        private readonly ITesisDirigidaService _service;
        private readonly IFileStorageService _fileStorageService;

        public TesisDirigidasController(ITesisDirigidaService service, IFileStorageService fileStorageService)
        {
            _service = service;
            _fileStorageService = fileStorageService;
        }

        [HttpGet("docente/{cedula}")]
        public async Task<IActionResult> ObtenerPorDocente(string cedula)
        {
            var tesis = await _service.ObtenerPorDocenteAsync(cedula);
            return Ok(new { data = tesis });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CreateTesisDirigidaDto dto)
        {
            var nueva = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorDocente), new { cedula = nueva.DocenteCedula }, nueva);
        }

        [HttpPost("{id}/asociar-solicitud")]
        public async Task<IActionResult> AsociarASolicitud(int id, [FromBody] AsociarTesisSolicitudDto request)
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

                await _service.AsociarASolicitudAsync(request.SolicitudId, id);
                
                return Ok(new
                {
                    success = true,
                    message = "Tesis asociada exitosamente a la solicitud"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }

        [HttpDelete("desasociar")]
        public async Task<IActionResult> DesasociarDeSolicitud(Guid solicitudId, int tesisId)
        {
            await _service.DesasociarDeSolicitudAsync(solicitudId, tesisId);
            return Ok();
        }

        /// <summary>
        /// Desasocia una tesis dirigida de una solicitud (POST version para frontend con ID en ruta)
        /// </summary>
        /// <param name="id">ID de la tesis</param>
        /// <param name="dto">Datos de la solicitud</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpPost("{id}/desasociar-solicitud")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DesasociarTesisDeSolicitud(int id, [FromBody] AsociarTesisSolicitudDto dto)
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

                await _service.DesasociarDeSolicitudAsync(dto.SolicitudId, id);
                return Ok(new
                {
                    success = true,
                    message = "Tesis dirigida desasociada exitosamente"
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

        [HttpGet("existe-por-hash/{hash}")]
        public async Task<IActionResult> ExistePorHash(string hash)
        {
            var existe = await _service.ExistePorHashAsync(hash);
            return Ok(existe);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Debes implementar este método en tu servicio
            var eliminado = await _service.EliminarAsync(id);
            if (eliminado)
                return NoContent();
            return NotFound();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, [FromBody] CreateTesisDirigidaDto dto)
        {
            // Debes implementar este método en tu servicio
            var actualizado = await _service.EditarAsync(id, dto);
            if (actualizado)
                return NoContent();
            return NotFound();
        }
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            var relativa = await _service.ObtenerRutaPdfAsync(id);
            if (string.IsNullOrEmpty(relativa))
                return NotFound();

            // Asegura que la ruta sea relativa a la carpeta 'uploads'
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var rutaAbsoluta = Path.Combine(uploadsFolder, relativa.Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(rutaAbsoluta))
                return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaAbsoluta);
            return File(bytes, "application/pdf", $"tesis_{id}.pdf");
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var tesis = await _service.ObtenerPorIdAsync(id);
            if (tesis == null)
                return NotFound();
            return Ok(tesis);
        }
        [HttpPost("subir-pdf")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubirPdf([FromForm] SubirTesisPdfRequest request)
        {
            var file = request.File;
            var dto = request.Dto;

            if (file == null || file.Length == 0)
                return BadRequest("Archivo no proporcionado.");

            var extensionesPermitidas = new[] { ".pdf", ".doc", ".docx" };
            if (!extensionesPermitidas.Contains(Path.GetExtension(file.FileName).ToLower()))
                return BadRequest("Formato de archivo no permitido.");

            var createDto = System.Text.Json.JsonSerializer.Deserialize<CreateTesisDirigidaDto>(dto);
            if (createDto == null)
                return BadRequest("Datos de tesis inválidos.");

            try
            {
                Console.WriteLine($"🔄 [TesisDirigidasController] Iniciando subida de archivo: {file.FileName}, Tamaño: {file.Length} bytes");
                
                // Usar FileStorageService para almacenamiento dual
                var (rutaLocal, urlCloudinary, hash) = await _fileStorageService.UploadFileAsync(file, "tesis");
                
                Console.WriteLine($"✅ [TesisDirigidasController] Archivo subido - Local: {rutaLocal}, Cloudinary: {!string.IsNullOrEmpty(urlCloudinary)}, Hash: {hash}");
                
                createDto.CertificacionRuta = rutaLocal;
                createDto.UrlCloudinary = urlCloudinary;
                createDto.ContenidoHash = hash;
                
                var nueva = await _service.CrearAsync(createDto);
                
                return Ok(new 
                { 
                    success = true, 
                    data = nueva,
                    message = "Tesis creada exitosamente con almacenamiento dual",
                    certificadoRuta = rutaLocal,
                    urlCloudinary = urlCloudinary
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al subir archivo: {ex.Message}");
            }
        }
        [HttpPut("editar-pdf/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditarConPdf(int id, [FromForm] SubirTesisPdfRequest request)
        {
            var file = request.File;
            var dto = request.Dto;

            if (file == null || file.Length == 0)
                return BadRequest("Archivo no proporcionado.");

            var createDto = System.Text.Json.JsonSerializer.Deserialize<CreateTesisDirigidaDto>(dto);
            if (createDto == null)
                return BadRequest("Datos de tesis inválidos.");

            try
            {
                // Obtener la tesis existente para eliminar archivos anteriores
                var tesisExistente = await _service.ObtenerPorIdAsync(id);
                if (tesisExistente == null)
                    return NotFound("Tesis no encontrada.");

                // Eliminar archivos anteriores si existen
                if (!string.IsNullOrEmpty(tesisExistente.CertificacionRuta) || !string.IsNullOrEmpty(tesisExistente.UrlCloudinary))
                {
                    await _fileStorageService.EliminarArchivoDualAsync(tesisExistente.CertificacionRuta, tesisExistente.UrlCloudinary);
                }

                // Subir nuevo archivo con almacenamiento dual
                var (rutaLocal, urlCloudinary, hash) = await _fileStorageService.UploadFileAsync(file, "tesis");
                
                createDto.CertificacionRuta = rutaLocal;
                createDto.UrlCloudinary = urlCloudinary;
                createDto.ContenidoHash = hash;
                
                var actualizado = await _service.EditarAsync(id, createDto);
                if (actualizado)
                {
                    return Ok(new 
                    { 
                        success = true, 
                        message = "Tesis actualizada exitosamente con almacenamiento dual",
                        certificadoRuta = rutaLocal,
                        urlCloudinary = urlCloudinary
                    });
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar archivo: {ex.Message}");
            }
        }

    }
}