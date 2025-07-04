using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SIGAD.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CertificadosController : ControllerBase
    {
        private readonly IAccionPersonalService _accionPersonalService;
        private readonly ILogger<CertificadosController> _logger;

        public CertificadosController(
            IAccionPersonalService accionPersonalService,
            ILogger<CertificadosController> logger)
        {
            _accionPersonalService = accionPersonalService;
            _logger = logger;
        }

        /// <summary>
        /// Genera un certificado de acción de personal para un docente promovido
        /// </summary>
        /// <param name="datos">Datos para generar el certificado</param>
        /// <returns>Archivo PDF del certificado generado</returns>
        /// <response code="200">Retorna el archivo PDF generado</response>
        /// <response code="400">Si los datos proporcionados no son válidos</response>
        /// <response code="401">Si el usuario no está autenticado</response>
        /// <response code="500">Si ocurre un error interno</response>
        [HttpPost("accion-personal")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GenerarAccionPersonal([FromBody] AccionPersonalDto datos)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation($"Generando certificado de acción de personal para docente: {datos.NombreCompleto} ({datos.Cedula})");
                
                var pdfBytes = await _accionPersonalService.GenerarAccionPersonalPdfAsync(datos);
                
                string nombreArchivo = $"accion_personal_{datos.Cedula}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                
                _logger.LogInformation($"Certificado generado exitosamente: {nombreArchivo}");
                
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Error de validación al generar certificado: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar certificado de acción de personal para {datos?.Cedula}");
                return StatusCode(500, new { success = false, message = "Error interno al generar el certificado" });
            }
        }

        /// <summary>
        /// Genera un certificado de acción de personal a partir de una solicitud de ascenso aprobada
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud de ascenso</param>
        /// <returns>Archivo PDF del certificado generado</returns>
        /// <response code="200">Retorna el archivo PDF generado</response>
        /// <response code="400">Si los datos proporcionados no son válidos</response>
        /// <response code="401">Si el usuario no está autenticado</response>
        /// <response code="404">Si la solicitud no existe o no está aprobada</response>
        /// <response code="500">Si ocurre un error interno</response>
        [HttpGet("accion-personal/solicitud/{solicitudId}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GenerarAccionPersonalDesdeSolicitud(Guid solicitudId)
        {
            try
            {
                _logger.LogInformation($"Generando certificado de acción de personal para solicitud ID: {solicitudId}");
                
                // Aquí se implementaría la lógica para obtener los datos de la solicitud
                // y convertirlos en un AccionPersonalDto
                
                // Por ahora, retornamos un error indicando que esta funcionalidad no está implementada
                return StatusCode(501, new { success = false, message = "Funcionalidad no implementada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al generar certificado para solicitud ID: {solicitudId}");
                return StatusCode(500, new { success = false, message = "Error interno al generar el certificado" });
            }
        }

        /// <summary>
        /// Genera un certificado de acción de personal para pruebas
        /// </summary>
        /// <returns>Archivo PDF del certificado</returns>
        [HttpGet("prueba")]
        [AllowAnonymous] // Permitir acceso sin autenticación para pruebas
        public async Task<IActionResult> GenerarCertificadoPrueba()
        {
            try
            {
                _logger.LogInformation("Generando certificado de prueba");
                
                // Datos de prueba para el certificado
                var datosAccionPersonal = new AccionPersonalDto
                {
                    NombreCompleto = "JUAN CARLOS PÉREZ RODRÍGUEZ",
                    Cedula = "1712345678",
                    RangoAnterior = "PROFESOR AUXILIAR 2",
                    RangoNuevo = "PROFESOR AGREGADO 1",
                    FechaSesion = "15 de julio de 2023",
                    PeriodoConvocatoria = "2023-01",
                    FechaEfectivaPromocion = "01 de agosto de 2023",
                    Anio = "2023",
                    Consecutivo = "00123"
                };

                // Generar el PDF
                var pdfBytes = await _accionPersonalService.GenerarAccionPersonalPdfAsync(datosAccionPersonal);

                _logger.LogInformation("Certificado de prueba generado exitosamente");
                
                // Devolver el archivo para descarga
                return File(pdfBytes, "application/pdf", $"accion_personal_prueba_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar certificado de prueba");
                return BadRequest($"Error al generar el certificado: {ex.Message}");
            }
        }

        /// <summary>
        /// Genera un certificado de acción de personal con datos personalizados
        /// </summary>
        /// <param name="datos">Datos para el certificado</param>
        /// <returns>Archivo PDF del certificado</returns>
        [HttpPost]
        public async Task<IActionResult> GenerarCertificado([FromBody] AccionPersonalDto datos)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Generar el PDF
                var pdfBytes = await _accionPersonalService.GenerarAccionPersonalPdfAsync(datos);

                // Devolver el archivo para descarga
                return File(pdfBytes, "application/pdf", $"accion_personal_{datos.Cedula}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el certificado: {ex.Message}");
            }
        }
    }
} 