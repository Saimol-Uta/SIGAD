using MailKit.Security;
using Microsoft.Extensions.Logging;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Services
{
    // Esta será la clase que realmente inyectaremos como IEmailService
    public class ResilientEmailService : IEmailService
    {
        private readonly SmtpEmailService _primaryEmailService; // El de MailKit
        private readonly IApiEmailService _fallbackEmailService;  // El de la API externa
        private readonly ILogger<ResilientEmailService> _logger;

        public ResilientEmailService(
            SmtpEmailService primaryEmailService,
            IApiEmailService fallbackEmailService,
            ILogger<ResilientEmailService> logger)
        {
            _primaryEmailService = primaryEmailService;
            _fallbackEmailService = fallbackEmailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Intento 1: Usar el servicio primario (MailKit + OAuth)
                _logger.LogInformation("Intentando enviar correo a {ToEmail} usando el servicio primario (MailKit).", toEmail);
                await _primaryEmailService.SendEmailAsync(toEmail, subject, body);
                _logger.LogInformation("Correo enviado exitosamente a {ToEmail} con el servicio primario.", toEmail);
            }
            catch (Exception ex) when (ex is AuthenticationException || ex is SaslException)
            {
                // ¡Falla de autenticación! (Probablemente el token)
                _logger.LogWarning(ex, "El servicio de correo primario (MailKit) falló por autenticación. Activando fallback a la API externa para el correo a {ToEmail}.", toEmail);

                // Intento 2: Usar el servicio de fallback (API Externa)
                await _fallbackEmailService.SendEmailAsync(toEmail, subject, body);
                _logger.LogInformation("Correo enviado exitosamente a {ToEmail} con el servicio de FALLBACK (API Externa).", toEmail);
            }
            catch (Exception ex)
            {
                // Otro tipo de error que no sea de autenticación (ej. de conexión)
                _logger.LogError(ex, "Ocurrió un error inesperado al enviar el correo a {ToEmail}.", toEmail);
                // Relanzamos la excepción para que la operación que llamó al servicio sepa que algo falló.
                throw;
            }
        }
    }
}