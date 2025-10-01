using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SIGAD.BlazorApp.Models;
using SIGAD.BlazorApp.Abstractions;
using SIGAD.BlazorApp.ApiClients;
using SIGAD.BlazorApp.Extensions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIGAD.BlazorApp.Services
{
    /// <summary>
    /// Servicio de autenticación refactorizado con principios SOLID.
    /// Usa IAuthApiClient (cliente tipado) e ITokenProvider (abstracción de almacenamiento).
    /// Fase 3: Usa métodos de extensión para mapeo de DTOs.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IAuthApiClient _authClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _httpClient; // Mantener temporalmente para métodos legacy

        public AuthService(
            IAuthApiClient authClient,
            ITokenProvider tokenProvider,
            AuthenticationStateProvider authenticationStateProvider,
            HttpClient httpClient)
        {
            _authClient = authClient;
            _tokenProvider = tokenProvider;
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto loginRequest)
        {
            // Mapear el DTO de Blazor al DTO de Application
            var appLoginRequest = new Application.DTOs.LoginRequestDto
            {
                Correo = loginRequest.Correo,
                Clave = loginRequest.Clave
            };

            // Usar el cliente tipado para la llamada a la API
            var appLoginResponse = await _authClient.LoginAsync(appLoginRequest);

            if (appLoginResponse == null || string.IsNullOrEmpty(appLoginResponse.Token))
            {
                return null;
            }

            // Usar el método de extensión para mapear (Fase 3: DtoMappingExtensions)
            var loginResponse = appLoginResponse.ToBlazorLoginResponseDto();

            // Usar la abstracción de token provider
            await _tokenProvider.SetTokenAsync(loginResponse.Token);

            // Notificar al provider de autenticación
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResponse.Token);

            // Mantener compatibilidad con HttpClient legacy
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse.Token);

            return loginResponse;
        }
        public async Task<bool> Register(RegisterRequestDto registerRequest)
        {
            // El DTO de Application.RegisterRequestDto tiene más campos que el de Blazor
            // Por ahora, mantener la implementación legacy usando HttpClient directamente
            // TODO: Actualizar cuando se alineen los DTOs o se cree un RegisterSimpleRequestDto en Application
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
            // Mantener implementación legacy por ahora (no está en IAuthApiClient)
            var response = await _httpClient.GetAsync($"api/Auth/cedula-existe/{cedula}");
            if (!response.IsSuccessStatusCode)
                return false;
            var existe = await response.Content.ReadFromJsonAsync<bool>();
            return existe;
        }

        public async Task Logout()
        {
            // Usar la abstracción de token provider
            await _tokenProvider.RemoveTokenAsync();

            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<bool> RegisterSimple(RegisterSimpleDto registerRequest)
        {
            // Mantener implementación legacy por ahora (no está en IAuthApiClient)
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