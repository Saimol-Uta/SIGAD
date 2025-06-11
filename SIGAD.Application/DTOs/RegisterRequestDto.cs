using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Clave { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es requerida")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "La cédula debe tener entre 8 y 10 caracteres")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer nombre es requerido")]
        [StringLength(50, ErrorMessage = "El primer nombre no puede exceder 50 caracteres")]
        public string Nombre1 { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El segundo nombre no puede exceder 50 caracteres")]
        public string? Nombre2 { get; set; }

        [Required(ErrorMessage = "El primer apellido es requerido")]
        [StringLength(50, ErrorMessage = "El primer apellido no puede exceder 50 caracteres")]
        public string Apellido1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "El segundo apellido es requerido")]
        [StringLength(50, ErrorMessage = "El segundo apellido no puede exceder 50 caracteres")]
        public string Apellido2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido")]
        [RegularExpression("^(ADMINISTRADOR|DOCENTE)$", ErrorMessage = "El rol debe ser ADMINISTRADOR o DOCENTE")]
        public string Rol { get; set; } = string.Empty;
    }
}