using SIGAD.Application.DTOs;

namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para operaciones de autenticación (login).
    /// Principio SRP: Enfocado solo en la autenticación de usuarios.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica un usuario con email y contraseña.
        /// </summary>
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);

        /// <summary>
        /// Valida las credenciales de un usuario sin generar token.
        /// </summary>
        Task<bool> ValidateCredentialsAsync(string email, string password);

        /// <summary>
        /// Verifica un hash de contraseña.
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }
}
