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
            // Validar archivo obligatorio
            if (informe == null || informe.Length == 0)
                throw new ArgumentException("El informe es obligatorio");

            // Guardar archivo y obtener ruta relativa y hash
            var (rutaRelativa, contentHash) = await GuardarArchivoAsync(informe);

            // Verificar que la solicitud existe
            if (!await _solicitudRepository.ExistsAsync(crearInvestigacionDto.SolicitudId))
                throw new ArgumentException("La solicitud especificada no existe");

            // Validar fechas
            if (crearInvestigacionDto.FechaFinalizacion <= crearInvestigacionDto.FechaInicio)
                throw new ArgumentException("La fecha de finalización debe ser posterior a la fecha de inicio");

            // Crear entidad
            var investigacion = new Investigacion
            {
                Titulo = crearInvestigacionDto.Titulo,
                FechaInicio = crearInvestigacionDto.FechaInicio,
                FechaFinalizacion = crearInvestigacionDto.FechaFinalizacion,
                RolEnInvestigacion = crearInvestigacionDto.RolEnInvestigacion,
                MesesDeInvestigacion = crearInvestigacionDto.MesesDeInvestigacion,
                DocenteCedula = crearInvestigacionDto.DocenteCedula,
                InformeRuta = rutaRelativa,
                ContenidoHash = contentHash
            };

            await _investigacionRepository.AddAsync(investigacion);

            // Asociar automáticamente a la solicitud
            await _investigacionRepository.AddToSolicitudAsync(crearInvestigacionDto.SolicitudId, investigacion.Id);

            // Obtener investigación completa con relaciones
            var investigacionCreada = await _investigacionRepository.GetByIdAsync(investigacion.Id);
            return MapToDto(investigacionCreada!);
        }

        public async Task<InvestigacionDto?> UpdateAsync(int id, ActualizarInvestigacionDto actualizarInvestigacionDto, IFormFile? archivo)
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
            investigacion.DocenteCedula = actualizarInvestigacionDto.DocenteCedula;

            // Procesar nuevo archivo si se proporciona
            if (archivo != null && archivo.Length > 0)
            {
                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(investigacion.InformeRuta))
                {
                    var rutaFisica = Path.Combine(_fileStoragePath, Path.GetFileName(investigacion.InformeRuta));
                    if (File.Exists(rutaFisica))
                    {
                        File.Delete(rutaFisica);
                    }
                }

                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                investigacion.InformeRuta = ruta;
                investigacion.ContenidoHash = hash;
            }

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
            if (!string.IsNullOrEmpty(investigacion.InformeRuta))
            {
                var rutaFisica = Path.Combine(_fileStoragePath, Path.GetFileName(investigacion.InformeRuta));
                if (File.Exists(rutaFisica))
                {
                    File.Delete(rutaFisica);
                }
            }

            await _investigacionRepository.DeleteAsync(id);
            return true;
        }

        public async Task<(byte[] FileContent, string ContentType, string FileName)> DownloadInformeAsync(int id)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            if (investigacion == null || string.IsNullOrEmpty(investigacion.InformeRuta))
                throw new FileNotFoundException("Informe no encontrado");

            var rutaFisica = Path.Combine(_fileStoragePath, Path.GetFileName(investigacion.InformeRuta));
            if (!File.Exists(rutaFisica))
                throw new FileNotFoundException("Informe no encontrado");

            var fileContent = await File.ReadAllBytesAsync(rutaFisica);
            var extension = Path.GetExtension(rutaFisica).ToLowerInvariant();

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
            var filePath = Path.Combine(_fileStoragePath, fileName);

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
            var relativePath = Path.Combine("investigaciones", fileName).Replace("\\", "/");

            return (relativePath, contentHash);
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