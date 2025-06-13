using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvestigacionesController : ControllerBase
    {
        private readonly IInvestigacionService _investigacionService;

        public InvestigacionesController(IInvestigacionService investigacionService)
        {
            _investigacionService = investigacionService;
        }

        /// <summary>
        /// Obtiene todas las investigaciones
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetAll()
        {
            var investigaciones = await _investigacionService.GetAllAsync();
            return Ok(investigaciones);
        }

        /// <summary>
        /// Obtiene una investigación por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InvestigacionDto>> GetById(int id)
        {
            var investigacion = await _investigacionService.GetByIdAsync(id);
            if (investigacion == null)
                return NotFound($"Investigación con ID {id} no encontrada");

            return Ok(investigacion);
        }

        /// <summary>
        /// Obtiene las investigaciones de un docente específico
        /// </summary>
        [HttpGet("docente/{cedula}")]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetByDocente(string cedula)
        {
            var investigaciones = await _investigacionService.GetByDocenteCedulaAsync(cedula);
            return Ok(investigaciones);
        }

        /// <summary>
        /// Obtiene las investigaciones asociadas a una solicitud específica
        /// </summary>
        [HttpGet("solicitud/{solicitudId}")]
        public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetBySolicitud(Guid solicitudId)
        {
            var investigaciones = await _investigacionService.GetBySolicitudIdAsync(solicitudId);
            return Ok(investigaciones);
        }

        /// <summary>
        /// Crea una nueva investigación con archivo de informe (se asocia automáticamente a la solicitud)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InvestigacionDto>> Create([FromForm] CrearInvestigacionDto crearInvestigacionDto, IFormFile? informe)
        {
            try
            {
                var investigacionCreada = await _investigacionService.CreateAsync(crearInvestigacionDto, informe);
                return CreatedAtAction(nameof(GetById), new { id = investigacionCreada.Id }, investigacionCreada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza una investigación existente (solo datos básicos, no el archivo)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<InvestigacionDto>> Update(int id, [FromBody] ActualizarInvestigacionDto actualizarInvestigacionDto)
        {
            try
            {
                var investigacionActualizada = await _investigacionService.UpdateAsync(id, actualizarInvestigacionDto);
                if (investigacionActualizada == null)
                    return NotFound($"Investigación con ID {id} no encontrada");

                return Ok(investigacionActualizada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Elimina una investigación por ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var eliminada = await _investigacionService.DeleteAsync(id);
            if (!eliminada)
                return NotFound($"Investigación con ID {id} no encontrada");

            return NoContent();
        }

        /// <summary>
        /// Obtiene una vista simplificada de todas las investigaciones
        /// </summary>
        [HttpGet("ver")]
        public async Task<ActionResult<IEnumerable<VerInvestigacionDto>>> GetVerInvestigaciones()
        {
            var investigaciones = await _investigacionService.GetVerInvestigacionesAsync();
            return Ok(investigaciones);
        }

        /// <summary>
        /// Descarga el informe de una investigación
        /// </summary>
        [HttpGet("{id}/descargar-informe")]
        public async Task<ActionResult> DownloadInforme(int id)
        {
            try
            {
                var (fileContent, contentType, fileName) = await _investigacionService.DownloadInformeAsync(id);
                return File(fileContent, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
