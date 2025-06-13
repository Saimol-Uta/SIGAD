using Microsoft.JSInterop;

namespace SIGAD.BlazorApp.Services
{
    public interface ISolicitudService
    {
        Task<string?> GetSolicitudActualAsync();
        Task SetSolicitudActualAsync(string solicitudId);
        Task ClearSolicitudActualAsync();
        Task<bool> TieneSolicitudActualAsync();
    }

    public class SolicitudService : ISolicitudService
    {
        private readonly IJSRuntime _jsRuntime;
        private const string STORAGE_KEY = "solicitudActualId";

        public SolicitudService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
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
            var solicitudId = await GetSolicitudActualAsync();
            return !string.IsNullOrEmpty(solicitudId);
        }
    }
} 