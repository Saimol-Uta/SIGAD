using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System.Security.Cryptography;

namespace SIGAD.Application.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IOrganizacionRepository _organizacionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly string _fileStoragePath;

        public CursoService(
            ICursoRepository cursoRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IOrganizacionRepository organizacionRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _cursoRepository = cursoRepository;
            _solicitudRepository = solicitudRepository;
            _organizacionRepository = organizacionRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _fileStoragePath = _configuration["FileStorage:CursosPath"] ?? "Files/Cursos";

            // Crear directorio si no existe
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
        }

        public async Task<IEnumerable<CursoDto>> GetAllAsync()
        {
            var cursos = await _cursoRepository.GetAllAsync();
            return cursos.Select(MapToDto);
        }

        public async Task<CursoDto?> GetByIdAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            return curso != null ? MapToDto(curso) : null;
        }

        public async Task<IEnumerable<CursoDto>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            var cursos = await _cursoRepository.GetByDocenteCedulaAsync(docenteCedula);
            return cursos.Select(MapToDto);
        }

        public async Task<IEnumerable<CursoDto>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            var cursos = await _cursoRepository.GetBySolicitudIdAsync(solicitudId);
            return cursos.Select(MapToDto);
        }

        public async Task<CursoDto> CreateAsync(CrearCursoDto crearCursoDto, IFormFile certificado)
        {
            // Validar archivo
            if (certificado == null || certificado.Length == 0)
                throw new ArgumentException("El certificado es requerido");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var extension = Path.GetExtension(certificado.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido. Use: PDF, JPG, JPEG, PNG, DOC, DOCX");

            if (certificado.Length > 10 * 1024 * 1024) // 10MB
                throw new ArgumentException("El archivo no puede exceder los 10MB");

            // Verificar que la solicitud existe
            if (!await _solicitudRepository.ExistsAsync(crearCursoDto.SolicitudId))
                throw new ArgumentException("La solicitud especificada no existe");

            // Buscar o crear la organización
            var organizacion = await _organizacionRepository.GetByNombreAsync(crearCursoDto.OrganizacionNombre);
            if (organizacion == null)
            {
                // Crear nueva organización
                organizacion = new Organizacion
                {
                    Nombre = crearCursoDto.OrganizacionNombre,
                    TipoOrganizacion = "Institución Educativa" // Tipo por defecto para cursos
                };
                await _organizacionRepository.AddAsync(organizacion);
                await _unitOfWork.SaveChangesAsync();
            }

            // Generar hash del contenido
            string contentHash;
            using (var stream = certificado.OpenReadStream())
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(stream);
                    contentHash = Convert.ToHexString(hashBytes);
                }
            }

            // Generar nombre único para el archivo
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_fileStoragePath, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await certificado.CopyToAsync(stream);
            }

            // Crear entidad
            var curso = new Curso
            {
                Nombre = crearCursoDto.Nombre,
                OrganizacionId = organizacion.Id,
                NumeroHoras = crearCursoDto.NumeroHoras,
                FechaFinalizacion = crearCursoDto.FechaFinalizacion,
                DocenteCedula = crearCursoDto.DocenteCedula,
                CertificadoRuta = filePath,
                ContenidoHash = contentHash
            };

            await _cursoRepository.AddAsync(curso);

            // Asociar automáticamente a la solicitud
            await _cursoRepository.AddToSolicitudAsync(crearCursoDto.SolicitudId, curso.Id);

            // Obtener curso completo con relaciones
            var cursoCreado = await _cursoRepository.GetByIdAsync(curso.Id);
            return MapToDto(cursoCreado!);
        }

        public async Task<CursoDto> UpdateAsync(ActualizarCursoDto actualizarCursoDto, IFormFile? certificado = null)
        {
            var cursoExistente = await _cursoRepository.GetByIdAsync(actualizarCursoDto.Id);
            if (cursoExistente == null)
                throw new ArgumentException("Curso no encontrado");

            // Actualizar propiedades básicas
            cursoExistente.Nombre = actualizarCursoDto.Nombre;
            cursoExistente.OrganizacionId = actualizarCursoDto.OrganizacionId;
            cursoExistente.NumeroHoras = actualizarCursoDto.NumeroHoras;
            cursoExistente.FechaFinalizacion = actualizarCursoDto.FechaFinalizacion;
            cursoExistente.DocenteCedula = actualizarCursoDto.DocenteCedula;

            // Si se proporciona nuevo certificado
            if (certificado != null && certificado.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                var extension = Path.GetExtension(certificado.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(extension))
                    throw new ArgumentException("Tipo de archivo no permitido. Use: PDF, JPG, JPEG, PNG, DOC, DOCX");

                if (certificado.Length > 10 * 1024 * 1024) // 10MB
                    throw new ArgumentException("El archivo no puede exceder los 10MB");

                // Eliminar archivo anterior
                if (File.Exists(cursoExistente.CertificadoRuta))
                {
                    File.Delete(cursoExistente.CertificadoRuta);
                }

                // Generar nuevo hash
                string contentHash;
                using (var stream = certificado.OpenReadStream())
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var hashBytes = sha256.ComputeHash(stream);
                        contentHash = Convert.ToHexString(hashBytes);
                    }
                }

                // Guardar nuevo archivo
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_fileStoragePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await certificado.CopyToAsync(stream);
                }

                cursoExistente.CertificadoRuta = filePath;
                cursoExistente.ContenidoHash = contentHash;
            }

            await _cursoRepository.UpdateAsync(cursoExistente);

            // Obtener curso actualizado con relaciones
            var cursoActualizado = await _cursoRepository.GetByIdAsync(actualizarCursoDto.Id);
            return MapToDto(cursoActualizado!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null)
                return false;

            // Eliminar archivo asociado
            if (File.Exists(curso.CertificadoRuta))
            {
                File.Delete(curso.CertificadoRuta);
            }

            await _cursoRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _cursoRepository.ExistsAsync(id);
        }

        public async Task<bool> AddToSolicitudAsync(AsociarCursoSolicitudDto asociarDto)
        {
            // Verificar que existan el curso y la solicitud
            if (!await _cursoRepository.ExistsAsync(asociarDto.CursoId))
                return false;

            if (!await _solicitudRepository.ExistsAsync(asociarDto.SolicitudId))
                return false;

            try
            {
                await _cursoRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.CursoId);
                return true;
            }
            catch
            {
                return false; // Puede fallar si ya existe la asociación
            }
        }

        public async Task<bool> RemoveFromSolicitudAsync(AsociarCursoSolicitudDto asociarDto)
        {
            try
            {
                await _cursoRepository.RemoveFromSolicitudAsync(asociarDto.SolicitudId, asociarDto.CursoId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(byte[] FileContent, string ContentType, string FileName)> DownloadCertificadoAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null || !File.Exists(curso.CertificadoRuta))
                throw new FileNotFoundException("Certificado no encontrado");

            var fileContent = await File.ReadAllBytesAsync(curso.CertificadoRuta);
            var extension = Path.GetExtension(curso.CertificadoRuta).ToLowerInvariant();
            
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            var fileName = $"Certificado_Curso_{id}_{curso.Nombre.Replace(" ", "_")}{extension}";

            return (fileContent, contentType, fileName);
        }



        public async Task<IEnumerable<VerCursoDto>> GetAllSimplifiedAsync()
        {
            var cursos = await _cursoRepository.GetAllAsync();
            return cursos.Select(MapToVerDto);
        }

        public async Task<IEnumerable<VerCursoDto>> GetByDocenteCedulaSimplifiedAsync(string docenteCedula)
        {
            var cursos = await _cursoRepository.GetByDocenteCedulaAsync(docenteCedula);
            return cursos.Select(MapToVerDto);
        }

        private static CursoDto MapToDto(Curso curso)
        {
            return new CursoDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                NombreOrganizacion = curso.Organizacion?.Nombre ?? "Sin organización",
                TipoOrganizacion = curso.Organizacion?.TipoOrganizacion ?? "No especificado",
                NumeroHoras = curso.NumeroHoras,
                FechaFinalizacion = curso.FechaFinalizacion,
                NombreDocente = curso.Docente != null 
                    ? $"{curso.Docente.Nombre1} {curso.Docente.Apellido1}" 
                    : "Docente no encontrado",
                DocenteCedula = curso.DocenteCedula,
                CertificadoRuta = curso.CertificadoRuta,
                ContenidoHash = curso.ContenidoHash,
                OrganizacionId = curso.OrganizacionId
            };
        }

        private static VerCursoDto MapToVerDto(Curso curso)
        {
            return new VerCursoDto
            {
                Id = curso.Id,
                Nombre = curso.Nombre,
                NombreOrganizacion = curso.Organizacion?.Nombre ?? "Sin organización",
                NumeroHoras = curso.NumeroHoras,
                FechaFinalizacion = curso.FechaFinalizacion,
                NombreDocente = curso.Docente != null 
                    ? $"{curso.Docente.Nombre1} {curso.Docente.Apellido1}" 
                    : "Docente no encontrado",
                DocenteCedula = curso.DocenteCedula,
                TieneCertificado = !string.IsNullOrEmpty(curso.CertificadoRuta) && File.Exists(curso.CertificadoRuta)
            };
        }
    }
} 