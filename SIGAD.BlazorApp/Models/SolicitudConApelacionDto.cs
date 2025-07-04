namespace SIGAD.BlazorApp.Models
{
    public class SolicitudConApelacionDto
    {
        public Guid Id { get; set; }
        public string DocenteNombreCompleto { get; set; } = "";
        public string DocenteCedula { get; set; } = "";
        public string RangoSolicitadoNombre { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = "";

        // Información de apelación
        public bool TieneApelacion { get; set; }
        public int? ApelacionId { get; set; } // Nuevo: ID de la apelación activa (si existe)
        public DateTime? FechaApelacion { get; set; }
        public DateTime? FechaLimiteApelacion { get; set; }
        public bool ApelacionVencida { get; set; }
        public int DiasRestantesApelacion { get; set; }
        // Nuevo campo para mostrar el estado real de la apelación (Pendiente, Aceptada, Rechazada)
        public string EstadoApelacion { get; set; } = string.Empty;
    }
}
