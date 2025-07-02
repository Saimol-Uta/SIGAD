using Microsoft.AspNetCore.Http;

namespace SIGAD.Application.Interfaces
{
    /// <summary>
    /// Servicio unificado para gestión de archivos con respaldo dual (Local + Cloudinary)
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Sube un archivo validándolo y guardándolo en ambos almacenamientos
        /// </summary>
        /// <param name="archivo">Archivo a subir</param>
        /// <param name="tipoDocumento">Tipo de documento (cursos, articulos, etc.)</param>
        /// <param name="allowedExtensions">Extensiones permitidas</param>
        /// <param name="maxSizeBytes">Tamaño máximo permitido</param>
        /// <returns>Tupla con (rutaLocal, urlCloudinary, hash)</returns>
        Task<(string rutaLocal, string urlCloudinary, string contentHash)> UploadFileAsync(
            IFormFile archivo,
            string tipoDocumento,
            string[]? allowedExtensions = null,
            long maxSizeBytes = 10485760);

        /// <summary>
        /// Guarda un archivo tanto localmente como en Cloudinary desde bytes
        /// </summary>
        /// <param name="contenido">Contenido del archivo en bytes</param>
        /// <param name="tipoDocumento">Tipo de documento (cursos, articulos, etc.)</param>
        /// <param name="identificador">Identificador único para el archivo</param>
        /// <param name="extension">Extensión del archivo (ej: .pdf)</param>
        /// <returns>Tupla con (rutaLocal, urlCloudinary)</returns>
        Task<(string rutaLocal, string urlCloudinary)> GuardarArchivoDualAsync(
            byte[] contenido, 
            string tipoDocumento, 
            string identificador,
            string extension = ".pdf");
        
        /// <summary>
        /// Guarda un archivo tanto localmente como en Cloudinary desde IFormFile
        /// </summary>
        /// <param name="archivo">Archivo desde formulario</param>
        /// <param name="tipoDocumento">Tipo de documento (cursos, articulos, etc.)</param>
        /// <param name="identificador">Identificador único para el archivo</param>
        /// <returns>Tupla con (rutaLocal, urlCloudinary)</returns>
        Task<(string rutaLocal, string urlCloudinary)> GuardarArchivoDualAsync(
            IFormFile archivo, 
            string tipoDocumento, 
            string identificador);
        
        /// <summary>
        /// Elimina un archivo tanto del almacenamiento local como de Cloudinary
        /// </summary>
        /// <param name="rutaLocal">Ruta local del archivo</param>
        /// <param name="urlCloudinary">URL de Cloudinary</param>
        /// <returns>True si la eliminación fue exitosa</returns>
        Task<bool> EliminarArchivoDualAsync(string? rutaLocal, string? urlCloudinary);
        
        /// <summary>
        /// Obtiene la mejor URL disponible para un archivo (prioriza Cloudinary)
        /// </summary>
        /// <param name="rutaLocal">Ruta local del archivo</param>
        /// <param name="urlCloudinary">URL de Cloudinary</param>
        /// <param name="baseUrl">URL base para archivos locales</param>
        /// <returns>URL óptima para acceder al archivo</returns>
        string ObtenerMejorUrl(string? rutaLocal, string? urlCloudinary, string baseUrl = "https://localhost:7072");
        
        /// <summary>
        /// Valida si un archivo es válido (tamaño, extensión, etc.)
        /// </summary>
        /// <param name="archivo">Archivo a validar</param>
        /// <param name="maxSizeBytes">Tamaño máximo permitido</param>
        /// <param name="allowedExtensions">Extensiones permitidas</param>
        /// <returns>True si el archivo es válido</returns>
        bool ValidarArchivo(IFormFile archivo, long maxSizeBytes = 10485760, string[]? allowedExtensions = null);
    }
}
