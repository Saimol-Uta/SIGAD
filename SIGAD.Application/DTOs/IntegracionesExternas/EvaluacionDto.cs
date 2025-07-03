namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class EvaluacionDto
    {
        public string PeriodoAcademico { get; set; } = default!;
        public DateTime FechaEvaluacion { get; set; }
        public decimal PuntajePorcentual { get; set; }
        public string InformeRuta { get; set; } = default!;
        public string ContenidoHash { get; set; } = default!;
        public string DocenteCedula { get; set; } = default!;
        
        // Nuevo campo para el PDF en binario desde BD externa
        public byte[]? PdfDocumento { get; set; }
    }
}
