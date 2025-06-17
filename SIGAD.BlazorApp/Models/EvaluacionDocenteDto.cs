namespace SIGAD.BlazorApp.Models
{
    public class VerEvaluacionDocenteDto
    {
        public int Id { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public bool TieneInforme => !string.IsNullOrEmpty(InformeRuta);

        public string PuntajeFormateado => $"{PuntajePorcentual:F2}%";
    }
}
