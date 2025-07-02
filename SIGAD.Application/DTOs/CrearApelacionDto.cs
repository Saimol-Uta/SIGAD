using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SIGAD.Application.DTOs
{
    public class CrearApelacionDto
    {
        [Required]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "El motivo de apelación es obligatorio.")]
        public string Motivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe proporcionar el enlace al documento.")]
        [Url(ErrorMessage = "Debe ser un enlace válido.")]
        public string DocumentoUrl { get; set; } = string.Empty;
    }


}
