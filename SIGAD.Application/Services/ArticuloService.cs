using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace SIGAD.Application.Services
{
    public class ArticuloService : IArticuloService
    {
        private readonly IArticuloRepository _articuloRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ArticuloService(
            IArticuloRepository articuloRepository,
            IDocenteRepository docenteRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService)
        {
            _articuloRepository = articuloRepository;
            _docenteRepository = docenteRepository; _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<ArticuloDto>> GetAllArticulosAsync()
        {
            var articulos = await _articuloRepository.GetAllAsync();
            return articulos.Select(MapToDto);
        }

        public async Task<ArticuloDto?> GetArticuloByIdAsync(string doi)
        {
            // Decodificar el DOI en caso de que venga codificado desde la URL
            var decodedDoi = Uri.UnescapeDataString(doi);
            var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);
            return articulo != null ? MapToDto(articulo) : null;
        }

        public async Task<IEnumerable<ArticuloDto>> GetArticulosByDocenteAsync(string docenteCedula)
        {
            var articulos = await _articuloRepository.GetByDocenteCedulaAsync(docenteCedula);
            return articulos.Select(MapToDto);
        }

        public async Task<IEnumerable<ArticuloDto>> GetArticulosBySolicitudAsync(Guid solicitudId)
        {
            var articulos = await _articuloRepository.GetBySolicitudIdAsync(solicitudId);
            return articulos.Select(MapToDto);
        }

        public async Task<ArticuloDto> CreateArticuloAsync(CrearArticuloDto createDto, IFormFile? archivo)
        {
            // Validar que el artículo no exista ya
            var articuloExistente = await _articuloRepository.ExistsAsync(createDto.DOI);
            if (articuloExistente)
            {
                throw new ArgumentException("Ya existe un artículo con este DOI");
            }

            // Validar que el docente existe
            var docenteExists = await _docenteRepository.ExistsByCedulaAsync(createDto.DocenteCedula);
            if (!docenteExists)
            {
                throw new ArgumentException("El docente especificado no existe");
            }

            // Procesar archivo si se proporciona
            string archivoRuta = string.Empty;
            string urlCloudinary = string.Empty;
            string contenidoHash = string.Empty;

            if (archivo != null && archivo.Length > 0)
            {
                var (localPath, cloudinaryUrl, hash) = await _fileStorageService.UploadFileAsync(
                    archivo,
                    "articulos",
                    new[] { ".pdf" },
                    10 * 1024 * 1024 // 10MB
                );
                archivoRuta = localPath;
                urlCloudinary = cloudinaryUrl;
                contenidoHash = hash;
            }

            var articulo = new Articulo
            {
                DOI = createDto.DOI,
                Titulo = createDto.Titulo,
                Revista = createDto.Revista,
                AnioPublicacion = createDto.AnioPublicacion,
                IdiomaPublicacion = createDto.IdiomaPublicacion,
                DocenteCedula = createDto.DocenteCedula,
                ArchivoRuta = archivoRuta,
                UrlCloudinary = urlCloudinary,
                ContenidoHash = contenidoHash
            };

            await _articuloRepository.AddAsync(articulo);

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync();

            // Si se especifica una solicitud, verificar que existe y asociar el artículo
            if (createDto.SolicitudId.HasValue)
            {
                var solicitud = await _solicitudRepository.GetByIdAsync(createDto.SolicitudId.Value);
                var solicitudExists = solicitud != null;
                if (solicitudExists)
                {
                    await _articuloRepository.AddToSolicitudAsync(createDto.SolicitudId.Value, articulo.DOI);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException($"La solicitud con ID {createDto.SolicitudId.Value} no existe");
                }
            }

            // Obtener el artículo creado con datos del docente
            var articuloCreado = await _articuloRepository.GetByIdAsync(articulo.DOI);
            return MapToDto(articuloCreado!);
        }

        public async Task<ArticuloDto> UpdateArticuloAsync(string doi, ActualizarArticuloDto updateDto, IFormFile? archivo)
        {
            // Decodificar el DOI en caso de que venga codificado desde la URL
            var decodedDoi = Uri.UnescapeDataString(doi);
            var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);
            if (articulo == null)
            {
                throw new ArgumentException("El artículo no existe");
            }

            // Actualizar propiedades
            articulo.Titulo = updateDto.Titulo;
            articulo.Revista = updateDto.Revista;
            articulo.AnioPublicacion = updateDto.AnioPublicacion;
            articulo.IdiomaPublicacion = updateDto.IdiomaPublicacion;

            // Procesar nuevo archivo si se proporciona
            if (archivo != null && archivo.Length > 0)
            {
                // Eliminar archivos anteriores
                await _fileStorageService.EliminarArchivoDualAsync(articulo.ArchivoRuta, articulo.UrlCloudinary);

                // Subir nuevo archivo
                var (localPath, cloudinaryUrl, hash) = await _fileStorageService.UploadFileAsync(
                    archivo,
                    "articulos",
                    new[] { ".pdf" },
                    10 * 1024 * 1024 // 10MB
                );

                articulo.ArchivoRuta = localPath;
                articulo.UrlCloudinary = cloudinaryUrl;
                articulo.ContenidoHash = hash;
            }

            await _articuloRepository.UpdateAsync(articulo);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(articulo);
        }

        public async Task<bool> DeleteArticuloAsync(string doi)
        {
            // Decodificar el DOI en caso de que venga codificado desde la URL
            var decodedDoi = Uri.UnescapeDataString(doi);
            var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);
            if (articulo == null)
            {
                return false;
            }

            // Eliminar archivos duales
            await _fileStorageService.EliminarArchivoDualAsync(articulo.ArchivoRuta, articulo.UrlCloudinary);

            await _articuloRepository.DeleteAsync(decodedDoi);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AsociarArticuloASolicitudAsync(AsociarArticuloSolicitudDto asociarDto)
        {
            // Log temporal
            Console.WriteLine($"[SERVICE] Asociando artículo - DOI: '{asociarDto.ArticuloDOI}', SolicitudId: {asociarDto.SolicitudId}");

            var articuloExists = await _articuloRepository.ExistsAsync(asociarDto.ArticuloDOI);
            Console.WriteLine($"[SERVICE] Artículo existe: {articuloExists}");

            if (!articuloExists)
            {
                return false;
            }

            var solicitud = await _solicitudRepository.GetByIdAsync(asociarDto.SolicitudId);
            Console.WriteLine($"[SERVICE] Solicitud existe: {solicitud != null}");

            if (solicitud == null)
            {
                return false;
            }

            await _articuloRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.ArticuloDOI);
            await _unitOfWork.SaveChangesAsync();
            Console.WriteLine($"[SERVICE] Asociación completada");
            return true;
        }

        public async Task<bool> DesasociarArticuloDeSolicitudAsync(Guid solicitudId, string articuloDoi)
        {
            await _articuloRepository.RemoveFromSolicitudAsync(solicitudId, articuloDoi);
            return true;
        }

        public async Task<byte[]?> GetArchivoArticuloAsync(string doi)
        {
            // Decodificar el DOI en caso de que venga codificado desde la URL
            var decodedDoi = Uri.UnescapeDataString(doi);
            var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);
            if (articulo == null || string.IsNullOrEmpty(articulo.ArchivoRuta))
            {
                return null;
            }

            // Obtener la mejor URL disponible y descargar
            var mejorUrl = _fileStorageService.ObtenerMejorUrl(articulo.ArchivoRuta, articulo.UrlCloudinary);

            // Si es una URL de Cloudinary, descargar desde allí
            if (!string.IsNullOrEmpty(articulo.UrlCloudinary) && mejorUrl == articulo.UrlCloudinary)
            {
                using var httpClient = new HttpClient();
                return await httpClient.GetByteArrayAsync(mejorUrl);
            }

            // Si no, intentar desde archivo local
            if (File.Exists(articulo.ArchivoRuta))
            {
                return await File.ReadAllBytesAsync(articulo.ArchivoRuta);
            }

            return null;
        }

        public async Task<string?> GetNombreArchivoAsync(string doi)
        {
            // Decodificar el DOI en caso de que venga codificado desde la URL
            var decodedDoi = Uri.UnescapeDataString(doi);
            var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);
            if (articulo == null || string.IsNullOrEmpty(articulo.ArchivoRuta))
            {
                return null;
            }

            return Path.GetFileName(articulo.ArchivoRuta);
        }

        public async Task<bool> VerificarUsoPrevioAsync(string doi, string? solicitudActualId = null)
        {
            try
            {
                var decodedDoi = Uri.UnescapeDataString(doi);
                var articulo = await _articuloRepository.GetByIdAsync(decodedDoi);

                if (articulo == null || articulo.ArticulosPorSolicitud == null)
                {
                    return false;
                }

                // Si no hay solicitud actual, verificar si tiene cualquier asociación
                if (string.IsNullOrEmpty(solicitudActualId))
                {
                    return articulo.ArticulosPorSolicitud.Any();
                }

                // Si hay solicitud actual, verificar si hay asociaciones diferentes a la actual
                if (Guid.TryParse(solicitudActualId, out var solicitudActualGuid))
                {
                    return articulo.ArticulosPorSolicitud.Any(ap => ap.SolicitudId != solicitudActualGuid);
                }

                return articulo.ArticulosPorSolicitud.Any();
            }
            catch (Exception)
            {
                // En caso de error, no bloquear la funcionalidad
                return false;
            }
        }

        private static ArticuloDto MapToDto(Articulo articulo)
        {
            return new ArticuloDto
            {
                DOI = articulo.DOI,
                Titulo = articulo.Titulo,
                Revista = articulo.Revista,
                AnioPublicacion = articulo.AnioPublicacion,
                IdiomaPublicacion = articulo.IdiomaPublicacion ?? string.Empty,
                ArchivoRuta = articulo.ArchivoRuta,
                UrlCloudinary = articulo.UrlCloudinary,
                ContenidoHash = articulo.ContenidoHash,
                DocenteCedula = articulo.DocenteCedula,
                DocenteNombreCompleto = articulo.Docente != null
                    ? $"{articulo.Docente.Nombre1} {articulo.Docente.Nombre2 ?? ""} {articulo.Docente.Apellido1} {articulo.Docente.Apellido2}".Trim()
                    : string.Empty,
                UnidadVerificadora = articulo.UnidadVerificadora,
                Verificado = articulo.Verificado,
                FechaVerificacion = articulo.FechaVerificacion,

                // Mapeo de solicitudes asociadas
                SolicitudId = articulo.ArticulosPorSolicitud?.FirstOrDefault()?.SolicitudId.ToString(),
                Solicitudes = articulo.ArticulosPorSolicitud?.Select(es => new SolicitudBasicaDto
                {
                    SolicitudId = es.SolicitudId.ToString(),
                    Estado = es.SolicitudAscenso?.Estado.ToString() ?? "Desconocido"
                }).ToList()
            };
        }
    }
}