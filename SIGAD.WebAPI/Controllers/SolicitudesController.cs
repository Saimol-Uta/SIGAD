using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System;
using System.Threading.Tasks;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        // El servicio para la lógica que estará comentada
        private readonly GestionSolicitudesAppService _solicitudesService;

        // El nuevo servicio para la lógica de validación que SÍ estará activa
        private readonly IValidacionRequisitosService _validacionService;

        // El constructor ahora inyecta ambos servicios
        public SolicitudesController(
            GestionSolicitudesAppService solicitudesService,
            IValidacionRequisitosService validacionService)
        {
            _solicitudesService = solicitudesService;
            _validacionService = validacionService;
        }

        /* --- LÓGICA ANTERIOR (COMENTADA POR AHORA) ---

        // GET /api/solicitudes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var solicitudes = await _solicitudesService.GetAllSolicitudesAsync();
            return Ok(solicitudes);
        }

        // POST /api/solicitudes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearSolicitudDto dto)
        {
            var docenteCedula = "1234567890"; // Temporal
            var id = await _solicitudesService.CrearSolicitudAsync(dto, docenteCedula);
            return Ok(new { Id = id });
        }

        // PUT /api/solicitudes/{id}/aprobar
        [HttpPut("{id}/aprobar")]
        public async Task<IActionResult> Aprobar(Guid id)
        {
            await _solicitudesService.AprobarSolicitudAsync(id);
            return Ok();
        }

        // PUT /api/solicitudes/{id}/rechazar
        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid id, [FromBody] string observaciones)
        {
            await _solicitudesService.RechazarSolicitudAsync(id, observaciones);
            return Ok();
        }
        
        */

        // --- PASO 5: NUEVO ENDPOINT (ACTIVO) ---
        [HttpGet("verificar-progreso/{rangoId}")]
        [Authorize] // Asegurarse de que solo usuarios autenticados puedan llamarlo
        public async Task<IActionResult> VerificarProgreso(int rangoId)
        {
            try
            {
                // Obtener la cédula del usuario logueado desde el token
                var docenteCedula = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(docenteCedula))
                {
                    return Unauthorized("No se pudo identificar la cédula del usuario en el token.");
                }

                var resultado = await _validacionService.VerificarProgresoAsync(docenteCedula, rangoId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // Puedes añadir un logger aquí si quieres registrar el error
                // _logger.LogError(ex, "Error al verificar progreso para rango {RangoId}", rangoId);
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}