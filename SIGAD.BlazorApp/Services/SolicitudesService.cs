using SIGAD.BlazorApp.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;

namespace SIGAD.BlazorApp.Services
{
    public interface ISolicitudesService
    {
        Task<List<SolicitudDto>> GetAllSolicitudesAsync();
        Task<SolicitudDto?> GetSolicitudByIdAsync(Guid id);
        Task<SolicitudDetalleDto?> GetSolicitudDetalleAsync(Guid id);
        Task<(bool success, string message)> AprobarSolicitudAsync(Guid id, string observaciones);
        Task<(bool success, string message)> RechazarSolicitudAsync(Guid id, string observaciones);
        
        // Métodos específicos para el proceso de dos etapas según Reglamento UTA
        Task<(bool success, string message)> AprobarPorComisionAsync(Guid id, string observaciones);
        Task<(bool success, string message)> AprobarPorConsejoAsync(Guid id, string observaciones);
        Task<(bool success, string message)> FinalizarProcesoAsync(Guid id, string observaciones);
        
        // Método para depuración
        Task<string> GetAuthStatusAsync();

        // Métodos para apelaciones (docente)
        Task<(bool success, string message)> PresentarApelacionAsync(Guid solicitudId, string justificacion, List<IBrowserFile> archivos);
        Task<List<ApelacionDto>> GetApelacionesBySolicitudAsync(Guid solicitudId);
        
        // Métodos para administrador de apelaciones
        Task<List<SolicitudConApelacionDto>> GetSolicitudesConApelacionesAsync();
        Task<ApelacionDetalleDto?> GetApelacionDetalleAsync(Guid solicitudId);
        Task<(bool success, string message)> ResolverApelacionAsync(int apelacionId, bool aceptada, string observaciones);
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

        // Método para depuración de autenticación
        public async Task<string> GetAuthStatusAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    return "No hay token de autenticación almacenado";
                }
                
                token = token.Trim('"');
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Probar endpoint de prueba con autenticación
                var response = await _httpClient.GetAsync("api/solicitudes/test-auth");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return $"Autenticación exitosa: {content}";
                }
                else
                {
                    return $"Error de autenticación: HTTP {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Error al verificar autenticación: {ex.Message}";
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

        public async Task<(bool success, string message)> AprobarSolicitudAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/aprobar", observaciones);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Solicitud aprobada exitosamente");
                }
                else
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (false, errorResult?.Message ?? "Error al aprobar la solicitud");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al aprobar solicitud {id}: {ex.Message}");
                return (false, "Error de conexión al aprobar la solicitud");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al aprobar la solicitud");
            }
        }

        public async Task<(bool success, string message)> RechazarSolicitudAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/rechazar", observaciones);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Solicitud rechazada exitosamente");
                }
                else
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (false, errorResult?.Message ?? "Error al rechazar la solicitud");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al rechazar solicitud {id}: {ex.Message}");
                return (false, "Error de conexión al rechazar la solicitud");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al rechazar la solicitud");
            }
        }

        // Métodos específicos para el proceso de dos etapas según Reglamento UTA
        public async Task<(bool success, string message)> AprobarPorComisionAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                
                // Crear un objeto para el body de la petición
                var requestBody = new { observaciones = observaciones };
                
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/aprobar-comision", requestBody);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Solicitud aprobada por Comisión exitosamente");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, "No tienes autorización para realizar esta acción. Verifica que estés logueado como ADMINISTRADOR.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error HTTP {response.StatusCode}: {errorContent}");
                    
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                        return (false, errorResult?.Message ?? $"Error HTTP {response.StatusCode}");
                    }
                    catch
                    {
                        return (false, $"Error HTTP {response.StatusCode}: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al aprobar por Comisión {id}: {ex.Message}");
                return (false, "Error de conexión al aprobar por Comisión");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al aprobar por Comisión");
            }
        }

        public async Task<(bool success, string message)> AprobarPorConsejoAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                
                // Crear un objeto para el body de la petición
                var requestBody = new { observaciones = observaciones };
                
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/aprobar-consejo", requestBody);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Solicitud aprobada por Consejo Universitario exitosamente");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, "No tienes autorización para realizar esta acción. Verifica que estés logueado como ADMINISTRADOR.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error HTTP {response.StatusCode}: {errorContent}");
                    
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                        return (false, errorResult?.Message ?? $"Error HTTP {response.StatusCode}");
                    }
                    catch
                    {
                        return (false, $"Error HTTP {response.StatusCode}: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al aprobar por Consejo {id}: {ex.Message}");
                return (false, "Error de conexión al aprobar por Consejo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al aprobar por Consejo");
            }
        }

        public async Task<(bool success, string message)> FinalizarProcesoAsync(Guid id, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                
                // Crear un objeto para el body de la petición
                var requestBody = new { observaciones = observaciones };
                
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/finalizar", requestBody);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Proceso de ascenso finalizado exitosamente");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, "No tienes autorización para realizar esta acción. Verifica que estés logueado como ADMINISTRADOR.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error HTTP {response.StatusCode}: {errorContent}");
                    
                    try
                    {
                        var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                        return (false, errorResult?.Message ?? $"Error HTTP {response.StatusCode}");
                    }
                    catch
                    {
                        return (false, $"Error HTTP {response.StatusCode}: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al finalizar proceso {id}: {ex.Message}");
                return (false, "Error de conexión al finalizar el proceso");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al finalizar el proceso");
            }
        }

        // Métodos para apelaciones
        public async Task<(bool success, string message)> PresentarApelacionAsync(Guid solicitudId, string justificacion, List<IBrowserFile> archivos)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                
                // Crear FormData para enviar archivos
                using var formData = new MultipartFormDataContent();
                
                // Agregar datos básicos
                formData.Add(new StringContent(solicitudId.ToString()), "SolicitudId");
                formData.Add(new StringContent(justificacion), "Justificacion");
                
                // Agregar archivos si existen
                if (archivos != null && archivos.Count > 0)
                {
                    foreach (var archivo in archivos)
                    {
                        var streamContent = new StreamContent(archivo.OpenReadStream());
                        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(archivo.ContentType);
                        formData.Add(streamContent, "DocumentosAdjuntos", archivo.Name);
                    }
                }
                
                var response = await _httpClient.PostAsync("api/apelaciones", formData);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (true, result?.Message ?? "Apelación presentada exitosamente");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error response: {errorContent}");
                    var errorResult = await response.Content.ReadFromJsonAsync<ApiResponse>();
                    return (false, errorResult?.Message ?? "Error al presentar la apelación");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al presentar apelación para solicitud {solicitudId}: {ex.Message}");
                return (false, "Error de conexión al presentar la apelación");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, "Error inesperado al presentar la apelación");
            }
        }

        public async Task<List<ApelacionDto>> GetApelacionesBySolicitudAsync(Guid solicitudId)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<List<ApelacionDto>>($"api/apelaciones/solicitud/{solicitudId}");
                return response ?? new List<ApelacionDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener apelaciones para solicitud {solicitudId}: {ex.Message}");
                return new List<ApelacionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<ApelacionDto>();
            }
        }

        // Métodos para administrador de apelaciones
        public async Task<List<SolicitudConApelacionDto>> GetSolicitudesConApelacionesAsync()
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudConApelacionDto>>("api/solicitudes/con-apelaciones");
                return response ?? new List<SolicitudConApelacionDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener solicitudes con apelaciones: {ex.Message}");
                return new List<SolicitudConApelacionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<SolicitudConApelacionDto>();
            }
        }

        public async Task<ApelacionDetalleDto?> GetApelacionDetalleAsync(Guid solicitudId)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                var response = await _httpClient.GetFromJsonAsync<ApelacionDetalleDto>($"api/apelaciones/detalle/{solicitudId}");
                return response;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener detalle de apelación para solicitud {solicitudId}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool success, string message)> ResolverApelacionAsync(int apelacionId, bool aceptada, string observaciones)
        {
            try
            {
                await EnsureAuthenticationHeaderAsync();
                // DEBUG: Mostrar token antes de la petición
                var token = await _localStorage.GetItemAsync<string>("authToken");
                Console.WriteLine($"Token usado para resolver apelación: {token}");

                var request = new Models.ResolverApelacionDto
                {
                    Aceptada = aceptada,
                    ObservacionesComision = observaciones ?? string.Empty
                };
                var response = await _httpClient.PostAsJsonAsync($"api/apelaciones/resolver/{apelacionId}", request);

                var responseText = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta cruda del backend: {responseText}");

                if (response.IsSuccessStatusCode)
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(responseText, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (true, result?.Message ?? "Apelación resuelta exitosamente");
                }
                else
                {
                    // Intentar deserializar, pero si falla, mostrar el texto crudo
                    try
                    {
                        var errorResult = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(responseText, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        return (false, errorResult?.Message ?? $"Error al resolver la apelación: {responseText}");
                    }
                    catch
                    {
                        return (false, $"Error al resolver la apelación: {responseText}");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al resolver apelación {apelacionId}: {ex.Message}");
                return (false, "Error de conexión al resolver la apelación");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return (false, $"Error inesperado al resolver la apelación: {ex.Message}");
            }
        }
     }

     // Clase para deserializar respuestas del API
     public class ApiResponse
     {
         public bool Success { get; set; }
         public string Message { get; set; } = string.Empty;
         public string? Field { get; set; }
     }
}
