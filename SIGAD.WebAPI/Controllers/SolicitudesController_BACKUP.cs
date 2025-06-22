// BACKUP del controlador original - se restaurará después de la migración
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ILogger<SolicitudesController> _logger;

        public SolicitudesController(ILogger<SolicitudesController> logger)
        {
            _logger = logger;
        }

        // GET: api/solicitudes
        [HttpGet]
        public async Task<IActionResult> GetSolicitudes()
        {
            // TODO: Implementar cuando GestionSolicitudesAppService esté listo
            return Ok(new { message = "Servicio temporalmente deshabilitado" });
        }
    }
}
