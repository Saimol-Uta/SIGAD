namespace SIGAD.Application.DTOs
{
    public class EvaluacionDocenteDto
    {
        public int Id { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
    }
} 