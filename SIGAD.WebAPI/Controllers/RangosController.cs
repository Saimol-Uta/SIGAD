/*
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
        private readonly ConsultaRangoAppService _consultaRangoService;
        private readonly GestionRangoAppService _gestionRangoService;
        private readonly ActualizarRangoService _actualizarRangoService;

        // El servicio de aplicación se inyecta a través del constructor
        public RangosController(
            ConsultaRangoAppService consultaRangoService,
            GestionRangoAppService gestionRangoService,
            ActualizarRangoService actualizarRangoService)
        {
            _consultaRangoService = consultaRangoService;
            _gestionRangoService = gestionRangoService;
            _actualizarRangoService = actualizarRangoService;
        }

        // GET /api/rangos/docente/{cedula}
        // Ejemplo: GET /api/rangos/docente/1234567890
        [HttpGet("docente/{cedula}")]
        public async Task<IActionResult> GetRangoByDocente(string cedula)
        {
            try
            {
                var rangos = await _consultaRangoService.GetAllRangosAsync();
                return Ok(rangos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/rangos
        [HttpPost]
        public async Task<IActionResult> CrearRango([FromBody] CrearRangoDto dto)
        {
            try
            {
                var rangoId = await _gestionRangoService.CrearRangoAsync(dto);
                return CreatedAtAction(nameof(GetRangoByDocente), new { cedula = "temp" }, rangoId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/rangos
        // Devuelve todos los rangos del sistema
        [HttpGet]
        public async Task<IActionResult> GetAllRangos()
        {
            try
            {
                var rangos = await _consultaRangoService.GetAllRangosAsync();
                return Ok(rangos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/rangos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarRango(int id, [FromBody] ActualizarRangoDto dto)
        {
            try
            {
                await _actualizarRangoService.ActualizarRangoAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
*/