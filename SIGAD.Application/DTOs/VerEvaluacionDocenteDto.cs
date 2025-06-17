using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class VerEvaluacionDocenteDto
    {
        public int Id { get; set; }

        [Display(Name = "Período Académico")]
        public string PeriodoAcademico { get; set; } = string.Empty;

        [Display(Name = "Fecha de Evaluación")]
        [DataType(DataType.Date)]
        public DateTime FechaEvaluacion { get; set; }

        [Display(Name = "Puntaje Porcentual")]
        public decimal PuntajePorcentual { get; set; }

        [Display(Name = "Informe")]
        public string InformeRuta { get; set; } = string.Empty;

        public bool TieneInforme => !string.IsNullOrEmpty(InformeRuta);

        public string PuntajeFormateado => $"{PuntajePorcentual:F2}%";
    }
}
