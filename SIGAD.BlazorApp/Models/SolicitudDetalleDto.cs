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
        public string RangoSolicitadoNombre { get; set; } = string.Empty;        
         public List<VerTesisDirigidaDto> TesisDirigidas { get; set; } = new();
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
     // DTO para la tabla de Tesis Dirigidas en la UI
    public class VerTesisDirigidaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty; // Cambiado de TituloTesis a Titulo
        public string Nivel { get; set; } = string.Empty; // Cambiado de NivelAcademico a Nivel
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CertificacionPath { get; set; } = string.Empty; // Cambiado de CertificacionRuta
        public string Institucion { get; set; } = string.Empty; // Mapea a Institucion de la entidad original
        
        // Propiedades calculadas para compatibilidad
        public string TituloTesis => Titulo; // Para compatibilidad con vista existente
        public string NivelAcademico => Nivel; // Para compatibilidad con vista existente
        public string CertificacionRuta => CertificacionPath; // Para compatibilidad con vista existente
        public bool TieneCertificacion => !string.IsNullOrEmpty(CertificacionPath);
        
        public string PeriodoFormateado => FechaInicio != default && FechaFin.HasValue
            ? $"{FechaInicio:MM/yyyy} - {FechaFin.Value:MM/yyyy}"
            : FechaInicio != default ? FechaInicio.ToString("MM/yyyy") : "Sin fecha";
    }

    public class VerArticuloDto
    {
        public string DOI { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string ArchivoRuta { get; set; } = string.Empty;
    }

    public class VerInvestigacionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string RolEnInvestigacion { get; set; } = string.Empty;
        public int MesesDeInvestigacion { get; set; }
        public string NombreDocente { get; set; } = string.Empty;
        public DateTime FechaFinalizacion { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
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
        public string CertificadoRuta { get; set; } = string.Empty;
    }
}
