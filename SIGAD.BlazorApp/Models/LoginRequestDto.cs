using System.ComponentModel.DataAnnotations;

namespace SIGAD.BlazorApp.Models
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Clave { get; set; } = string.Empty;
    }
}
