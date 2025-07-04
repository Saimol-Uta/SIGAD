using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
using SIGAD.WebAPI.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tesis/externas")]
    public class TesisExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArchivoImportacionService _archivoService;

        public TesisExternasController(
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
        public async Task<IActionResult> ImportarTesis(string cedula)
        {
            try
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
                    // Validar que el DTO tenga datos válidos
                    if (string.IsNullOrEmpty(dto.ContenidoHash) || string.IsNullOrEmpty(dto.TituloTesis))
                        continue;

                    bool existe = await _unitOfWork.TesisDirigidas.ExistsByHashAsync(dto.ContenidoHash);
                    if (!existe)
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
                                    "tesis", 
                                    identificador
                                );
                            }
                            catch (Exception ex)
                            {
                                // Log error pero continúa con la importación sin archivo
                                Console.WriteLine($"Error procesando PDF para tesis {dto.TituloTesis}: {ex.Message}");
                            }
                        }

                        // Mapeo de estado externo a enum interno
                        string estadoDto = dto.Estado?.Trim() ?? "EnProceso";
                        switch (estadoDto)
                        {
                            case "Finalizada":
                                estadoDto = "Culminada";
                                break;
                            case "En Curso":
                                estadoDto = "EnProceso";
                                break;
                                // Agrega más casos si hay otros valores posibles
                        }

                        // Intenta convertir el string a enum, si falla usa EnProceso como valor por defecto
                        EstadoTesis estadoTesis = EstadoTesis.EnProceso;
                        Enum.TryParse<EstadoTesis>(estadoDto, true, out estadoTesis);

                        var tesis = new TesisDirigida
                        {
                            DocenteCedula = docente.Cedula,
                            NivelAcademico = NivelAcademicoHelper.ParseNivelAcademico(dto.NivelAcademico),
                            TituloTesis = dto.TituloTesis,
                            Estado = estadoTesis,
                            FechaInicio = dto.FechaInicio,
                            FechaFin = dto.FechaFin,
                            Institucion = dto.Institucion,
                            // Usar la ruta local si se procesó el PDF, sino la ruta original
                            CertificacionRuta = rutaArchivoLocal ?? dto.CertificacionRuta ?? "",
                            ContenidoHash = dto.ContenidoHash
                        };

                        await _unitOfWork.TesisDirigidas.AddAsync(tesis);
                        insertadas++;
                    }
                }

                await _unitOfWork.CompleteAsync();

                return Ok(new { mensaje = $"Se importaron {insertadas} tesis nuevas." });
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