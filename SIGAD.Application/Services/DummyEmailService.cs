using Microsoft.Extensions.Logging;
using SIGAD.Application.Services;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Services
{
    public class DummyEmailService : IEmailService
    {
        private readonly ILogger<DummyEmailService> _logger;

        public DummyEmailService(ILogger<DummyEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // En lugar de enviar un correo real, lo imprimimos en la consola de depuración de la WebAPI.
            _logger.LogWarning("--- SIMULACIÓN DE ENVÍO DE CORREO ---");
            _logger.LogInformation("Para: {Email}", toEmail);
            _logger.LogInformation("Asunto: {Subject}", subject);
            _logger.LogInformation("Cuerpo del Mensaje: {Body}", body);
            _logger.LogWarning("------------------------------------");

            return Task.CompletedTask;
        }
    }
}