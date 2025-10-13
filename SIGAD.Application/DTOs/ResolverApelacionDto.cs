using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class ResolverApelacionDto
    {
        [Required(ErrorMessage = "Debe especificar si acepta o rechaza la apelación")]
        public bool Aceptada { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
        public string? ObservacionesComision { get; set; }
    }
}
