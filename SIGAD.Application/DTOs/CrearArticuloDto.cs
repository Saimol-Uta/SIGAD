using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class CrearArticuloDto
    {
        [Required(ErrorMessage = "El DOI es requerido")]
        [StringLength(100, ErrorMessage = "El DOI no puede exceder 100 caracteres")]
        public string DOI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La revista es requerida")]
        [StringLength(100, ErrorMessage = "La revista no puede exceder 100 caracteres")]
        public string Revista { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de publicación es requerido")]
        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        public int AnioPublicacion { get; set; }

        [Required(ErrorMessage = "La cédula del docente es requerida")]
        [StringLength(10, ErrorMessage = "La cédula no puede exceder 10 caracteres")]
        public string DocenteCedula { get; set; } = string.Empty;

        public Guid? SolicitudId { get; set; }

        /// <summary>
        /// Idioma de publicación del artículo (opcional, para validación de rangos principales)
        /// </summary>
        public string? IdiomaPublicacion { get; set; }
    }
}
