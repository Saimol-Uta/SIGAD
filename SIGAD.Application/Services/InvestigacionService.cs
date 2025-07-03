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
        private readonly IFileStorageService _fileStorageService;

        public InvestigacionService(
            IInvestigacionRepository investigacionRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IFileStorageService fileStorageService)
        {
            _investigacionRepository = investigacionRepository;
            _solicitudRepository = solicitudRepository;
            _fileStorageService = fileStorageService;
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

            // Subir archivo usando FileStorageService
            var (localPath, cloudinaryUrl, contentHash) = await _fileStorageService.UploadFileAsync(
                informe, 
                "investigaciones", 
                new[] { ".pdf", ".doc", ".docx" },
                10 * 1024 * 1024 // 10MB
            );

            // Verificar que la solicitud existe (solo si se proporciona una solicitudId)
            if (crearInvestigacionDto.SolicitudId.HasValue)
            {
                if (!await _solicitudRepository.ExistsAsync(crearInvestigacionDto.SolicitudId.Value))
                    throw new ArgumentException("La solicitud especificada no existe");
            }

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
                InformeRuta = localPath,
                UrlCloudinary = cloudinaryUrl,
                ContenidoHash = contentHash
            };

            await _investigacionRepository.AddAsync(investigacion);

            // Asociar a la solicitud solo si se proporcionó un SolicitudId
            if (crearInvestigacionDto.SolicitudId.HasValue)
            {
                await _investigacionRepository.AddToSolicitudAsync(crearInvestigacionDto.SolicitudId.Value, investigacion.Id);
            }

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
                // Eliminar archivos anteriores
                await _fileStorageService.EliminarArchivoDualAsync(investigacion.InformeRuta, investigacion.UrlCloudinary);

                // Subir nuevo archivo
                var (localPath, cloudinaryUrl, hash) = await _fileStorageService.UploadFileAsync(
                    archivo, 
                    "investigaciones", 
                    new[] { ".pdf", ".doc", ".docx" },
                    10 * 1024 * 1024 // 10MB
                );
                
                investigacion.InformeRuta = localPath;
                investigacion.UrlCloudinary = cloudinaryUrl;
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

            // Eliminar archivos duales
            await _fileStorageService.EliminarArchivoDualAsync(investigacion.InformeRuta, investigacion.UrlCloudinary);

            await _investigacionRepository.DeleteAsync(id);
            return true;
        }

        public async Task<(byte[] FileContent, string ContentType, string FileName)> DownloadInformeAsync(int id)
        {
            var investigacion = await _investigacionRepository.GetByIdAsync(id);
            if (investigacion == null || string.IsNullOrEmpty(investigacion.InformeRuta))
                throw new FileNotFoundException("Informe no encontrado");

            // Obtener la mejor URL disponible y descargar
            var mejorUrl = _fileStorageService.ObtenerMejorUrl(investigacion.InformeRuta, investigacion.UrlCloudinary);
            
            byte[] fileContent;
            
            // Si es una URL de Cloudinary, descargar desde allí
            if (!string.IsNullOrEmpty(investigacion.UrlCloudinary) && mejorUrl == investigacion.UrlCloudinary)
            {
                using var httpClient = new HttpClient();
                fileContent = await httpClient.GetByteArrayAsync(mejorUrl);
            }
            else if (File.Exists(investigacion.InformeRuta))
            {
                fileContent = await File.ReadAllBytesAsync(investigacion.InformeRuta);
            }
            else
            {
                throw new FileNotFoundException("Informe no encontrado");
            }

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
                DocenteCedula = investigacion.DocenteCedula,
                InformeRuta = investigacion.InformeRuta,
                UrlCloudinary = investigacion.UrlCloudinary,
                ContenidoHash = investigacion.ContenidoHash,
                
                // Mapeo de solicitudes asociadas
                SolicitudId = investigacion.InvestigacionesPorSolicitud?.FirstOrDefault()?.SolicitudId.ToString(),
                Solicitudes = investigacion.InvestigacionesPorSolicitud?.Select(ips => new SolicitudBasicaDto
                {
                    SolicitudId = ips.SolicitudId.ToString(),
                    Estado = ips.SolicitudAscenso?.Estado.ToString() ?? "Desconocido"
                }).ToList()
                EsInternacional = investigacion.EsInternacional // Exponer en el DTO
            };
        }

        public async Task<bool> AsociarInvestigacionASolicitudAsync(AsociarInvestigacionSolicitudDto asociarDto)
        {
            var investigacionExists = await _investigacionRepository.ExistsAsync(asociarDto.InvestigacionId);
            if (!investigacionExists)
            {
                return false;
            }

            var solicitudExists = await _solicitudRepository.ExistsAsync(asociarDto.SolicitudId);
            if (!solicitudExists)
            {
                return false;
            }

            await _investigacionRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.InvestigacionId);
            return true;
        }

        public async Task DesasociarInvestigacionDeSolicitudAsync(Guid solicitudId, int investigacionId)
        {
            await _investigacionRepository.RemoveFromSolicitudAsync(solicitudId, investigacionId);
        }
    }
}