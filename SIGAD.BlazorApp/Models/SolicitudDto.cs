namespace SIGAD.BlazorApp.Models
{
    public class SolicitudDto
    {
        public Guid Id { get; set; }
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string RangoSolicitadoNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaEnvio { get; set; }
    }
}
