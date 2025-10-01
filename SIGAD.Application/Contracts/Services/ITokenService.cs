namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para generación y gestión de tokens JWT.
    /// Principio SRP: Enfocado exclusivamente en la generación de tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Genera un token JWT para un usuario autenticado.
        /// </summary>
        string GenerateJwtToken(
            string correo,
            string rol,
            string cedula,
            string nombre1,
            string? nombre2,
            string apellido1,
            string apellido2,
            int? rangoId,
            string? rangoNombre);

        /// <summary>
        /// Valida un token JWT y devuelve sus claims.
        /// </summary>
        Task<Dictionary<string, string>?> ValidateTokenAsync(string token);

        /// <summary>
        /// Extrae el identificador de usuario de un token.
        /// </summary>
        string? GetUserIdFromToken(string token);
    }
}
