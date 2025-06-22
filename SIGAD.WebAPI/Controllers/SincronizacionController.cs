// using Microsoft.AspNetCore.Mvc;
// using SIGAD.Application.Common;
// using SIGAD.Application.DTOs.IntegracionesExternas;
// using SIGAD.Application.Services;

// using System.Threading.Tasks;

// namespace SIGAD.WebAPI.Controllers
// {
//     [ApiController]
//     [Route("api/sincronizacion")]
//     public class SincronizacionController : ControllerBase
//     {
//         private readonly DocenteSyncCoordinator _coordinador;

//         public SincronizacionController(DocenteSyncCoordinator coordinador)
//         {
//             _coordinador = coordinador;
//         }

//         [HttpGet("importar/{cedula}")]
//         public async Task<ActionResult<HistorialDocenteDto>> ImportarHistorial(string cedula, [FromQuery] string fuente = "SGTH")
//         {
//             var historial = await _coordinador.SincronizarDesdeFuenteAsync(cedula, fuente == "SUT" ? Fuente.SUT : Fuente.SGTH);
//             return Ok(historial);
//         }

//     }
// }
