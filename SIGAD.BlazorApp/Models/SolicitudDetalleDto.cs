namespace SIGAD.BlazorApp.Models
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
        public string RangoSolicitadoNombre { get; set; } = string.Empty;        // La evidencia que se presentó en esta solicitud específica
        public List<VerArticuloDto> ArticulosPresentados { get; set; } = new();
        public List<VerInvestigacionDto> InvestigacionesPresentadas { get; set; } = new();
        public List<VerCursoDto> CursosPresentados { get; set; } = new();
        public List<VerExperienciaLaboralDto> ExperienciasLaborales { get; set; } = new();
        public List<VerEvaluacionDocenteDto> EvaluacionesDocente { get; set; } = new();

        // Estados de aprobación según el reglamento UTA
        public bool AprobadoPorComision { get; set; } = false;
        public bool AprobadoPorConsejo { get; set; } = false;
        public DateTime? FechaAprobacionComision { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }
        public string? ObservacionesComision { get; set; }
        public string? ObservacionesConsejo { get; set; }
    }

    public class VerArticuloDto
    {
        public string DOI { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
    }

    public class VerInvestigacionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string RolEnInvestigacion { get; set; } = string.Empty;
        public int MesesDeInvestigacion { get; set; }
        public string NombreDocente { get; set; } = string.Empty;
        public DateTime FechaFinalizacion { get; set; }
    }

    public class VerCursoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreOrganizacion { get; set; } = string.Empty;
        public int NumeroHoras { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string NombreDocente { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public bool TieneCertificado { get; set; }
    }
}
