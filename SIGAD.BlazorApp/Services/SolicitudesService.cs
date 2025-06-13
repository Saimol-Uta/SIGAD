using SIGAD.BlazorApp.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace SIGAD.BlazorApp.Services
{
    public interface ISolicitudesService
    {
        Task<List<SolicitudDto>> GetAllSolicitudesAsync();
        Task<SolicitudDto?> GetSolicitudByIdAsync(Guid id);
        Task<SolicitudDetalleDto?> GetSolicitudDetalleAsync(Guid id);
        Task<bool> AprobarSolicitudAsync(Guid id, string observaciones);
        Task<bool> RechazarSolicitudAsync(Guid id, string observaciones);
    }

    public class SolicitudesService : ISolicitudesService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public SolicitudesService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task EnsureAuthenticationHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                // Limpiar comillas extras si las hay
                token = token.Trim('"');
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<SolicitudDto>> GetAllSolicitudesAsync()
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudDto>>("api/solicitudes");
                return response ?? new List<SolicitudDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener solicitudes: {ex.Message}");
                return new List<SolicitudDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<SolicitudDto>();
            }
        }
        public async Task<SolicitudDto?> GetSolicitudByIdAsync(Guid id)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                return await _httpClient.GetFromJsonAsync<SolicitudDto>($"api/solicitudes/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener solicitud {id}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }

        public async Task<SolicitudDetalleDto?> GetSolicitudDetalleAsync(Guid id)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                return await _httpClient.GetFromJsonAsync<SolicitudDetalleDto>($"api/solicitudes/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener detalle de solicitud {id}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AprobarSolicitudAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/aprobar", observaciones);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al aprobar solicitud {id}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RechazarSolicitudAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/rechazar", observaciones);
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al rechazar solicitud {id}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }
    }
}
