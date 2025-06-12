/*
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly GestionArticulosAppService _articulosService;

        public ArticulosController(GestionArticulosAppService articulosService)
        {
            _articulosService = articulosService;
        }

        // POST /api/articulos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearArticuloDto dto)
        {
            var docenteCedula = "1234567890"; // Temporal
            await _articulosService.CrearArticuloAsync(dto, docenteCedula);
            return Ok();
        }
    }
}
*/
