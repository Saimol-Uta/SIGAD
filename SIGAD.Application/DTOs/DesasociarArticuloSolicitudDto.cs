using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class DesasociarArticuloSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }
    }
}
