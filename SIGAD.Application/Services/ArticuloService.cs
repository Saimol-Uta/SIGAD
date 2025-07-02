using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
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
        private readonly string _uploadsPath;

        public ArticuloService(
            IArticuloRepository articuloRepository,
            IDocenteRepository docenteRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _articuloRepository = articuloRepository;
            _docenteRepository = docenteRepository;
            _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
            _uploadsPath = configuration["FileStorage:ArticulosPath"] ?? "uploads/articulos";
            
            // Crear directorio si no existe
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
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
            string contenidoHash = string.Empty;

            if (archivo != null && archivo.Length > 0)
            {
                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                archivoRuta = ruta;
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
                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(articulo.ArchivoRuta))
                {
                    var rutaFisicaAnterior = Path.Combine(_uploadsPath, Path.GetFileName(articulo.ArchivoRuta));
                    if (File.Exists(rutaFisicaAnterior))
                    {
                        File.Delete(rutaFisicaAnterior);
                    }
                }

                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                articulo.ArchivoRuta = ruta;
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

            // Eliminar archivo si existe
            if (!string.IsNullOrEmpty(articulo.ArchivoRuta))
            {
                var rutaFisica = Path.Combine(_uploadsPath, Path.GetFileName(articulo.ArchivoRuta));
                if (File.Exists(rutaFisica))
                {
                    File.Delete(rutaFisica);
                }
            }

            await _articuloRepository.DeleteAsync(decodedDoi);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AsociarArticuloASolicitudAsync(AsociarArticuloSolicitudDto asociarDto)
        {
            var articuloExists = await _articuloRepository.ExistsAsync(asociarDto.ArticuloDOI);
            if (!articuloExists)
            {
                return false;
            }

            var solicitud = await _solicitudRepository.GetByIdAsync(asociarDto.SolicitudId);
            if (solicitud == null)
            {
                return false;
            }

            await _articuloRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.ArticuloDOI);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesasociarArticuloDeSolicitudAsync(Guid solicitudId, string articuloDoi)
        {
            await _articuloRepository.RemoveFromSolicitudAsync(solicitudId, articuloDoi);
            await _unitOfWork.SaveChangesAsync();
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

            // Construir la ruta física completa desde la ruta relativa
            var rutaFisica = Path.Combine(_uploadsPath, Path.GetFileName(articulo.ArchivoRuta));
            if (!File.Exists(rutaFisica))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(rutaFisica);
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

        private async Task<(string rutaRelativa, string hash)> GuardarArchivoAsync(IFormFile archivo)
        {
            // Validaciones
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Tipo de archivo no permitido. Use: PDF, DOC, DOCX");

            if (archivo.Length > 25 * 1024 * 1024) // 25MB
                throw new ArgumentException("El archivo no puede exceder los 25MB");

            // Generar nombre único para el archivo
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(_uploadsPath, nombreArchivo);

            // Guardar archivo físicamente
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Calcular hash
            string contentHash;
            using (var stream = File.OpenRead(rutaCompleta))
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(stream);
                contentHash = Convert.ToHexString(hashBytes);
            }

            // Ruta relativa para la base de datos
            var relativePath = Path.Combine("articulos", nombreArchivo).Replace("\\", "/");

            return (relativePath, contentHash);
        }

        private async Task<string> CalcularHashArchivoAsync(string rutaArchivo)
        {
            using var sha256 = SHA256.Create();
            await using var stream = File.OpenRead(rutaArchivo);
            var hash = await sha256.ComputeHashAsync(stream);
            return Convert.ToHexString(hash);
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
                DocenteCedula = articulo.DocenteCedula,
                DocenteNombreCompleto = articulo.Docente != null 
                    ? $"{articulo.Docente.Nombre1} {articulo.Docente.Nombre2 ?? ""} {articulo.Docente.Apellido1} {articulo.Docente.Apellido2}".Trim()
                    : string.Empty,
                UnidadVerificadora = articulo.UnidadVerificadora,
                Verificado = articulo.Verificado,
                FechaVerificacion = articulo.FechaVerificacion
            };
        }
    }
} 