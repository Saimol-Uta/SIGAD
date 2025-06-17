using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using SIGAD.Application.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var oauthSettings = _configuration.GetSection("GoogleSmtpOAuth");
            var userEmail = oauthSettings["SenderEmail"];
            var clientId = oauthSettings["ClientId"];
            var clientSecret = oauthSettings["ClientSecret"];
            var refreshToken = oauthSettings["RefreshToken"];

            var accessToken = await GetGoogleOAuth2AccessToken(clientId, clientSecret, refreshToken);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Sistema SIGAD", userEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = $"<p>{body}</p>" };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            var saslMechanism = new SaslMechanismOAuth2(userEmail, accessToken);
            await smtp.AuthenticateAsync(saslMechanism);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        // Método privado para manejar la obtención del token de acceso de Google
        private async Task<string> GetGoogleOAuth2AccessToken(string clientId, string clientSecret, string refreshToken)
        {
            using var httpClient = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
            });

            var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", requestContent);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Error al obtener el token de acceso de Google: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var accessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ApplicationException("El token de acceso obtenido de Google es nulo o vacío.");
            }

            return accessToken;
        }
    }
}