using SIGAD.Application.DTOs;
using System.Net.Http.Json;

namespace SIGAD.BlazorApp.ApiClients
{
    /// <summary>
    /// Cliente tipado para operaciones de autenticación.
    /// Encapsula las llamadas HTTP y las rutas de la API de autenticación.
    /// Principio SRP y DIP: Componentes dependen de esta abstracción, no de HttpClient directo.
    /// </summary>
    public interface IAuthApiClient
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<bool> RegisterAsync(RegisterRequestDto request);
        Task<bool> SolicitarRecuperacionAsync(string email);
        Task<bool> RestablecerContrasenaAsync(string email, string codigo, string nuevaContrasena, string confirmarContrasena);
        Task<bool> VerificarCodigoAsync(string email, string codigo);
    }

    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Auth";

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/login", request);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/register", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SolicitarRecuperacionAsync(string email)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/solicitar-recuperacion", new { email });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestablecerContrasenaAsync(string email, string codigo, string nuevaContrasena, string confirmarContrasena)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/restablecer-contrasena", new
                {
                    email,
                    codigo,
                    nuevaContrasena,
                    confirmarContrasena
                });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VerificarCodigoAsync(string email, string codigo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/verificar-codigo", new { email, codigo });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
