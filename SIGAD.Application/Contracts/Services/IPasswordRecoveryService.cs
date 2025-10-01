namespace SIGAD.Application.Contracts.Services
{
    /// <summary>
    /// Contrato para operaciones de recuperación de contraseña.
    /// Principio SRP: Enfocado exclusivamente en el proceso de recuperación de contraseña.
    /// </summary>
    public interface IPasswordRecoveryService
    {
        /// <summary>
        /// Solicita un código de recuperación de contraseña para un email.
        /// </summary>
        Task<bool> SolicitarRecuperacionAsync(string email);

        /// <summary>
        /// Verifica si un código de recuperación es válido para un email.
        /// </summary>
        Task<bool> VerificarCodigoAsync(string email, string codigo);

        /// <summary>
        /// Restablece la contraseña de un usuario usando el código de recuperación.
        /// </summary>
        Task<bool> RestablecerContrasenaAsync(string email, string codigo, string nuevaContrasena, string confirmarContrasena);
    }
}
