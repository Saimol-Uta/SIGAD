using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System;
using System.Threading.Tasks;
using System.Security.Claims; // Agregar esta línea al inicio del archivo

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

        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _solicitudesService.GetAllParaAdminAsync();
            return Ok(solicitudes);
        }

        // GET: api/solicitudes/{id}
        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(Guid id)
        {
            var solicitud = await _solicitudesService.GetDetalleParaAdminAsync(id);
            return solicitud != null ? Ok(solicitud) : NotFound();
        }

        // POST: api/solicitudes
        [HttpPost]
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
        public async Task<IActionResult> Aprobar(Guid id, [FromBody] string observaciones)
        {
            await _solicitudesService.AprobarSolicitudAsync(id, observaciones);
            return NoContent();
        }

        // PUT: api/solicitudes/{id}/rechazar
        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid id, [FromBody] string observaciones)
        {
            await _solicitudesService.RechazarSolicitudAsync(id, observaciones);
            return NoContent();
        }

        // Endpoint de prueba sin autorización
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API funcionando correctamente", timestamp = DateTime.Now });
        }

        // Endpoint de prueba con autorización básica
        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            var userInfo = new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            return Ok(userInfo);
        }
    }
}