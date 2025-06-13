using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class AsociarCursoSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        [Display(Name = "ID de la Solicitud")]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "El ID del curso es requerido")]
        [Display(Name = "ID del Curso")]
        public int CursoId { get; set; }
    }
} 