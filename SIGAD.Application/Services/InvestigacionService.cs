using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System.Security.Cryptography;

namespace SIGAD.Application.Services
{
    public class InvestigacionService : IInvestigacionService
    {
        private readonly IInvestigacionRepository _investigacionRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IConfiguration _configuration;
        private readonly string _fileStoragePath;

        public InvestigacionService(
            IInvestigacionRepository investigacionRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IConfiguration configuration)
        {
            _investigacionRepository = investigacionRepository;
            _solicitudRepository = solicitudRepository;
            _configuration = configuration;
            _fileStoragePath = _configuration["FileStorage:InvestigacionesPath"] ?? "Files/Investigaciones";

            // Crear directorio si no existe
            if (!Directory.Exists(_fileStoragePath))
            {
                Directory.CreateDirectory(_fileStoragePath);
            }
        }

        public async Task<IEnumerable<InvestigacionDto>> GetAllAsync()
        {
            var investigaciones = await _investigacionRepository.GetAllAsync();
            return investigaciones.Select(MapToDto);
        }

        public async Task<InvestigacionDto?> GetByIdAsync(int id)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            return investigacion != null ? MapToDto(investigacion) : null;
        }

        public async Task<IEnumerable<InvestigacionDto>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            var investigaciones = await _investigacionRepository.GetByDocenteCedulaAsync(docenteCedula);
            return investigaciones.Select(MapToDto);
        }

        public async Task<IEnumerable<InvestigacionDto>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            var investigaciones = await _investigacionRepository.GetBySolicitudIdAsync(solicitudId);
            return investigaciones.Select(MapToDto);
        }

        public async Task<InvestigacionDto> CreateAsync(CrearInvestigacionDto crearInvestigacionDto, IFormFile informe)
        {
            // Validar archivo
            if (informe == null || informe.Length == 0)
                throw new ArgumentException("El informe es requerido");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(informe.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido. Use: PDF, DOC, DOCX");

            if (informe.Length > 25 * 1024 * 1024) // 25MB para informes
                throw new ArgumentException("El archivo no puede exceder los 25MB");

            // Verificar que la solicitud existe
            if (!await _solicitudRepository.ExistsAsync(crearInvestigacionDto.SolicitudId))
                throw new ArgumentException("La solicitud especificada no existe");

            // Validar fechas
            if (crearInvestigacionDto.FechaFinalizacion <= crearInvestigacionDto.FechaInicio)
                throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio");

            // Generar hash del contenido
            string contentHash;
            using (var stream = informe.OpenReadStream())
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
                await informe.CopyToAsync(stream);
            }

            // Crear entidad
            var investigacion = new Investigacion
            {
                Titulo = crearInvestigacionDto.Titulo,
                FechaInicio = crearInvestigacionDto.FechaInicio,
                FechaFinalizacion = crearInvestigacionDto.FechaFinalizacion,
                RolEnInvestigacion = crearInvestigacionDto.RolEnInvestigacion,
                MesesDeInvestigacion = crearInvestigacionDto.MesesDeInvestigacion,
                DocenteCedula = crearInvestigacionDto.DocenteCedula,
                InformeRuta = filePath,
                ContenidoHash = contentHash
            };

            await _investigacionRepository.AddAsync(investigacion);

            // Asociar automáticamente a la solicitud
            await _investigacionRepository.AddToSolicitudAsync(crearInvestigacionDto.SolicitudId, investigacion.Id);

            // Obtener investigación completa con relaciones
            var investigacionCreada = await _investigacionRepository.GetByIdAsync(investigacion.Id);
            return MapToDto(investigacionCreada!);
        }

        public async Task<InvestigacionDto?> UpdateAsync(int id, ActualizarInvestigacionDto actualizarInvestigacionDto)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            if (investigacion == null)
                return null;

            // Validar fechas
            if (actualizarInvestigacionDto.FechaFinalizacion <= actualizarInvestigacionDto.FechaInicio)
                throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio");

            // Actualizar propiedades
            investigacion.Titulo = actualizarInvestigacionDto.Titulo;
            investigacion.FechaInicio = actualizarInvestigacionDto.FechaInicio;
            investigacion.FechaFinalizacion = actualizarInvestigacionDto.FechaFinalizacion;
            investigacion.RolEnInvestigacion = actualizarInvestigacionDto.RolEnInvestigacion;
            investigacion.MesesDeInvestigacion = actualizarInvestigacionDto.MesesDeInvestigacion;

            await _investigacionRepository.UpdateAsync(investigacion);

            // Obtener investigación actualizada con relaciones
            var investigacionActualizada = await _investigacionRepository.GetByIdAsync(id);
            return MapToDto(investigacionActualizada!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            if (investigacion == null)
                return false;

            // Eliminar archivo asociado
            if (File.Exists(investigacion.InformeRuta))
            {
                File.Delete(investigacion.InformeRuta);
            }

            await _investigacionRepository.DeleteAsync(id);
            return true;
        }

        public async Task<(byte[] FileContent, string ContentType, string FileName)> DownloadInformeAsync(int id)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            if (investigacion == null || !File.Exists(investigacion.InformeRuta))
                throw new FileNotFoundException("Informe no encontrado");

            var fileContent = await File.ReadAllBytesAsync(investigacion.InformeRuta);
            var extension = Path.GetExtension(investigacion.InformeRuta).ToLowerInvariant();
            
            var contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };

            var fileName = $"Informe_Investigacion_{id}_{investigacion.Titulo.Replace(" ", "_")}{extension}";

            return (fileContent, contentType, fileName);
        }

        public async Task<IEnumerable<VerInvestigacionDto>> GetVerInvestigacionesAsync()
        {
            var investigaciones = await _investigacionRepository.GetAllAsync();
            return investigaciones.Select(i => new VerInvestigacionDto
            {
                Id = i.Id,
                Titulo = i.Titulo,
                RolEnInvestigacion = i.RolEnInvestigacion,
                MesesDeInvestigacion = i.MesesDeInvestigacion,
                NombreDocente = i.Docente != null 
                    ? $"{i.Docente.Nombre1} {i.Docente.Apellido1}" 
                    : "Docente no encontrado"
            });
        }

        private static InvestigacionDto MapToDto(Investigacion investigacion)
        {
            return new InvestigacionDto
            {
                Id = investigacion.Id,
                Titulo = investigacion.Titulo,
                FechaInicio = investigacion.FechaInicio,
                FechaFinalizacion = investigacion.FechaFinalizacion,
                RolEnInvestigacion = investigacion.RolEnInvestigacion,
                MesesDeInvestigacion = investigacion.MesesDeInvestigacion,
                NombreDocente = investigacion.Docente != null 
                    ? $"{investigacion.Docente.Nombre1} {investigacion.Docente.Apellido1}" 
                    : "Docente no encontrado",
                DocenteCedula = investigacion.DocenteCedula,
                InformeRuta = investigacion.InformeRuta,
                ContenidoHash = investigacion.ContenidoHash
            };
        }
    }
} 