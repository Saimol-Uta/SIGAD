using SIGAD.Application.DTOs;
using System.Net.Http.Json;

namespace SIGAD.BlazorApp.ApiClients
{
    /// <summary>
    /// Cliente tipado para operaciones de comando (Command) de solicitudes.
    /// Principio CQRS y ISP: Solo contiene métodos de escritura/modificación.
    /// </summary>
    public interface ISolicitudesCommandApiClient
    {
        Task<SolicitudDetalleDto?> CrearSolicitudAsync(CrearSolicitudDto dto);
        Task<bool> EnviarSolicitudAsync(Guid solicitudId);
        Task<bool> AprobarPorComisionAsync(Guid solicitudId, string? observaciones = null);
        Task<bool> AprobarPorConsejoAsync(Guid solicitudId, string? observaciones = null);
        Task<bool> RechazarSolicitudAsync(Guid solicitudId, string observaciones);
        Task<bool> CancelarSolicitudAsync(Guid solicitudId);
        Task<bool> AsociarArticuloAsync(AsociarArticuloSolicitudDto dto);
        Task<bool> DesasociarArticuloAsync(DesasociarArticuloSolicitudDto dto);
        Task<bool> AsociarCursoAsync(AsociarCursoSolicitudDto dto);
        Task<bool> AsociarInvestigacionAsync(AsociarInvestigacionSolicitudDto dto);
        Task<bool> AsociarTesisAsync(AsociarTesisSolicitudDto dto);
        Task<bool> AsociarEvaluacionAsync(AsociarEvaluacionSolicitudDto dto);
    }

    public class SolicitudesCommandApiClient : ISolicitudesCommandApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseRoute = "api/Ascensos";

        public SolicitudesCommandApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SolicitudDetalleDto?> CrearSolicitudAsync(CrearSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}", dto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SolicitudDetalleDto>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> EnviarSolicitudAsync(Guid solicitudId)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/{solicitudId}/enviar", new { });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AprobarPorComisionAsync(Guid solicitudId, string? observaciones = null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/{solicitudId}/aprobar-comision", new { observaciones });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AprobarPorConsejoAsync(Guid solicitudId, string? observaciones = null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/{solicitudId}/aprobar-consejo", new { observaciones });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RechazarSolicitudAsync(Guid solicitudId, string observaciones)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/{solicitudId}/rechazar", new { observaciones });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelarSolicitudAsync(Guid solicitudId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{BaseRoute}/{solicitudId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsociarArticuloAsync(AsociarArticuloSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/asociar-articulo", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DesasociarArticuloAsync(DesasociarArticuloSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/desasociar-articulo", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsociarCursoAsync(AsociarCursoSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/asociar-curso", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsociarInvestigacionAsync(AsociarInvestigacionSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/asociar-investigacion", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsociarTesisAsync(AsociarTesisSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/asociar-tesis", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AsociarEvaluacionAsync(AsociarEvaluacionSolicitudDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseRoute}/asociar-evaluacion", dto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
