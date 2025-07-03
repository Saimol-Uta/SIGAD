using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class ActualizarInvestigacionDto
    {
        public string DocenteCedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres")]
        [Display(Name = "Título de la Investigación")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFinalizacion { get; set; }

        [Required(ErrorMessage = "El rol en la investigación es requerido")]
        [StringLength(50, ErrorMessage = "El rol no puede exceder los 50 caracteres")]
        [Display(Name = "Rol en la Investigación")]
        public string RolEnInvestigacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los meses de investigación son requeridos")]
        [Range(1, 120, ErrorMessage = "Los meses deben estar entre 1 y 120")]
        [Display(Name = "Meses de Investigación")]
        public int MesesDeInvestigacion { get; set; }

        /// <summary>
        /// Indica si la investigación es internacional (opcional, para validación de rangos principales)
        /// </summary>
        public bool EsInternacional { get; set; } = false;
    }
}