using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class CrearCursoDto
    {
        [Required(ErrorMessage = "El nombre del curso es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La organización es requerida")]
        [StringLength(100, ErrorMessage = "El nombre de la organización no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre de la Organización")]
        public string OrganizacionNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de horas es requerido")]
        [Range(1, 1000, ErrorMessage = "El número de horas debe estar entre 1 y 1000")]
        [Display(Name = "Número de Horas")]
        public int NumeroHoras { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFinalizacion { get; set; }

        [Required(ErrorMessage = "La cédula del docente es requerida")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "La cédula debe tener entre 8 y 10 caracteres")]
        [Display(Name = "Cédula del Docente")]
        public string DocenteCedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        [Display(Name = "ID de la Solicitud")]
        public Guid SolicitudId { get; set; }

        /// <summary>
        /// Número de horas de capacitación impartidas por el docente (opcional, para validación de rangos principales)
        /// </summary>
        public int? HorasImpartidas { get; set; }
    }
}