// using Microsoft.AspNetCore.Mvc;
// using SIGAD.Application.Interfaces;
// using SIGAD.Application.Interfaces.Integraciones;

// using SIGAD.Domain.Entities;
// using SIGAD.Domain.Interfaces;

// namespace SIGAD.WebAPI.Controllers
// {
//     [ApiController]
//     [Route("api/experiencia/externa")]
//     public class ExperienciaExternaController : ControllerBase
//     {
//         private readonly ISgthSyncService _sgth;
//         private readonly ISutSyncService _sut;
//         private readonly IUnitOfWork _unitOfWork;

//         public ExperienciaExternaController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork)
//         {
//             _sgth = sgth;
//             _sut = sut;
//             _unitOfWork = unitOfWork;
//         }

//         [HttpPost("importar/{cedula}")]
//         public async Task<IActionResult> ImportarExperienciaLaboral(string cedula)
//         {
//             var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
//             if (docente == null)
//                 return NotFound($"Docente con cédula {cedula} no encontrado.");

//             var externos = (await _sgth.ObtenerExperienciasAsync(cedula))
//                             .Concat(await _sut.ObtenerExperienciasAsync(cedula))
//                             .DistinctBy(e => e.ContenidoHash);

//             int insertados = 0;

//             foreach (var dto in externos)
//             {
//                 bool existe = await _unitOfWork.Experiencias.ExistePorHashAsync(dto.ContenidoHash);
//                 if (!existe)
//                 {
//                     // Obtener o crear organización
//                     var organizacion = await _unitOfWork.Organizaciones.ObtenerPorNombreAsync(dto.Organizacion);
//                     if (organizacion == null)
//                     {
//                         organizacion = new Organizacion { Nombre = dto.Organizacion };
//                         await _unitOfWork.Organizaciones.AgregarAsync(organizacion);
//                         await _unitOfWork.CompleteAsync(); // Para obtener el ID
//                     }

//                     var experiencia = new ExperienciaLaboral
//                     {
//                         Cargo = dto.Cargo,
//                         FechaInicio = dto.FechaInicio,
//                         FechaFin = dto.FechaFin,
//                         CertificadoRuta = dto.CertificadoRuta,
//                         ContenidoHash = dto.ContenidoHash,
//                         DocenteCedula = docente.Cedula,
//                         OrganizacionId = organizacion.Id
//                     };

//                     await _unitOfWork.Experiencias.AgregarAsync(experiencia);
//                     insertados++;
//                 }
//             }

//             await _unitOfWork.CompleteAsync();

//             return Ok(new { mensaje = $"Se importaron {insertados} experiencias laborales nuevas." });
//         }
//     }
// }
