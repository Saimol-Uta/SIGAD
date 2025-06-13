using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class AsociarArticuloSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "El DOI del artículo es requerido")]
        [StringLength(100, ErrorMessage = "El DOI no puede exceder 100 caracteres")]
        public string ArticuloDOI { get; set; } = string.Empty;
    }
} 