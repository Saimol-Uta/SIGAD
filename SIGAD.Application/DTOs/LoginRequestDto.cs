using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Clave { get; set; } = string.Empty;
    }
}