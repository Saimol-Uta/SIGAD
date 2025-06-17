using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class CrearSolicitudRequestDto
    {
        [Required(ErrorMessage = "El rango solicitado es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del rango solicitado debe ser mayor a 0")]
        public int RangoSolicitadoId { get; set; }
    }
} 