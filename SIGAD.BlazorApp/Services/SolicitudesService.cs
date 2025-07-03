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
        Task<(bool success, string message)> AprobarSolicitudAsync(Guid id, string observaciones);
        Task<(bool success, string message)> RechazarSolicitudAsync(Guid id, string observaciones);
        
        // Métodos específicos para el proceso de dos etapas según Reglamento UTA
        Task<(bool success, string message)> AprobarPorComisionAsync(Guid id, string observaciones);
        Task<(bool success, string message)> AprobarPorConsejoAsync(Guid id, string observaciones);
        Task<(bool success, string message)> FinalizarProcesoAsync(Guid id, string observaciones);
        
        // Método para depuración
        Task<string> GetAuthStatusAsync();
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
     }

     // Clase para deserializar respuestas del API
     public class ApiResponse
     {
         public bool Success { get; set; }
         public string Message { get; set; } = string.Empty;
         public string? Field { get; set; }
     }
}
