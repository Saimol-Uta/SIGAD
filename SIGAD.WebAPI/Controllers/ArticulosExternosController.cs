using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/articulos/externos")]
    public class ArticulosExternosController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;

        public ArticulosExternosController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarArticulos(string cedula)
        {
            var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
            if (docente == null)
                return NotFound($"Docente con cédula {cedula} no encontrado.");

            var externos = (await _sgth.ObtenerArticulosAsync(cedula))
                            .Concat(await _sut.ObtenerArticulosAsync(cedula))
                            .DistinctBy(a => a.ContenidoHash);

            int insertados = 0;

            foreach (var dto in externos)
            {
                bool existe = await _unitOfWork.Articulos.ExistePorHashAsync(dto.ContenidoHash);
                if (!existe)
                {
                    var articulo = new Articulo
                    {
                        DOI = dto.DOI,
                        Titulo = dto.Titulo,
                        Revista = dto.Revista,
                        AnioPublicacion = dto.AnioPublicacion,
                        ArchivoRuta = dto.ArchivoRuta,
                        ContenidoHash = dto.ContenidoHash,
                        DocenteCedula = docente.Cedula
                    };

                    await _unitOfWork.Articulos.AgregarAsync(articulo);
                    insertados++;
                }
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { mensaje = $"Se importaron {insertados} artículos nuevos." });
        }
    }
}
