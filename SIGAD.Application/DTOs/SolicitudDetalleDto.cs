namespace SIGAD.Application.DTOs
{
    public class SolicitudDetalleDto
    {
        public Guid Id { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? ObservacionesAdmin { get; set; }
        public string DocenteCedula { get; set; }
        public string DocenteNombreCompleto { get; set; }
        public string? RangoActualNombre { get; set; }
        public string RangoSolicitadoNombre { get; set; }

        // La evidencia que se presentó en esta solicitud específica
        public List<VerArticuloDto> ArticulosPresentados { get; set; } = new();
        public List<VerInvestigacionDto> InvestigacionesPresentadas { get; set; } = new();
        public List<VerCursoDto> CursosPresentados { get; set; } = new(); // Necesitarás crear VerCursoDto, etc.
        // ... y así para las otras evidencias ...
    }
}