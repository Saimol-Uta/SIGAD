using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class AsociarTesisSolicitudDto
    {
        [Required]
        public Guid SolicitudId { get; set; }
        
        [Required]
        public int TesisId { get; set; }
    }
}
