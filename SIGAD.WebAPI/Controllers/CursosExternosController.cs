// using Microsoft.AspNetCore.Mvc;
// using SIGAD.Application.Interfaces.Integraciones;
// using SIGAD.Domain.Entities;
// using SIGAD.Domain.Interfaces;

// namespace SIGAD.WebAPI.Controllers
// {
//     [ApiController]
//     [Route("api/cursos/externos")]
//     public class CursosExternosController : ControllerBase
//     {
//         private readonly ISgthSyncService _sgth;
//         private readonly ISutSyncService _sut;
//         private readonly IUnitOfWork _unitOfWork;

//         public CursosExternosController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork)
//         {
//             _sgth = sgth;
//             _sut = sut;
//             _unitOfWork = unitOfWork;
//         }

//         [HttpPost("importar/{cedula}")]
//         public async Task<IActionResult> ImportarCursos(string cedula)
//         {
//             var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
//             if (docente == null)
//                 return NotFound($"Docente con cédula {cedula} no encontrado.");

//             var externos = (await _sgth.ObtenerCursosAsync(cedula))
//                             .Concat(await _sut.ObtenerCursosAsync(cedula))
//                             .DistinctBy(c => c.ContenidoHash);

//             int insertados = 0;

//             foreach (var dto in externos)
//             {
//                 bool existe = await _unitOfWork.Cursos.ExistePorHashAsync(dto.ContenidoHash);
//                 if (!existe)
//                 {
//                     var organizacion = await _unitOfWork.Organizaciones.ObtenerPorNombreAsync(dto.Organizacion);
//                     if (organizacion == null)
//                     {
//                         organizacion = new Organizacion { Nombre = dto.Organizacion };
//                         await _unitOfWork.Organizaciones.AgregarAsync(organizacion);
//                         await _unitOfWork.CompleteAsync();
//                     }

//                     var curso = new Curso
//                     {
//                         Nombre = dto.Nombre,
//                         NumeroHoras = dto.NumeroHoras,
//                         FechaFinalizacion = dto.FechaFinalizacion,
//                         CertificadoRuta = dto.CertificadoRuta,
//                         ContenidoHash = dto.ContenidoHash,
//                         DocenteCedula = docente.Cedula,
//                         OrganizacionId = organizacion.Id,

//                         TipoCurso = dto.TipoCurso,
//                         ImpartidoPorDocente = dto.ImpartidoPorDocente

//                     };



//                     await _unitOfWork.Cursos.AgregarAsync(curso);
//                     insertados++;
//                 }
//             }

//             await _unitOfWork.CompleteAsync();

//             return Ok(new { mensaje = $"Se importaron {insertados} cursos nuevos." });
//         }
//     }
// }
