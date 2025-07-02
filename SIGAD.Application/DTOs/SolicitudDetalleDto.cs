namespace SIGAD.Application.DTOs
{
    public class SolicitudDetalleDto
    {
        public Guid Id { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string? ObservacionesAdmin { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string? RangoActualNombre { get; set; }
        public string RangoSolicitadoNombre { get; set; } = string.Empty;

        // Campos para seguimiento de aprobación UTA
        public bool AprobadoPorComision { get; set; }
        public bool AprobadoPorConsejo { get; set; }
        public DateTime? FechaAprobacionComision { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }
        public string? ObservacionesComision { get; set; }
        public string? ObservacionesConsejo { get; set; }

        public List<VerTesisDirigidaDto> TesisDirigidas { get; set; } = new();



        // La evidencia que se presentó en esta solicitud específica
        public List<VerArticuloDto> ArticulosPresentados { get; set; } = new();
        public List<VerInvestigacionDto> InvestigacionesPresentadas { get; set; } = new();
        public List<VerCursoDto> CursosPresentados { get; set; } = new();
        public List<VerExperienciaLaboralDto> ExperienciasLaborales { get; set; } = new();
        public List<VerEvaluacionDocenteDto> EvaluacionesDocente { get; set; } = new();
    }
}