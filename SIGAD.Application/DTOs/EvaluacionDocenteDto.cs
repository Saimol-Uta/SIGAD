namespace SIGAD.Application.DTOs
{
    public class EvaluacionDocenteDto
    {
        public int Id { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string? UrlCloudinary { get; set; }
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        
        // Información de asociación con solicitudes
        public string? SolicitudId { get; set; }
        public List<SolicitudBasicaDto>? Solicitudes { get; set; }
    }
    
    public class SolicitudBasicaDto
    {
        public string SolicitudId { get; set; } = string.Empty;
        public string? Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
} 