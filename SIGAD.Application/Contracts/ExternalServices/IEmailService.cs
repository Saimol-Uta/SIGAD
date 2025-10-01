namespace SIGAD.Application.Contracts.ExternalServices
{
    /// <summary>
    /// Contrato para servicios de envío de correos electrónicos.
    /// Principio DIP: Application define el contrato, Infrastructure lo implementa.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico simple.
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Envía un correo electrónico con una plantilla HTML.
        /// </summary>
        Task<bool> SendTemplatedEmailAsync(string to, string subject, string templateName, Dictionary<string, string> parameters);

        /// <summary>
        /// Envía un correo de recuperación de contraseña.
        /// </summary>
        Task<bool> SendPasswordRecoveryEmailAsync(string to, string recoveryCode);

        /// <summary>
        /// Envía una notificación de cambio de estado de solicitud.
        /// </summary>
        Task<bool> SendSolicitudStatusEmailAsync(string to, string docenteName, string estadoSolicitud, string? observaciones = null);
    }
}
