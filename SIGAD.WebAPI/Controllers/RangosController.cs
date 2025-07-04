using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RangosController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RangosController> _logger;

        public RangosController(IUnitOfWork unitOfWork, ILogger<RangosController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // GET: api/rangos
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllRangos()
        {
            try
            {
                var rangos = (await _unitOfWork.Rangos.GetAllAsync()).OrderBy(r => r.Id);
                var rangosDto = rangos.Select(r => new RangoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    ArticulosRequeridos = r.ArticulosRequeridos,
                    AniosExperienciaRequeridos = r.AniosExperienciaRequeridos,
                    HorasCursoRequeridas = r.HorasCursoRequeridas,
                    MesesInvestigacionRequeridos = r.MesesInvestigacionRequeridos,
                    TesisDirigidasRequeridas = r.TesisDirigidasRequeridas,
                    PuntajePromedioEvaluacionesRequerido = r.PuntajePromedioEvaluacionesRequerido,

                    // Campos nuevos que faltaban
                    HorasCapacitacionPedagogicaRequeridas = r.HorasCapacitacionPedagogicaRequeridas,
                    HorasCapacitacionImpartidaRequeridas = r.HorasCapacitacionImpartidaRequeridas,
                    PublicacionesIdiomaExtranjeroRequeridas = r.PublicacionesIdiomaExtranjeroRequeridas,
                    ProyectosInternacionalesRequeridos = r.ProyectosInternacionalesRequeridos,
                    RequiereArticuloEnGradoActual = r.RequiereArticuloEnGradoActual,
                    PermiteCoordinacionProyectos = r.PermiteCoordinacionProyectos
                });
                return Ok(new { success = true, data = rangosDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rangos");
                return StatusCode(500, new { success = false, message = "Error interno del servidor", error = ex.Message });
            }
        }

        // GET: api/rangos/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRangoById(int id)
        {
            try
            {
                var rango = await _unitOfWork.Rangos.GetByIdAsync(id);
                if (rango == null) return NotFound();

                var rangoDto = new RangoDto
                {
                    Id = rango.Id,
                    Nombre = rango.Nombre,
                    ArticulosRequeridos = rango.ArticulosRequeridos,
                    AniosExperienciaRequeridos = rango.AniosExperienciaRequeridos,
                    HorasCursoRequeridas = rango.HorasCursoRequeridas,
                    MesesInvestigacionRequeridos = rango.MesesInvestigacionRequeridos,
                    TesisDirigidasRequeridas = rango.TesisDirigidasRequeridas,
                    PuntajePromedioEvaluacionesRequerido = rango.PuntajePromedioEvaluacionesRequerido
                };
                return Ok(rangoDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rango {RangoId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/rangos/disponibles/{rangoActualId}
        [HttpGet("disponibles/{rangoActualId}")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> GetRangosDisponiblesParaPromocion(int rangoActualId)
        {
            try
            {
                var rangos = await _unitOfWork.Rangos.GetRangosDisponiblesParaPromocionAsync(rangoActualId);
                var rangosDto = rangos.Select(r => new RangoDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    ArticulosRequeridos = r.ArticulosRequeridos,
                    AniosExperienciaRequeridos = r.AniosExperienciaRequeridos,
                    HorasCursoRequeridas = r.HorasCursoRequeridas,
                    MesesInvestigacionRequeridos = r.MesesInvestigacionRequeridos,
                    TesisDirigidasRequeridas = r.TesisDirigidasRequeridas,
                    PuntajePromedioEvaluacionesRequerido = r.PuntajePromedioEvaluacionesRequerido
                });
                return Ok(rangosDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rangos disponibles para promoción desde {RangoActualId}", rangoActualId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/rangos/validar/{docenteCedula}/{rangoSolicitadoId}
        [HttpGet("validar/{docenteCedula}/{rangoSolicitadoId}")]
        [Authorize]
        public async Task<IActionResult> ValidarRequisitos(string docenteCedula, int rangoSolicitadoId)
        {
            try
            {
                var validaciones = await _unitOfWork.Rangos.ValidarTodosRequisitosAsync(docenteCedula, rangoSolicitadoId);
                return Ok(validaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar requisitos para docente {Cedula} y rango {RangoId}", docenteCedula, rangoSolicitadoId);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}