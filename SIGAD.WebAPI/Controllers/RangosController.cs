// SIGAD.WebAPI/Controllers/RangosController.cs
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;          // Para RangoDto
using SIGAD.Application.Services;    // Para ConsultaRangoAppService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController] // Indica que este es un controlador de API
    [Route("api/[controller]")] // Define la ruta base: "api/rangos"
    public class RangosController : ControllerBase
    {
        private readonly ConsultaRangoAppService _rangoAppService;
        private readonly GestionRangoAppService _gestionRangoService;
        private readonly ActualizarRangoService _actualizarRangoService;


        // El servicio de aplicación se inyecta a través del constructor
        public RangosController(ConsultaRangoAppService rangoAppService, GestionRangoAppService gestionRangoService, ActualizarRangoService actualizarRangoService)
        {
            _rangoAppService = rangoAppService;
            _gestionRangoService = gestionRangoService;
            _actualizarRangoService = actualizarRangoService;
        }

        // GET: api/rangos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RangoDto>>> GetAllRangosAsync()
        {
            var rangos = await _rangoAppService.GetAllRangosAsync();
            if (rangos == null || !rangos.Any())
            {
                return NotFound("No se encontraron rangos."); // Opcional: podrías devolver una lista vacía Ok(new List<RangoDto>())
            }
            return Ok(rangos); // Devuelve 200 OK con la lista de rangos
        }

        [HttpPost]
        public async Task<IActionResult> CrearRango([FromBody] CrearRangoDto rangoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Devuelve errores de validación
            }

            try
            {
                var nuevoRango = await _gestionRangoService.CrearRangoAsync(rangoDto);
                // Devuelve un 201 Created con una referencia al nuevo recurso y el objeto creado.
                // Necesitaríamos un endpoint GetById para que esto funcione perfectamente.
                return CreatedAtAction(nameof(GetRangoById), new { id = nuevoRango.Id }, nuevoRango);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Devuelve 409 Conflict si el rango ya existe
            }
            catch (Exception ex)
            {
                // Manejo de otros posibles errores
                return StatusCode(500, "Ocurrió un error interno.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RangoDto>> GetRangoById(int id)
        {
            var rangos = await _rangoAppService.GetAllRangosAsync();
            var rango = rangos.FirstOrDefault(r => r.Id == id);

            if (rango == null)
            {
                return NotFound($"No se encontró un rango con Id {id}.");
            }

            return Ok(rango);
        }

        // Aquí podrías añadir más métodos para actualizar, eliminar, etc. rangos según sea necesario.
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarRango(int id, [FromBody] ActualizarRangoDto rangoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _actualizarRangoService.ActualizarRangoAsync(id, rangoDto);
                // HTTP 204 No Content es la respuesta estándar para un UPDATE exitoso que no devuelve datos.
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // HTTP 404 si no se encontró el recurso
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }
    }
}