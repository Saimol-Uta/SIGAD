using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.WebAPI.Services;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/experiencias/externas")]
    public class ExperienciasExternasController : ControllerBase
    {
        private readonly ISgthSyncService _sgth;
        private readonly ISutSyncService _sut;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IArchivoImportacionService _archivoService;

        public ExperienciasExternasController(
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
        public async Task<IActionResult> ImportarExperiencias(string cedula)
        {
            try
            {
                var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
                if (docente == null)
                    return NotFound($"Docente con cédula {cedula} no encontrado.");

                var externos = (await _sgth.ObtenerExperienciasAsync(cedula))
                                .Concat(await _sut.ObtenerExperienciasAsync(cedula))
                                .DistinctBy(e => e.ContenidoHash);

                Console.WriteLine($"Encontradas {externos.Count()} experiencias externas para cédula {cedula}");

                int insertados = 0;

                foreach (var dto in externos)
                {
                    Console.WriteLine($"Procesando experiencia: {dto.Organizacion}, PDF: {(dto.PdfDocumento?.Length ?? 0)} bytes");
                    
                    // Validar que el DTO tenga datos válidos
                    if (string.IsNullOrEmpty(dto.ContenidoHash) || string.IsNullOrEmpty(dto.Organizacion))
                        continue;

                    bool existe = await _unitOfWork.Experiencias.ExistePorHashAsync(dto.ContenidoHash);
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
                                    "experiencias", 
                                    identificador
                                );
                                Console.WriteLine($"PDF procesado exitosamente para experiencia {dto.Organizacion}. Ruta: {rutaArchivoLocal}");
                            }
                            catch (Exception ex)
                            {
                                // Log error pero continúa con la importación sin archivo
                                Console.WriteLine($"Error procesando PDF para experiencia {dto.Organizacion}: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"No hay PDF para experiencia {dto.Organizacion}");
                        }

                        // Buscar o crear la organización
                        var organizacion = await _unitOfWork.Organizaciones.ObtenerPorNombreAsync(dto.Organizacion);
                        if (organizacion == null)
                        {
                            organizacion = new Organizacion { Nombre = dto.Organizacion };
                            await _unitOfWork.Organizaciones.AgregarAsync(organizacion);
                            await _unitOfWork.CompleteAsync(); // Guardar para obtener el ID
                        }

                        var experiencia = new ExperienciaLaboral
                        {
                            OrganizacionId = organizacion.Id,
                            Cargo = dto.Cargo,
                            FechaInicio = dto.FechaInicio,
                            FechaFin = dto.FechaFin,
                            // Solo usar ruta local si se procesó el PDF, sino dejar vacío
                            CertificadoRuta = rutaArchivoLocal ?? "",
                            ContenidoHash = dto.ContenidoHash,
                            DocenteCedula = docente.Cedula
                        };

                        Console.WriteLine($"Guardando experiencia {dto.Organizacion} con ruta: '{experiencia.CertificadoRuta}'");

                        await _unitOfWork.Experiencias.AgregarAsync(experiencia);
                        insertados++;
                    }
                }

                await _unitOfWork.CompleteAsync();

                return Ok(new { mensaje = $"Se importaron {insertados} experiencias laborales nuevas." });
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
