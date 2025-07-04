using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIGAD.Application.Interfaces;
using System.Text.RegularExpressions;

namespace SIGAD.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary? _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;
        private readonly bool _isEnabled;

        public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            
            try
            {
                var cloudName = configuration["Cloudinary:CloudName"];
                var apiKey = configuration["Cloudinary:ApiKey"];
                var apiSecret = configuration["Cloudinary:ApiSecret"];
                _isEnabled = bool.Parse(configuration["Cloudinary:UseCloudinary"] ?? "false");

                if (_isEnabled && !string.IsNullOrEmpty(cloudName) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(apiSecret))
                {
                    var account = new Account(cloudName, apiKey, apiSecret);
                    _cloudinary = new Cloudinary(account);
                    
                    _logger.LogInformation("✅ CloudinaryService inicializado correctamente para cloud: {CloudName}", cloudName);
                }
                else
                {
                    _logger.LogWarning("⚠️ CloudinaryService deshabilitado o configuración incompleta");
                    _isEnabled = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al inicializar CloudinaryService");
                _isEnabled = false;
            }
        }

        public async Task<string> SubirArchivoAsync(byte[] contenido, string tipoDocumento, string identificador, string extension)
        {
            if (!_isEnabled || _cloudinary == null)
            {
                _logger.LogWarning("CloudinaryService no está habilitado");
                return string.Empty;
            }

            try
            {
                using var stream = new MemoryStream(contenido);
                
                // Generar public_id organizado por carpetas con estructura forzada
                var publicId = $"sigad/{tipoDocumento}/{identificador}";
                
                _logger.LogInformation("🔄 Preparando subida a Cloudinary - TipoDocumento: {TipoDocumento}, Identificador: {Identificador}, PublicId: {PublicId}", 
                    tipoDocumento, identificador, publicId);
                
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription($"{identificador}{extension}", stream),
                    PublicId = publicId,
                    UseFilename = false,
                    UniqueFilename = false,
                    Overwrite = true,
                    AccessMode = "public", // Asegurar acceso público
                    Folder = $"sigad/{tipoDocumento}" // Forzar carpeta específica
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("✅ Archivo subido a Cloudinary: {PublicId} -> {Url}", publicId, uploadResult.SecureUrl);
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    _logger.LogError("❌ Error al subir archivo a Cloudinary: {Error}, StatusCode: {StatusCode}", uploadResult.Error?.Message, uploadResult.StatusCode);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Excepción al subir archivo a Cloudinary");
                return string.Empty;
            }
        }

        public async Task<bool> EliminarArchivoAsync(string urlCloudinary)
        {
            if (!_isEnabled || _cloudinary == null || string.IsNullOrEmpty(urlCloudinary))
            {
                return false;
            }

            try
            {
                var publicId = ExtraerPublicId(urlCloudinary);
                if (string.IsNullOrEmpty(publicId))
                {
                    _logger.LogWarning("No se pudo extraer public_id de la URL: {Url}", urlCloudinary);
                    return false;
                }

                var deleteParams = new DeletionParams(publicId)
                {
                    ResourceType = ResourceType.Raw
                };

                var result = await _cloudinary.DestroyAsync(deleteParams);

                if (result.Result == "ok")
                {
                    _logger.LogInformation("✅ Archivo eliminado de Cloudinary: {PublicId}", publicId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("⚠️ No se pudo eliminar archivo de Cloudinary: {PublicId} - {Result}", publicId, result.Result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar archivo de Cloudinary: {Url}", urlCloudinary);
                return false;
            }
        }

        public string ExtraerPublicId(string urlCloudinary)
        {
            if (string.IsNullOrEmpty(urlCloudinary))
                return string.Empty;

            try
            {
                // Patrón para extraer public_id de URLs de Cloudinary
                // Ejemplo: https://res.cloudinary.com/tu-cloud/raw/upload/v1234567890/sigad/cursos/archivo.pdf
                var pattern = @"https://res\.cloudinary\.com/[^/]+/raw/upload/(?:v\d+/)?(.+)";
                var match = Regex.Match(urlCloudinary, pattern);
                
                if (match.Success)
                {
                    var publicId = match.Groups[1].Value;
                    // Remover extensión si existe
                    var lastDotIndex = publicId.LastIndexOf('.');
                    if (lastDotIndex > 0)
                    {
                        publicId = publicId.Substring(0, lastDotIndex);
                    }
                    return publicId;
                }
                
                _logger.LogWarning("No se pudo extraer public_id de URL: {Url}", urlCloudinary);
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al extraer public_id de URL: {Url}", urlCloudinary);
                return string.Empty;
            }
        }

        public async Task<bool> VerificarDisponibilidadAsync()
        {
            if (!_isEnabled || _cloudinary == null)
            {
                return false;
            }

            try
            {
                // Hacer una consulta simple para verificar conectividad
                var pingResult = await _cloudinary.GetResourceAsync("non-existent-resource");
                // Si llegamos aquí sin excepción, el servicio está disponible
                return true;
            }
            catch (Exception ex) when (ex.Message.Contains("not found") || ex.Message.Contains("Not Found"))
            {
                // "Resource not found" es esperado y significa que la conexión funciona
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Cloudinary no está disponible");
                return false;
            }
        }
    }
}
