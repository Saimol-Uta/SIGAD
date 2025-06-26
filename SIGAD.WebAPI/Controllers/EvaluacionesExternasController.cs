using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/evaluaciones/externas")]
    public class EvaluacionesExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;

        public EvaluacionesExternasController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarEvaluaciones(string cedula)
        {
            var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
            if (docente == null)
                return NotFound($"Docente con cédula {cedula} no encontrado.");

            var externos = (await _sgth.ObtenerEvaluacionesAsync(cedula))
                            .Concat(await _sut.ObtenerEvaluacionesAsync(cedula))
                            .DistinctBy(e => e.ContenidoHash);

            int insertados = 0;

            foreach (var dto in externos)
            {
                bool existe = await _unitOfWork.Evaluaciones.ExistePorHashAsync(dto.ContenidoHash);
                if (!existe)
                {
                    var evaluacion = new EvaluacionDocente
                    {
                        PeriodoAcademico = dto.PeriodoAcademico,
                        FechaEvaluacion = dto.FechaEvaluacion,
                        PuntajePorcentual = dto.PuntajePorcentual,
                        InformeRuta = dto.InformeRuta,
                        ContenidoHash = dto.ContenidoHash,
                        DocenteCedula = docente.Cedula
                    };

                    await _unitOfWork.Evaluaciones.AgregarAsync(evaluacion);
                    insertados++;
                }
            }

            await _unitOfWork.CompleteAsync();

            return Ok(new { mensaje = $"Se importaron {insertados} evaluaciones nuevas." });
        }
    }
}
