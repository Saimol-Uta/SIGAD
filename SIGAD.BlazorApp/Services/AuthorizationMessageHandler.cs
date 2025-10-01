using SIGAD.BlazorApp.Abstractions;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SIGAD.BlazorApp.Services
{
    /// <summary>
    /// Handler HTTP que inyecta automáticamente el token JWT en las peticiones.
    /// Refactorizado para usar ITokenProvider (principio DIP).
    /// </summary>
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthorizationMessageHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Usar la abstracción para obtener el token
            var token = await _tokenProvider.GetTokenAsync();

            // Si el token existe, lo añadimos al encabezado de la petición
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Continuamos con el envío de la petición ya modificada
            return await base.SendAsync(request, cancellationToken);
        }
    }
}