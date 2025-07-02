using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/investigaciones/externa")]
    public class InvestigacionesExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;

        public InvestigacionesExternasController(
            ISgthSyncService sgth,
            ISutSyncService sut,
            IUnitOfWork unitOfWork)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarInvestigaciones(string cedula)
        {
            var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
            if (docente == null)
                return NotFound($"Docente con cédula {cedula} no encontrado.");

            var investigaciones = (await _sgth.ObtenerInvestigacionesAsync(cedula))
                                    .Concat(await _sut.ObtenerInvestigacionesAsync(cedula))
                                    .DistinctBy(i => i.ContenidoHash);

            int insertados = 0;

            foreach (var dto in investigaciones)
            {
                if (!await _unitOfWork.Investigaciones.ExistePorHashAsync(dto.ContenidoHash))
                {
                    var nueva = new Investigacion
                    {
                        Titulo = dto.Titulo,
                        FechaInicio = dto.FechaInicio,
                        FechaFinalizacion = dto.FechaFinalizacion,
                        RolEnInvestigacion = dto.RolEnInvestigacion,
                        MesesDeInvestigacion = dto.MesesDeInvestigacion,
                        InformeRuta = dto.InformeRuta,
                        ContenidoHash = dto.ContenidoHash,
                        DocenteCedula = docente.Cedula,
                        TipoProyecto = Enum.Parse<TipoInvestigacion>(dto.TipoProyecto),
                        MesesDeParticipacion = dto.MesesDeParticipacion,
                        UnidadVerificadora = dto.UnidadVerificadora
                    };

                    await _unitOfWork.Investigaciones.AgregarAsync(nueva);
                    insertados++;
                }
            }

            await _unitOfWork.CompleteAsync();
            return Ok(new { mensaje = $"Se importaron {insertados} investigaciones nuevas." });
        }
    }
}

