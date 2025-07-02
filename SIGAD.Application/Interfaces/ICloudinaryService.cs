namespace SIGAD.Application.Interfaces
{
    /// <summary>
    /// Servicio específico para operaciones con Cloudinary
    /// </summary>
    public interface ICloudinaryService
    {
        /// <summary>
        /// Sube un archivo a Cloudinary
        /// </summary>
        /// <param name="contenido">Contenido del archivo en bytes</param>
        /// <param name="tipoDocumento">Tipo de documento para organización en folders</param>
        /// <param name="identificador">Identificador único del archivo</param>
        /// <param name="extension">Extensión del archivo</param>
        /// <returns>URL pública de Cloudinary</returns>
        Task<string> SubirArchivoAsync(byte[] contenido, string tipoDocumento, string identificador, string extension);
        
        /// <summary>
        /// Elimina un archivo de Cloudinary usando su URL
        /// </summary>
        /// <param name="urlCloudinary">URL completa del archivo en Cloudinary</param>
        /// <returns>True si la eliminación fue exitosa</returns>
        Task<bool> EliminarArchivoAsync(string urlCloudinary);
        
        /// <summary>
        /// Extrae el public_id de una URL de Cloudinary
        /// </summary>
        /// <param name="urlCloudinary">URL de Cloudinary</param>
        /// <returns>Public ID del archivo</returns>
        string ExtraerPublicId(string urlCloudinary);
        
        /// <summary>
        /// Verifica si el servicio de Cloudinary está disponible
        /// </summary>
        /// <returns>True si Cloudinary está configurado y disponible</returns>
        Task<bool> VerificarDisponibilidadAsync();
    }
}
