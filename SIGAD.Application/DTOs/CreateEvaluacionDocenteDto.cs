using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class CreateEvaluacionDocenteDto
    {
        [Required(ErrorMessage = "El período académico es requerido")]
        [StringLength(50, ErrorMessage = "El período académico no puede exceder 50 caracteres")]
        public string PeriodoAcademico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de evaluación es requerida")]
        public DateTime FechaEvaluacion { get; set; }

        [Required(ErrorMessage = "El puntaje porcentual es requerido")]
        [Range(0.00, 100.00, ErrorMessage = "El puntaje debe estar entre 0 y 100")]
        public decimal PuntajePorcentual { get; set; }

        [Required(ErrorMessage = "La cédula del docente es requerida")]
        [StringLength(10, ErrorMessage = "La cédula no puede exceder 10 caracteres")]
        public string DocenteCedula { get; set; } = string.Empty;

        public Guid? SolicitudId { get; set; }
    }
} 