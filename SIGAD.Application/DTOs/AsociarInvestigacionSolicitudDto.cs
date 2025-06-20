using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class AsociarInvestigacionSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "El ID de la investigación es requerido")]
        public int InvestigacionId { get; set; }
    }
} 