using SIGAD.Application.DTOs;

namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para operaciones de registro de usuarios.
    /// Principio SRP: Enfocado solo en el registro y creación de cuentas.
    /// </summary>
    public interface IUserRegistrationService
    {
        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        Task<bool> RegisterAsync(RegisterRequestDto registerRequest);

        /// <summary>
        /// Genera un hash seguro de una contraseña.
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con su hash.
        /// </summary>
        bool VerifyPassword(string password, string hash);

        /// <summary>
        /// Valida si un email ya está registrado.
        /// </summary>
        Task<bool> EmailExistsAsync(string email);
    }
}
