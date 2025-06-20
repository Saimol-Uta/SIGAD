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
        public async Task<ActionResult<InvestigacionDto>> Create([FromForm] CrearInvestigacionDto crearInvestigacionDto, IFormFile informe)
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
                
                // Establecer el header Content-Disposition para forzar la descarga con el nombre correcto
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                
                return File(fileContent, contentType, fileName);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Asocia una investigación a una solicitud
        /// </summary>
        /// <param name="asociarDto">Datos de la asociación</param>
        /// <returns>Resultado de la asociación</returns>
        [HttpPost("asociar-solicitud")]
        public async Task<ActionResult> AsociarInvestigacionASolicitud([FromBody] AsociarInvestigacionSolicitudDto asociarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _investigacionService.AsociarInvestigacionASolicitudAsync(asociarDto);
                if (!result)
                {
                    return BadRequest("No se pudo asociar la investigación a la solicitud. Verifique que ambos existan.");
                }

                return Ok("Investigación asociada a la solicitud exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Desasocia una investigación de una solicitud
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="investigacionId">ID de la investigación</param>
        /// <returns>Resultado de la desasociación</returns>
        [HttpDelete("desasociar-solicitud/{solicitudId}/{investigacionId}")]
        public async Task<ActionResult> DesasociarInvestigacionDeSolicitud(Guid solicitudId, int investigacionId)
        {
            try
            {
                var result = await _investigacionService.DesasociarInvestigacionDeSolicitudAsync(solicitudId, investigacionId);
                if (!result)
                {
                    return BadRequest("No se pudo desasociar la investigación de la solicitud.");
                }

                return Ok("Investigación desasociada de la solicitud exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
