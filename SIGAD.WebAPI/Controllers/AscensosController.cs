//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SIGAD.Application.Services;
//using SIGAD.Domain.Entities;
//using System.Security.Claims;

//namespace SIGAD.WebAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    public class AscensosController : ControllerBase
//    {
//        private readonly GestionSolicitudesAppService _solicitudService;

//        public AscensosController(GestionSolicitudesAppService solicitudService)
//        {
//            _solicitudService = solicitudService;
//        }

//        [HttpGet("verificar-activa")]
//        public async Task<IActionResult> VerificarSolicitudActiva()
//        {
//            var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(cedula)) return Unauthorized();

//            var solicitudActiva = await _solicitudService.TieneSolicitudActivaAsync(cedula);
//            return Ok(solicitudActiva);
//        }

//        [HttpPost("crear")]
//        public async Task<IActionResult> CrearSolicitud()
//        {
//            var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(cedula)) return Unauthorized();

//            try
//            {
//                var solicitud = await _solicitudService.CrearSolicitudSimpleAsync(cedula, 1); // cambiar 1 por valor real si aplica
//                return Ok(solicitud);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                return NotFound(ex.Message);
//            }
//        }
//    }
//}
