using System.Net.Http.Json;
using SIGAD.BlazorApp.Models;

namespace SIGAD.BlazorApp.Services
{
    public class ReporteService
    {
        private readonly HttpClient _http;

        public ReporteService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<ReporteDataDto>?> GetReportePorEstadoAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ReporteDataDto>>("api/reportes/solicitudes-por-estado");
        }

        public async Task<IEnumerable<ReporteDataDto>?> GetReportePorNivelAsync()
        {
            return await _http.GetFromJsonAsync<IEnumerable<ReporteDataDto>>("api/reportes/solicitudes-por-nivel");
        }

        public async Task<IEnumerable<ReporteDataDto>?> GetReportePorMesAsync(int anio)
        {
            // Así pasamos parámetros en la URL
            return await _http.GetFromJsonAsync<IEnumerable<ReporteDataDto>>($"api/reportes/solicitudes-por-mes/{anio}");
        }
    }
}
