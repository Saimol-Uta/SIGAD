using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.WebAPI.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/articulos/externos")]
    public class ArticulosExternosController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArchivoImportacionService _archivoService;

        public ArticulosExternosController(ISgthSyncService sgth, ISutSyncService sut, IUnitOfWork unitOfWork, IArchivoImportacionService archivoService)
        {
            _sgth = sgth;
            _sut = sut;
            _unitOfWork = unitOfWork;
            _archivoService = archivoService;
        }

        [HttpPost("importar/{cedula}")]
        public async Task<IActionResult> ImportarArticulos(string cedula)
        {
            try
            {
                var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
                if (docente == null)
                    return NotFound($"Docente con cédula {cedula} no encontrado.");

                var externos = (await _sgth.ObtenerArticulosAsync(cedula))
                                .Concat(await _sut.ObtenerArticulosAsync(cedula))
                                .DistinctBy(a => a.ContenidoHash);

                int insertados = 0;

                foreach (var dto in externos)
                {
                    // Validar que el DTO tenga datos válidos
                    if (string.IsNullOrEmpty(dto.ContenidoHash) || string.IsNullOrEmpty(dto.DOI))
                        continue;

                    bool existe = await _unitOfWork.Articulos.ExistePorHashAsync(dto.ContenidoHash);
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
                                    "articulos", 
                                    identificador
                                );
                            }
                            catch (Exception ex)
                            {
                                // Log error pero continúa con la importación sin archivo
                                Console.WriteLine($"Error procesando PDF para artículo {dto.DOI}: {ex.Message}");
                            }
                        }

                        var articulo = new Articulo
                        {
                            DOI = dto.DOI,
                            Titulo = dto.Titulo,
                            Revista = dto.Revista,
                            AnioPublicacion = dto.AnioPublicacion,
                            IdiomaPublicacion = dto.IdiomaPublicacion ?? "No especificado",
                            // Usar la ruta local si se procesó el PDF, sino la ruta original
                            ArchivoRuta = rutaArchivoLocal ?? dto.ArchivoRuta ?? "",
                            ContenidoHash = dto.ContenidoHash,
                            DocenteCedula = docente.Cedula,
                            UnidadVerificadora = dto.UnidadVerificadora,
                            Verificado = dto.Verificado,
                            FechaVerificacion = dto.FechaVerificacion
                        };

                        await _unitOfWork.Articulos.AgregarAsync(articulo);
                        insertados++;
                    }
                }

                await _unitOfWork.CompleteAsync();

                return Ok(new { mensaje = $"Se importaron {insertados} artículos nuevos." });
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
