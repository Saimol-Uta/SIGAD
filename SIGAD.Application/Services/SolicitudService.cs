using System.Net.Http.Json;

public class SolicitudService
{
    private readonly HttpClient _http;

    public SolicitudService(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> GetSolicitudActualAsync()
    {
        var response = await _http.GetAsync("api/solicitudes/verif-solicitud-activa");

        if (!response.IsSuccessStatusCode)
            return null;

        var resultado = await response.Content.ReadFromJsonAsync<SolicitudActivaResponse>();

        return resultado?.TieneBorrador == true ? resultado.SolicitudId : null;
    }

    private class SolicitudActivaResponse
    {
        public bool TieneBorrador { get; set; }
        public string? SolicitudId { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
