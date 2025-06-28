using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SIGAD.BlazorApp.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIGAD.BlazorApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest);
            if (!response.IsSuccessStatusCode)
            {
                return null; // O manejar el error específico
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return null;
            }

            await _localStorage.SetItemAsync("authToken", loginResponse.Token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse.Token);

            return loginResponse;
        }
        public async Task<bool> Register(RegisterRequestDto registerRequest)
        {
            // Solo enviar los campos requeridos por el nuevo flujo
            var apiRequest = new
            {
                Correo = registerRequest.Correo,
                Clave = registerRequest.Clave,
                Cedula = registerRequest.Cedula
            };
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", apiRequest);
            return response.IsSuccessStatusCode;
        }

        // Verifica si la cédula existe en la base de datos
        public async Task<bool> CedulaExisteAsync(string cedula)
        {
            var response = await _httpClient.GetAsync($"api/Auth/cedula-existe/{cedula}");
            if (!response.IsSuccessStatusCode)
                return false;
            var existe = await response.Content.ReadFromJsonAsync<bool>();
            return existe;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        public async Task<bool> RegisterSimple(RegisterSimpleDto registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register-simple", registerRequest);
            return response.IsSuccessStatusCode;
        }
    }
    public interface IAuthService
    {
        Task<LoginResponseDto?> Login(LoginRequestDto loginRequest);
        Task<bool> Register(RegisterRequestDto registerRequest);
        Task<bool> CedulaExisteAsync(string cedula);
        Task Logout();
        Task<bool> RegisterSimple(RegisterSimpleDto registerRequest);
    }
}