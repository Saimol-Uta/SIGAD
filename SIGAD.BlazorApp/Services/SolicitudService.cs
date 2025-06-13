using Microsoft.JSInterop;
using System.Net.Http;
using System.Text.Json;

namespace SIGAD.BlazorApp.Services
{
    public interface ISolicitudService
    {
        Task<string?> GetSolicitudActualAsync();
        Task SetSolicitudActualAsync(string solicitudId);
        Task ClearSolicitudActualAsync();
        Task<bool> TieneSolicitudActualAsync();
        Task<bool> VerificarSolicitudExisteAsync(string solicitudId);
    }

    public class SolicitudService : ISolicitudService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private const string STORAGE_KEY = "solicitudActualId";

        public SolicitudService(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public async Task<string?> GetSolicitudActualAsync()
        {
            try
            {
                var solicitudId = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", STORAGE_KEY);
                return string.IsNullOrEmpty(solicitudId) ? null : solicitudId;
            }
            catch
            {
                return null;
            }
        }

        public async Task SetSolicitudActualAsync(string solicitudId)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, solicitudId);
        }

        public async Task ClearSolicitudActualAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", STORAGE_KEY);
        }

        public async Task<bool> TieneSolicitudActualAsync()
        {
            try
            {
                // Verificar directamente con el backend si hay solicitud activa
                var response = await _httpClient.GetAsync("api/auth/verificar-solicitud-activa");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<VerificarSolicitudResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.Success == true && result.TieneSolicitudActiva && !string.IsNullOrEmpty(result.SolicitudId))
                    {
                        // Actualizar localStorage con el ID correcto
                        await SetSolicitudActualAsync(result.SolicitudId);
                        return true;
                    }
                    else
                    {
                        // No hay solicitud activa, limpiar localStorage
                        await ClearSolicitudActualAsync();
                        return false;
                    }
                }
                
                return false;
            }
            catch
            {
                // En caso de error, verificar solo localStorage
                var solicitudId = await GetSolicitudActualAsync();
                return !string.IsNullOrEmpty(solicitudId);
            }
        }

        public async Task<bool> VerificarSolicitudExisteAsync(string solicitudId)
        {
            try
            {
                // Intentar obtener la solicitud del backend
                var response = await _httpClient.GetAsync($"api/solicitudesAscenso/{solicitudId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class VerificarSolicitudResponse
    {
        public bool Success { get; set; }
        public bool TieneSolicitudActiva { get; set; }
        public string? SolicitudId { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
} 