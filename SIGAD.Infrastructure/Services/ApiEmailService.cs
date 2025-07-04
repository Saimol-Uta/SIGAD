using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIGAD.Application.Interfaces;
using SIGAD.Infrastructure.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace SIGAD.Infrastructure.Services
{
    // Esta es la clase que implementa la interfaz
    public class ApiEmailService : IApiEmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const string ApiEndpoint = "https://api-b7rtqstgmq-uc.a.run.app/enviarCorreo";

        public ApiEmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var httpClient = _httpClientFactory.CreateClient();

            // Usamos la clase "molde" definida abajo para crear el payload
            var requestPayload = new ApiEmailRequest
            {
                To = to,
                Subject = subject,
                Text = body
            };

            await httpClient.PostAsJsonAsync(ApiEndpoint, requestPayload);
        }
    }

    // Esta es la clase "molde" para el JSON.
    // La ponemos aquí mismo para mantenerlo simple y organizado.
    internal class ApiEmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}