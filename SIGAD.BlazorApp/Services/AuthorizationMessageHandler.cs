using Blazored.LocalStorage;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SIGAD.BlazorApp.Services
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthorizationMessageHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Intentar obtener el token del local storage
            var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);

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