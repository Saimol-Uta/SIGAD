using SIGAD.BlazorApp.Models;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.BlazorApp.Services
{
    public interface INotificacionClienteService
    {
        Task<int> GetUnreadCountAsync();
        Task<List<NotificacionDto>> GetNotificacionesAsync();
        Task MarkAsReadAsync(int notificacionId);
    }

    public class NotificacionClienteService : INotificacionClienteService
    {
        private readonly HttpClient _httpClient;

        public NotificacionClienteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetUnreadCountAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<UnreadCountResponse>("api/notificaciones/unread-count");
                return result?.UnreadCount ?? 0;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<List<NotificacionDto>> GetNotificacionesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<NotificacionDto>>("api/notificaciones");
            }
            catch
            {
                return new List<NotificacionDto>();
            }
        }

        public async Task MarkAsReadAsync(int notificacionId)
        {
            try
            {
                await _httpClient.PostAsync($"api/notificaciones/{notificacionId}/mark-as-read", null);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error al marcar notificación como leída: {ex.Message}");
            }
        }

        private class UnreadCountResponse
        {
            public int UnreadCount { get; set; }
        }
    }
}