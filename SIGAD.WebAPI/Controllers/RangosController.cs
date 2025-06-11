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

        // El servicio de aplicación se inyecta a través del constructor
        public RangosController(ConsultaRangoAppService rangoAppService)
        {
            _rangoAppService = rangoAppService;
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

        // NOTA SOBRE SaveChangesAsync():
        // Si tuviéramos un método POST para crear un rango, se vería algo así:
        // [HttpPost]
        // public async Task<IActionResult> CreateRango([FromBody] CrearRangoDto crearRangoDto)
        // {
        //     // Suponiendo que tienes un CrearRangoAppService o un método en ConsultaRangoAppService
        //     // var rangoId = await _rangoAppService.CreateRangoAsync(crearRangoDto);
        //
        //     // Aquí es donde el Application Service se habría encargado de llamar a AddAsync del repositorio
        //     // y luego, importante, a SaveChangesAsync() (ya sea directamente en el App Service si tiene
        //     // el DbContext inyectado -menos ideal- o a través de un servicio de Unit of Work).
        //
        //     // return CreatedAtAction(nameof(GetRangoById), new { id = rangoId }, crearRangoDto);
        // }
    }
}