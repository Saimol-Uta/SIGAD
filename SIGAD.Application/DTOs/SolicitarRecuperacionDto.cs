using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class SolicitarRecuperacionDto
    {
        // Se requiere 'EmailAddress' para una validación automática de formato.
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
