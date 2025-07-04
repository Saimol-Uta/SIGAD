using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin/solicitudes")]
    [Authorize(Roles = "Administrador")]
    public class AdminSolicitudesController : ControllerBase
    {
        private readonly GestionSolicitudesAppService _gestionSolicitudesService;

        public AdminSolicitudesController(GestionSolicitudesAppService gestionSolicitudesService)
        {
            _gestionSolicitudesService = gestionSolicitudesService;
        }

        [HttpGet("con-apelaciones")]
        public async Task<IActionResult> GetSolicitudesConApelaciones()
        {
            try
            {
                var solicitudes = await _gestionSolicitudesService.GetSolicitudesConApelacionesAsync();
                return Ok(solicitudes);
            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = "Error al obtener solicitudes con apelaciones",
                    details = ex.Message
                };
                
                Console.WriteLine($"Error en GetSolicitudesConApelaciones: {ex}");
                return StatusCode(500, errorDetails);
            }
        }

        [HttpGet("pendientes-apelacion")]
        public async Task<IActionResult> GetSolicitudesPendientesApelacion()
        {
            try
            {
                var solicitudes = await _gestionSolicitudesService.GetSolicitudesConApelacionesAsync();
                
                // Filtrar solo las que tienen apelaciones pendientes
                var pendientes = solicitudes.Where(s => s.TieneApelacion && !s.ApelacionVencida).ToList();
                
                return Ok(pendientes);
            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = "Error al obtener solicitudes pendientes de apelación",
                    details = ex.Message
                };
                
                Console.WriteLine($"Error en GetSolicitudesPendientesApelacion: {ex}");
                return StatusCode(500, errorDetails);
            }
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var solicitudes = await _gestionSolicitudesService.GetSolicitudesConApelacionesAsync();
                
                var dashboard = new
                {
                    TotalSolicitudes = solicitudes.Count,
                    ConApelacion = solicitudes.Count(s => s.TieneApelacion),
                    ApelacionesPendientes = solicitudes.Count(s => s.TieneApelacion && !s.ApelacionVencida),
                    ApelacionesVencidas = solicitudes.Count(s => s.ApelacionVencida),
                    PorVencer = solicitudes.Count(s => s.TieneApelacion && s.DiasRestantesApelacion <= 1 && !s.ApelacionVencida),
                    SolicitudesPendientes = solicitudes.Where(s => s.TieneApelacion && !s.ApelacionVencida)
                        .OrderBy(s => s.DiasRestantesApelacion)
                        .Take(5)
                        .ToList()
                };
                
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                var errorDetails = new
                {
                    message = "Error al obtener datos del dashboard",
                    details = ex.Message
                };
                
                Console.WriteLine($"Error en GetDashboardData: {ex}");
                return StatusCode(500, errorDetails);
            }
        }
    }
}
