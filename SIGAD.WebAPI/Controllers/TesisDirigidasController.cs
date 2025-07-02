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

        public TesisDirigidasController(ITesisDirigidaService service)
        {
            _service = service;
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

        [HttpPost("asociar")]
        public async Task<IActionResult> AsociarASolicitud(Guid solicitudId, int tesisId)
        {
            await _service.AsociarASolicitudAsync(solicitudId, tesisId);
            return Ok();
        }

        [HttpDelete("desasociar")]
        public async Task<IActionResult> DesasociarDeSolicitud(Guid solicitudId, int tesisId)
        {
            await _service.DesasociarDeSolicitudAsync(solicitudId, tesisId);
            return Ok();
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

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "Tesis");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            createDto.CertificacionRuta = $"Tesis/{fileName}";

            var nueva = await _service.CrearAsync(createDto);
            return CreatedAtAction(nameof(ObtenerPorDocente), new { cedula = nueva.DocenteCedula }, nueva);
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

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "Tesis");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            createDto.CertificacionRuta = $"Tesis/{fileName}";

            var actualizado = await _service.EditarAsync(id, createDto);
            if (actualizado)
                return NoContent();

            return NotFound();
        }

    }
}