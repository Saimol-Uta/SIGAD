using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
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
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly ILogger<CertificadosController> _logger;

        public CertificadosController(
            IAccionPersonalService accionPersonalService,
            ISolicitudAscensoRepository solicitudRepository,
            ILogger<CertificadosController> logger)
        {
            _accionPersonalService = accionPersonalService;
            _solicitudRepository = solicitudRepository;
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
        [Authorize(Policy = "RequireAdminRole")] // Fase 4: Política centralizada para rol administrador
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
        [Authorize] // Permitir acceso a todos los usuarios autenticados
        public async Task<IActionResult> GenerarAccionPersonalDesdeSolicitud(Guid solicitudId)
        {
            try
            {
                _logger.LogInformation($"Generando certificado de acción de personal para solicitud ID: {solicitudId}");

                // Obtener la solicitud con todos sus detalles
                var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(solicitudId);

                if (solicitud == null)
                {
                    _logger.LogWarning($"Solicitud no encontrada: {solicitudId}");
                    return NotFound(new { success = false, message = "Solicitud no encontrada" });
                }

                // Verificar que la solicitud esté aprobada
                if (solicitud.Estado != EstadoSolicitud.Aprobada)
                {
                    _logger.LogWarning($"La solicitud {solicitudId} no está aprobada. Estado actual: {solicitud.Estado}");
                    return BadRequest(new { success = false, message = "Solo se pueden generar certificados para solicitudes aprobadas" });
                }

                // Verificar que la solicitud tenga los datos necesarios
                if (solicitud.Docente == null || solicitud.RangoActual == null || solicitud.RangoSolicitado == null)
                {
                    _logger.LogWarning($"La solicitud {solicitudId} no tiene todos los datos necesarios para generar el certificado");
                    return BadRequest(new { success = false, message = "La solicitud no tiene todos los datos necesarios" });
                }

                // Crear el DTO para generar el certificado
                var datosAccionPersonal = new AccionPersonalDto
                {
                    NombreCompleto = solicitud.Docente.NombreCompleto,
                    Cedula = solicitud.Docente.Cedula,
                    RangoAnterior = solicitud.RangoActual.Nombre,
                    RangoNuevo = solicitud.RangoSolicitado.Nombre,
                    FechaSesion = solicitud.FechaAprobacionConsejo?.ToString("dd 'de' MMMM 'de' yyyy") ?? "N/A",
                    PeriodoConvocatoria = $"{solicitud.FechaCreacion.Year}-{(solicitud.FechaCreacion.Month <= 6 ? "01" : "02")}",
                    FechaEfectivaPromocion = solicitud.FechaResolucion?.ToString("dd 'de' MMMM 'de' yyyy") ?? DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"),
                    Anio = DateTime.Now.Year.ToString(),
                    Consecutivo = $"{solicitudId.ToString().Substring(0, 5)}",
                    SolicitudId = solicitudId
                };

                // Generar el PDF
                var pdfBytes = await _accionPersonalService.GenerarAccionPersonalPdfAsync(datosAccionPersonal);

                string nombreArchivo = $"accion_personal_{datosAccionPersonal.Cedula}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                _logger.LogInformation($"Certificado generado exitosamente para solicitud {solicitudId}: {nombreArchivo}");

                return File(pdfBytes, "application/pdf", nombreArchivo);
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