using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class UpdateEvaluacionDocenteDto
    {
        [Required(ErrorMessage = "El período académico es requerido")]
        [StringLength(50, ErrorMessage = "El período académico no puede exceder 50 caracteres")]
        public string PeriodoAcademico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de evaluación es requerida")]
        public DateTime FechaEvaluacion { get; set; }

        [Required(ErrorMessage = "El puntaje porcentual es requerido")]
        [Range(0, 100, ErrorMessage = "El puntaje debe estar entre 0 y 100")]
        public decimal PuntajePorcentual { get; set; }
    }
} 