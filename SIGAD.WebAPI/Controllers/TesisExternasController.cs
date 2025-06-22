using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tesis/externas")]
    public class TesisExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;

        public TesisExternasController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarTesis(string cedula)
        {
            var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
            if (docente == null)
                return NotFound($"Docente con cédula {cedula} no encontrado.");

            var externas = (await _sgth.ObtenerTesisDirigidasAsync(cedula))
                            .Concat(await _sut.ObtenerTesisDirigidasAsync(cedula))
                            .DistinctBy(t => t.ContenidoHash);

            int insertadas = 0;

            foreach (var dto in externas)
            {
                bool existe = await _unitOfWork.TesisDirigidas.ExistsByHashAsync(dto.ContenidoHash);
                if (!existe)
                {
                    var tesis = new TesisDirigida
                    {
                        DocenteCedula = docente.Cedula,
                        NivelAcademico = Enum.Parse<NivelAcademico>(dto.NivelAcademico),
                        TituloTesis = dto.TituloTesis,
                        Estado = Enum.Parse<EstadoTesis>(dto.Estado),
                        FechaInicio = dto.FechaInicio,
                        FechaFin = dto.FechaFin,
                        Institucion = dto.Institucion,
                        CertificacionRuta = dto.CertificacionRuta,
                        ContenidoHash = dto.ContenidoHash
                    };

                    await _unitOfWork.TesisDirigidas.AddAsync(tesis);
                    insertadas++;
                }
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { mensaje = $"Se importaron {insertadas} tesis nuevas." });
        }
    }
}
