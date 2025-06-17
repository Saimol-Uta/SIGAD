using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class AsociarEvaluacionSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "El ID de la evaluación es requerido")]
        public int EvaluacionId { get; set; }
    }
} 