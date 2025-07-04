using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIGAD.Application.Interfaces;
using System.Security.Cryptography;

namespace SIGAD.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<FileStorageService> _logger;
        private readonly string _localStoragePath;
        private readonly long _maxFileSizeBytes;

        public FileStorageService(
            ICloudinaryService cloudinaryService,
            IConfiguration configuration,
            ILogger<FileStorageService> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            
            _localStoragePath = configuration["FileStorage:LocalStoragePath"] ?? "uploads";
            _maxFileSizeBytes = long.Parse(configuration["FileStorage:MaxFileSizeBytes"] ?? "10485760"); // 10MB
        }

        public async Task<(string rutaLocal, string urlCloudinary)> GuardarArchivoDualAsync(
            byte[] contenido, 
            string tipoDocumento, 
            string identificador,
            string extension = ".pdf")
        {
            try
            {
                _logger.LogInformation("🔄 Iniciando guardado dual para {TipoDocumento}/{Identificador}", tipoDocumento, identificador);

                // 1. Usar contenido sin compresión por ahora
                var contenidoFinal = contenido;

                // 2. Generar nombres únicos
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var nombreArchivo = $"{identificador}_{timestamp}{extension}";

                // 3. Guardar localmente
                var rutaLocal = await GuardarLocalAsync(contenidoFinal, tipoDocumento, nombreArchivo);

                // 4. Subir a Cloudinary (en paralelo para mayor velocidad)
                _logger.LogInformation("🔄 Llamando a CloudinaryService.SubirArchivoAsync con TipoDocumento: {TipoDocumento}, Identificador: {Identificador}", 
                    tipoDocumento, $"{identificador}_{timestamp}");
                
                var urlCloudinary = await _cloudinaryService.SubirArchivoAsync(
                    contenidoFinal, tipoDocumento, $"{identificador}_{timestamp}", extension);

                _logger.LogInformation("✅ Guardado dual completado - Local: {RutaLocal}, Cloudinary: {HasUrl}, CloudinaryUrl: {CloudinaryUrl}", 
                    rutaLocal, !string.IsNullOrEmpty(urlCloudinary), urlCloudinary);

                return (rutaLocal, urlCloudinary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en guardado dual para {TipoDocumento}/{Identificador}", tipoDocumento, identificador);
                throw;
            }
        }

        public async Task<(string rutaLocal, string urlCloudinary)> GuardarArchivoDualAsync(
            IFormFile archivo, 
            string tipoDocumento, 
            string identificador)
        {
            if (!ValidarArchivo(archivo))
            {
                throw new ArgumentException("Archivo no válido");
            }

            // Convertir IFormFile a bytes
            using var memoryStream = new MemoryStream();
            await archivo.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            return await GuardarArchivoDualAsync(contenido, tipoDocumento, identificador, extension);
        }

        public async Task<bool> EliminarArchivoDualAsync(string? rutaLocal, string? urlCloudinary)
        {
            var resultados = new List<bool>();

            // Eliminar archivo local
            if (!string.IsNullOrEmpty(rutaLocal))
            {
                try
                {
                    var rutaCompleta = Path.Combine(_localStoragePath, rutaLocal);
                    if (File.Exists(rutaCompleta))
                    {
                        File.Delete(rutaCompleta);
                        _logger.LogInformation("✅ Archivo local eliminado: {RutaLocal}", rutaLocal);
                        resultados.Add(true);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Archivo local no encontrado: {RutaLocal}", rutaLocal);
                        resultados.Add(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al eliminar archivo local: {RutaLocal}", rutaLocal);
                    resultados.Add(false);
                }
            }

            // Eliminar de Cloudinary
            if (!string.IsNullOrEmpty(urlCloudinary))
            {
                try
                {
                    var eliminadoCloudinary = await _cloudinaryService.EliminarArchivoAsync(urlCloudinary);
                    resultados.Add(eliminadoCloudinary);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al eliminar archivo de Cloudinary: {UrlCloudinary}", urlCloudinary);
                    resultados.Add(false);
                }
            }

            // Considerar exitoso si al menos uno fue eliminado
            return resultados.Any() && resultados.Any(r => r);
        }

        public string ObtenerMejorUrl(string? rutaLocal, string? urlCloudinary, string baseUrl = "https://localhost:7072")
        {
            // Priorizar Cloudinary (CDN rápido)
            if (!string.IsNullOrEmpty(urlCloudinary))
            {
                return urlCloudinary;
            }

            // Fallback a archivo local
            if (!string.IsNullOrEmpty(rutaLocal))
            {
                var relativePath = rutaLocal.StartsWith("/uploads", StringComparison.OrdinalIgnoreCase)
                    ? rutaLocal
                    : $"/uploads/{rutaLocal.TrimStart('/')}";
                
                return $"{baseUrl.TrimEnd('/')}{relativePath}";
            }

            return string.Empty;
        }

        public bool ValidarArchivo(IFormFile archivo, long maxSizeBytes = 0, string[]? allowedExtensions = null)
        {
            if (archivo == null || archivo.Length == 0)
            {
                _logger.LogWarning("⚠️ Archivo nulo o vacío");
                return false;
            }

            // Validar tamaño
            var maxSize = maxSizeBytes > 0 ? maxSizeBytes : _maxFileSizeBytes;
            if (archivo.Length > maxSize)
            {
                _logger.LogWarning("⚠️ Archivo excede el tamaño máximo: {Size} > {MaxSize}", archivo.Length, maxSize);
                return false;
            }

            // Validar extensión
            var extensionesPermitidas = allowedExtensions ?? new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            if (!extensionesPermitidas.Contains(extension))
            {
                _logger.LogWarning("⚠️ Extensión no permitida: {Extension}. Permitidas: {AllowedExtensions}", 
                    extension, string.Join(", ", extensionesPermitidas));
                return false;
            }

            return true;
        }

        public async Task<(string rutaLocal, string urlCloudinary, string contentHash)> UploadFileAsync(
            IFormFile archivo,
            string tipoDocumento,
            string[]? allowedExtensions = null,
            long maxSizeBytes = 10485760)
        {
            // Validar archivo
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo es obligatorio");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            
            // Validar extensión
            if (allowedExtensions != null && !allowedExtensions.Contains(extension))
                throw new ArgumentException($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", allowedExtensions)}");

            // Validar tamaño
            if (archivo.Length > maxSizeBytes)
                throw new ArgumentException($"El archivo no puede exceder los {maxSizeBytes / 1024 / 1024}MB");

            // Generar identificador único más organizado
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var shortGuid = Guid.NewGuid().ToString("N")[..8];
            var identificador = $"{tipoDocumento}_{timestamp}_{shortGuid}";

            // Leer contenido y generar hash
            using var stream = archivo.OpenReadStream();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var contenido = memoryStream.ToArray();

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(contenido);
                var contentHash = Convert.ToHexString(hashBytes);

                // Guardar usando el método existente
                var (rutaLocal, urlCloudinary) = await GuardarArchivoDualAsync(contenido, tipoDocumento, identificador, extension);
                
                return (rutaLocal, urlCloudinary, contentHash);
            }
        }

        #region Métodos Privados

        private async Task<string> GuardarLocalAsync(byte[] contenido, string tipoDocumento, string nombreArchivo)
        {
            // Crear directorio si no existe
            var directorioCompleto = Path.Combine(_localStoragePath, tipoDocumento);
            Directory.CreateDirectory(directorioCompleto);

            // Guardar archivo
            var rutaCompleta = Path.Combine(directorioCompleto, nombreArchivo);
            await File.WriteAllBytesAsync(rutaCompleta, contenido);

            // Retornar ruta relativa
            var rutaRelativa = $"{tipoDocumento}/{nombreArchivo}";
            
            _logger.LogInformation("✅ Archivo guardado localmente: {RutaRelativa} ({Size} bytes)", 
                rutaRelativa, contenido.Length);

            return rutaRelativa;
        }

        #endregion
    }
}
