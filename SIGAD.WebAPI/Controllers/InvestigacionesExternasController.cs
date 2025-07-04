using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
using SIGAD.WebAPI.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/investigaciones/externa")]
    public class InvestigacionesExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArchivoImportacionService _archivoService;

        public InvestigacionesExternasController(
            ISgthSyncService sgth,
            ISutSyncService sut,
            IUnitOfWork unitOfWork,
            IArchivoImportacionService archivoService)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
            _archivoService = archivoService;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarInvestigaciones(string cedula)
        {
            try
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
                    // Validar que el DTO tenga datos válidos
                    if (string.IsNullOrEmpty(dto.ContenidoHash) || string.IsNullOrEmpty(dto.Titulo))
                        continue;

                    if (!await _unitOfWork.Investigaciones.ExistePorHashAsync(dto.ContenidoHash))
                    {
                        // Procesar y guardar PDF si existe
                        string? rutaArchivoLocal = null;
                        if (dto.PdfDocumento != null && dto.PdfDocumento.Length > 0)
                        {
                            try
                            {
                                var identificador = $"{dto.DocenteCedula}_{dto.ContenidoHash.Substring(0, Math.Min(8, dto.ContenidoHash.Length))}";
                                rutaArchivoLocal = await _archivoService.ProcesarYGuardarPdfAsync(
                                    dto.PdfDocumento, 
                                    "investigaciones", 
                                    identificador
                                );
                            }
                            catch (Exception ex)
                            {
                                // Log error pero continúa con la importación sin archivo
                                Console.WriteLine($"Error procesando PDF para investigación {dto.Titulo}: {ex.Message}");
                            }
                        }

                        var nueva = new Investigacion
                        {
                            Titulo = dto.Titulo,
                            FechaInicio = dto.FechaInicio,
                            FechaFinalizacion = dto.FechaFinalizacion,
                            RolEnInvestigacion = dto.RolEnInvestigacion,
                            MesesDeInvestigacion = dto.MesesDeInvestigacion,
                            // Usar la ruta local si se procesó el PDF, sino la ruta original
                            InformeRuta = rutaArchivoLocal ?? dto.InformeRuta ?? "",
                            ContenidoHash = dto.ContenidoHash,
                            DocenteCedula = docente.Cedula,
                            TipoProyecto = !string.IsNullOrEmpty(dto.TipoProyecto) ? Enum.Parse<TipoInvestigacion>(dto.TipoProyecto) : TipoInvestigacion.Aplicada,
                            MesesDeParticipacion = dto.MesesDeParticipacion,
                            UnidadVerificadora = dto.UnidadVerificadora,
                            EsInternacional = dto.EsInternacional
                        };

                        await _unitOfWork.Investigaciones.AgregarAsync(nueva);
                        insertados++;
                    }
                }

                await _unitOfWork.CompleteAsync();
                return Ok(new { mensaje = $"Se importaron {insertados} investigaciones nuevas." });
            }
            catch (Exception ex)
            {
                // Muestra el mensaje de la excepción interna si existe, si no el mensaje principal
                var error = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, title = "Error interno", message = error });
            }
        }
    }
}

