using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;

namespace SIGAD.Application.Services
{
    public class EvaluacionDocenteService : IEvaluacionDocenteService
    {
        private readonly IEvaluacionDocenteRepository _evaluacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public EvaluacionDocenteService(
            IEvaluacionDocenteRepository evaluacionRepository,
            IDocenteRepository docenteRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService)
        {
            _evaluacionRepository = evaluacionRepository;
            _docenteRepository = docenteRepository;
            _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<EvaluacionDocenteDto>> GetAllEvaluacionesAsync()
        {
            var evaluaciones = await _evaluacionRepository.GetAllAsync();
            return evaluaciones.Select(MapToDto);
        }

        public async Task<EvaluacionDocenteDto?> GetEvaluacionByIdAsync(int id)
        {
            var evaluacion = await _evaluacionRepository.GetByIdAsync(id);
            return evaluacion != null ? MapToDto(evaluacion) : null;
        }

        public async Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesByDocenteAsync(string docenteCedula)
        {
            var evaluaciones = await _evaluacionRepository.GetByDocenteCedulaAsync(docenteCedula);
            return evaluaciones.Select(MapToDto);
        }

        public async Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesBySolicitudAsync(Guid solicitudId)
        {
            var evaluaciones = await _evaluacionRepository.GetBySolicitudIdAsync(solicitudId);
            return evaluaciones.Select(MapToDto);
        }

        public async Task<EvaluacionDocenteDto> CreateEvaluacionAsync(CreateEvaluacionDocenteDto createDto, IFormFile? archivo)
        {
            // Validar que el docente existe
            var docenteExists = await _docenteRepository.ExistsByCedulaAsync(createDto.DocenteCedula);
            if (!docenteExists)
            {
                throw new ArgumentException("El docente especificado no existe");
            }

            // Procesar archivo si se proporciona
            string? rutaLocal = null;
            string? urlCloudinary = null;
            string contenidoHash = string.Empty;

            if (archivo != null && archivo.Length > 0)
            {
                var (ruta, cloudinaryUrl, hash) = await _fileStorageService.UploadFileAsync(archivo, "evaluaciones");
                rutaLocal = ruta;
                urlCloudinary = cloudinaryUrl;
                contenidoHash = hash;
            }

            var evaluacion = new EvaluacionDocente
            {
                PeriodoAcademico = createDto.PeriodoAcademico,
                FechaEvaluacion = createDto.FechaEvaluacion,
                PuntajePorcentual = createDto.PuntajePorcentual,
                DocenteCedula = createDto.DocenteCedula,
                InformeRuta = rutaLocal,
                UrlCloudinary = urlCloudinary,
                ContenidoHash = contenidoHash
            };

            await _evaluacionRepository.AddAsync(evaluacion);

            // Guardar cambios para generar el ID
            await _unitOfWork.SaveChangesAsync();

            // Si se especifica una solicitud, verificar que existe y asociar la evaluación
            if (createDto.SolicitudId.HasValue)
            {
                var solicitudExists = await _solicitudRepository.ExistsAsync(createDto.SolicitudId.Value);
                if (solicitudExists)
                {
                    await _evaluacionRepository.AddToSolicitudAsync(createDto.SolicitudId.Value, evaluacion.Id);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException($"La solicitud con ID {createDto.SolicitudId.Value} no existe");
                }
            }

            // Obtener la evaluación creada con datos del docente
            var evaluacionCreada = await _evaluacionRepository.GetByIdAsync(evaluacion.Id);
            return MapToDto(evaluacionCreada!);
        }

        public async Task<EvaluacionDocenteDto> UpdateEvaluacionAsync(int id, UpdateEvaluacionDocenteDto updateDto, IFormFile? archivo)
        {
            var evaluacion = await _evaluacionRepository.GetByIdAsync(id);
            if (evaluacion == null)
            {
                throw new ArgumentException("La evaluación no existe");
            }

            // Actualizar propiedades
            evaluacion.PeriodoAcademico = updateDto.PeriodoAcademico;
            evaluacion.FechaEvaluacion = updateDto.FechaEvaluacion;
            evaluacion.PuntajePorcentual = updateDto.PuntajePorcentual;

            // Procesar nuevo archivo si se proporciona
            if (archivo != null && archivo.Length > 0)
            {
                // Eliminar archivos anteriores si existen
                await _fileStorageService.EliminarArchivoDualAsync(evaluacion.InformeRuta, evaluacion.UrlCloudinary);

                // Subir nuevo archivo
                var (rutaLocal, urlCloudinary, hash) = await _fileStorageService.UploadFileAsync(archivo, "evaluaciones");
                evaluacion.InformeRuta = rutaLocal;
                evaluacion.UrlCloudinary = urlCloudinary;
                evaluacion.ContenidoHash = hash;
            }

            await _evaluacionRepository.UpdateAsync(evaluacion);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(evaluacion);
        }

        public async Task<bool> DeleteEvaluacionAsync(int id)
        {
            var evaluacion = await _evaluacionRepository.GetByIdAsync(id);
            if (evaluacion == null)
            {
                return false;
            }

            // Eliminar archivos de ambos almacenamientos
            await _fileStorageService.EliminarArchivoDualAsync(evaluacion.InformeRuta, evaluacion.UrlCloudinary);

            await _evaluacionRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AsociarEvaluacionASolicitudAsync(AsociarEvaluacionSolicitudDto asociarDto)
        {
            var evaluacionExists = await _evaluacionRepository.ExistsAsync(asociarDto.EvaluacionId);
            if (!evaluacionExists)
            {
                return false;
            }

            var solicitudExists = await _solicitudRepository.ExistsAsync(asociarDto.SolicitudId);
            if (!solicitudExists)
            {
                return false;
            }

            // Validar que la evaluación no esté ya usada en otra solicitud aprobada
            var estaYaUsada = await _evaluacionRepository.EstaEvaluacionYaUsadaAsync(asociarDto.EvaluacionId);
            if (estaYaUsada)
            {
                Console.WriteLine($"Debug - Service - Evaluación {asociarDto.EvaluacionId} ya está usada en otra solicitud");
                return false;
            }

            Console.WriteLine($"Debug - Service - Asociando evaluación {asociarDto.EvaluacionId} a solicitud {asociarDto.SolicitudId}");
            await _evaluacionRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.EvaluacionId);
            await _unitOfWork.SaveChangesAsync();
            Console.WriteLine($"Debug - Service - Asociación completada y guardada en BD");
            return true;
        }

        public async Task<bool> DesasociarEvaluacionDeSolicitudAsync(Guid solicitudId, int evaluacionId)
        {
            await _evaluacionRepository.RemoveFromSolicitudAsync(solicitudId, evaluacionId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<byte[]?> GetArchivoEvaluacionAsync(int id)
        {
            var evaluacion = await _evaluacionRepository.GetByIdAsync(id);
            if (evaluacion == null || (string.IsNullOrEmpty(evaluacion.InformeRuta) && string.IsNullOrEmpty(evaluacion.UrlCloudinary)))
            {
                return null;
            }

            // Obtener la mejor URL y usarla para descargar el archivo
            var mejorUrl = _fileStorageService.ObtenerMejorUrl(evaluacion.InformeRuta, evaluacion.UrlCloudinary);
            
            // Si la mejor URL es local, leer el archivo directamente
            if (!string.IsNullOrEmpty(evaluacion.InformeRuta) && mejorUrl.Contains("localhost"))
            {
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", evaluacion.InformeRuta.TrimStart('/'));
                if (File.Exists(rutaCompleta))
                {
                    return await File.ReadAllBytesAsync(rutaCompleta);
                }
            }

            // Para URLs de Cloudinary, se necesitaría un HttpClient para descargar
            // Por ahora, intentamos usar el archivo local como fallback
            if (!string.IsNullOrEmpty(evaluacion.InformeRuta))
            {
                var rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", evaluacion.InformeRuta.TrimStart('/'));
                if (File.Exists(rutaCompleta))
                {
                    return await File.ReadAllBytesAsync(rutaCompleta);
                }
            }

            return null;
        }

        public async Task<string?> GetNombreArchivoAsync(int id)
        {
            var evaluacion = await _evaluacionRepository.GetByIdAsync(id);
            return evaluacion?.InformeRuta;
        }

        public async Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesDisponiblesAsync(string docenteCedula, Guid? solicitudActualId = null)
        {
            var evaluacionesDisponibles = await _evaluacionRepository.GetEvaluacionesDisponiblesParaSolicitudAsync(docenteCedula, solicitudActualId);
            
            return evaluacionesDisponibles.Select(e => new EvaluacionDocenteDto
            {
                Id = e.Id,
                PeriodoAcademico = e.PeriodoAcademico,
                PuntajePorcentual = e.PuntajePorcentual,
                FechaEvaluacion = e.FechaEvaluacion,
                DocenteCedula = e.DocenteCedula,
                InformeRuta = e.InformeRuta,
                UrlCloudinary = e.UrlCloudinary,
                ContenidoHash = e.ContenidoHash,
                DocenteNombreCompleto = e.Docente?.NombreCompleto ?? ""
            });
        }

        public async Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesUsadasAsync(string docenteCedula)
        {
            var evaluacionesUsadas = await _evaluacionRepository.GetEvaluacionesUsadasEnSolicitudesAsync(docenteCedula);
            
            return evaluacionesUsadas.Select(e => new EvaluacionDocenteDto
            {
                Id = e.Id,
                PeriodoAcademico = e.PeriodoAcademico,
                PuntajePorcentual = e.PuntajePorcentual,
                FechaEvaluacion = e.FechaEvaluacion,
                DocenteCedula = e.DocenteCedula,
                InformeRuta = e.InformeRuta,
                UrlCloudinary = e.UrlCloudinary,
                ContenidoHash = e.ContenidoHash,
                DocenteNombreCompleto = e.Docente?.NombreCompleto ?? ""
            });
        }

        private static EvaluacionDocenteDto MapToDto(EvaluacionDocente evaluacion)
        {
            var nombreCompleto = evaluacion.Docente != null
                ? $"{evaluacion.Docente.Nombre1} {evaluacion.Docente.Nombre2} {evaluacion.Docente.Apellido1} {evaluacion.Docente.Apellido2}".Trim()
                : string.Empty;

            // Mapear solicitudes asociadas
            List<SolicitudBasicaDto>? solicitudes = null;
            string? solicitudIdPrincipal = null;

            if (evaluacion.EvaluacionesPorSolicitud?.Any() == true)
            {
                solicitudes = evaluacion.EvaluacionesPorSolicitud
                    .Where(eps => eps.Solicitud != null)
                    .Select(eps => new SolicitudBasicaDto
                    {
                        SolicitudId = eps.Solicitud!.Id.ToString(),
                        Estado = eps.Solicitud.Estado.ToString(),
                        FechaCreacion = eps.Solicitud.FechaCreacion
                    }).ToList();
                
                // Debug: Log información de solicitudes para esta evaluación
                Console.WriteLine($"Debug - Evaluación ID {evaluacion.Id} tiene {solicitudes.Count} solicitudes asociadas:");
                foreach (var sol in solicitudes)
                {
                    Console.WriteLine($"  - Solicitud ID: {sol.SolicitudId}, Estado: {sol.Estado}, Fecha: {sol.FechaCreacion}");
                }
                
                // Priorizar solicitud en estado Borrador o Enviada para mostrar como principal
                var solicitudPrincipal = solicitudes
                    .Where(s => s.Estado == "Borrador" || s.Estado == "Enviada")
                    .OrderByDescending(s => s.FechaCreacion)
                    .FirstOrDefault();
                
                // Si no hay solicitud en estados activos, buscar EnRevision también
                if (solicitudPrincipal == null)
                {
                    solicitudPrincipal = solicitudes
                        .Where(s => s.Estado == "EnRevision")
                        .OrderByDescending(s => s.FechaCreacion)
                        .FirstOrDefault();
                }
                
                // Si no hay solicitud activa, usar la más reciente
                solicitudIdPrincipal = solicitudPrincipal?.SolicitudId ?? solicitudes.FirstOrDefault()?.SolicitudId;
                
                // Debug: Log resultado del mapeo
                Console.WriteLine($"Debug - Evaluación ID {evaluacion.Id} - Solicitud principal seleccionada: {solicitudIdPrincipal}");
                if (solicitudPrincipal != null)
                {
                    Console.WriteLine($"  - Estado de solicitud principal: {solicitudPrincipal.Estado}");
                }
            }

            return new EvaluacionDocenteDto
            {
                Id = evaluacion.Id,
                PeriodoAcademico = evaluacion.PeriodoAcademico,
                FechaEvaluacion = evaluacion.FechaEvaluacion,
                PuntajePorcentual = evaluacion.PuntajePorcentual,
                InformeRuta = evaluacion.InformeRuta,
                UrlCloudinary = evaluacion.UrlCloudinary,
                ContenidoHash = evaluacion.ContenidoHash,
                DocenteCedula = evaluacion.DocenteCedula,
                DocenteNombreCompleto = nombreCompleto,
                SolicitudId = solicitudIdPrincipal,
                Solicitudes = solicitudes
            };
        }
    }
}