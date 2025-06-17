using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class RestablecerContrasenaDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string Codigo { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string NuevaContrasena { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
