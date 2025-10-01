using SIGAD.Application.DTOs;
using System.Net.Http.Json;

namespace SIGAD.BlazorApp.ApiClients
{
    /// <summary>
    /// Cliente tipado para operaciones de consulta (Query) de solicitudes.
    /// Principio CQRS y ISP: Solo contiene métodos de lectura.
    /// </summary>
    public interface ISolicitudesQueryApiClient
    {
        Task<SolicitudDetalleDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<SolicitudDetalleDto>> GetAllAsync();
        Task<IEnumerable<SolicitudDetalleDto>> GetHistorialByDocenteAsync(string docenteCedula);
        Task<bool> HasActiveSolicitudAsync(string docenteCedula);
        Task<SolicitudDetalleDto?> GetActiveSolicitudByDocenteAsync(string docenteCedula);
    }

    public class SolicitudesQueryApiClient : ISolicitudesQueryApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Ascensos";

        public SolicitudesQueryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SolicitudDetalleDto?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<SolicitudDetalleDto>($"{BaseRoute}/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetAllAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<SolicitudDetalleDto>>($"{BaseRoute}");
                return result ?? Enumerable.Empty<SolicitudDetalleDto>();
            }
            catch
            {
                return Enumerable.Empty<SolicitudDetalleDto>();
            }
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetHistorialByDocenteAsync(string docenteCedula)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<IEnumerable<SolicitudDetalleDto>>($"{BaseRoute}/historial/{docenteCedula}");
                return result ?? Enumerable.Empty<SolicitudDetalleDto>();
            }
            catch
            {
                return Enumerable.Empty<SolicitudDetalleDto>();
            }
        }

        public async Task<bool> HasActiveSolicitudAsync(string docenteCedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseRoute}/verificar-activa");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<SolicitudDetalleDto?> GetActiveSolicitudByDocenteAsync(string docenteCedula)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<SolicitudDetalleDto>($"{BaseRoute}/activa/{docenteCedula}");
            }
            catch
            {
                return null;
            }
        }
    }
}
