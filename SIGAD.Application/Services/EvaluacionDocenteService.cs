using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SIGAD.Application.Services
{
    public class EvaluacionDocenteService : IEvaluacionDocenteService
    {
        private readonly IEvaluacionDocenteRepository _evaluacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsPath;

        public EvaluacionDocenteService(
            IEvaluacionDocenteRepository evaluacionRepository,
            IDocenteRepository docenteRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _evaluacionRepository = evaluacionRepository;
            _docenteRepository = docenteRepository;
            _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
            _uploadsPath = configuration["FileStorage:EvaluacionesPath"] ?? "uploads/evaluaciones";

            // Crear directorio si no existe
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
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
            string archivoRuta = string.Empty;
            string contenidoHash = string.Empty;

            if (archivo != null && archivo.Length > 0)
            {
                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                archivoRuta = ruta;
                contenidoHash = hash;
            }

            var evaluacion = new EvaluacionDocente
            {
                PeriodoAcademico = createDto.PeriodoAcademico,
                FechaEvaluacion = createDto.FechaEvaluacion,
                PuntajePorcentual = createDto.PuntajePorcentual,
                DocenteCedula = createDto.DocenteCedula,
                InformeRuta = archivoRuta,
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
                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(evaluacion.InformeRuta))
                {
                    var rutaFisica = Path.Combine(_uploadsPath, Path.GetFileName(evaluacion.InformeRuta));
                    if (File.Exists(rutaFisica))
                    {
                        File.Delete(rutaFisica);
                    }
                }

                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                evaluacion.InformeRuta = ruta;
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

            // Eliminar archivo si existe
            if (!string.IsNullOrEmpty(evaluacion.InformeRuta))
            {
                var rutaFisica = Path.Combine(_uploadsPath, Path.GetFileName(evaluacion.InformeRuta));
                if (File.Exists(rutaFisica))
                {
                    File.Delete(rutaFisica);
                }
            }

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
                return false;
            }

            await _evaluacionRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.EvaluacionId);
            await _unitOfWork.SaveChangesAsync();
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
            if (evaluacion == null || string.IsNullOrEmpty(evaluacion.InformeRuta))
            {
                return null;
            }

            var rutaFisica = Path.Combine(_uploadsPath, Path.GetFileName(evaluacion.InformeRuta));
            if (!File.Exists(rutaFisica))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(rutaFisica);
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
                ContenidoHash = e.ContenidoHash,
                DocenteNombreCompleto = e.Docente?.NombreCompleto ?? ""
            });
        }

        private async Task<(string rutaRelativa, string hash)> GuardarArchivoAsync(IFormFile archivo)
        {
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido. Use: PDF, JPG, JPEG, PNG, DOC, DOCX");

            if (archivo.Length > 25 * 1024 * 1024) // 25MB
                throw new ArgumentException("El archivo no puede exceder los 25MB");

            // Generar nombre único
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            // Guardar archivo físicamente
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Calcular hash
            string contentHash;
            using (var stream = File.OpenRead(filePath))
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(stream);
                contentHash = Convert.ToHexString(hashBytes);
            }

            // Ruta relativa para la base de datos
            var relativePath = Path.Combine("evaluaciones", fileName).Replace("\\", "/");

            return (relativePath, contentHash);
        }

        private async Task<string> CalcularHashArchivoAsync(string rutaArchivo)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(rutaArchivo);
            var hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
            return Convert.ToHexString(hashBytes);
        }

        private static EvaluacionDocenteDto MapToDto(EvaluacionDocente evaluacion)
        {
            var nombreCompleto = evaluacion.Docente != null
                ? $"{evaluacion.Docente.Nombre1} {evaluacion.Docente.Nombre2} {evaluacion.Docente.Apellido1} {evaluacion.Docente.Apellido2}".Trim()
                : string.Empty;

            return new EvaluacionDocenteDto
            {
                Id = evaluacion.Id,
                PeriodoAcademico = evaluacion.PeriodoAcademico,
                FechaEvaluacion = evaluacion.FechaEvaluacion,
                PuntajePorcentual = evaluacion.PuntajePorcentual,
                InformeRuta = evaluacion.InformeRuta,
                ContenidoHash = evaluacion.ContenidoHash,
                DocenteCedula = evaluacion.DocenteCedula,
                DocenteNombreCompleto = nombreCompleto
            };
        }
    }
}