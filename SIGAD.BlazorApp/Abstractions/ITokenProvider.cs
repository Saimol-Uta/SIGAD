namespace SIGAD.BlazorApp.Abstractions
{
    /// <summary>
    /// Abstracción para el proveedor de tokens de autenticación.
    /// Principio DIP: Los servicios dependen de esta interfaz, no de la implementación concreta (LocalStorage).
    /// Principio OCP: Permite cambiar la forma de almacenar el token sin modificar los consumidores.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Obtiene el token de autenticación almacenado.
        /// </summary>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Guarda el token de autenticación.
        /// </summary>
        Task SetTokenAsync(string token);

        /// <summary>
        /// Elimina el token de autenticación.
        /// </summary>
        Task RemoveTokenAsync();

        /// <summary>
        /// Verifica si existe un token almacenado.
        /// </summary>
        Task<bool> HasTokenAsync();
    }
}
