/*
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestigacionesController : ControllerBase
    {
        private readonly GestionInvestigacionesAppService _investigacionesService;

        public InvestigacionesController(GestionInvestigacionesAppService investigacionesService)
        {
            _investigacionesService = investigacionesService;
        }

        // POST /api/investigaciones
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearInvestigacionDto dto)
        {
            var docenteCedula = "1234567890"; // Temporal
            await _investigacionesService.CrearInvestigacionAsync(dto, docenteCedula);
            return Ok();
        }
    }
}
*/
