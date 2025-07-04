using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class ActualizarArticuloDto
    {
        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La revista es requerida")]
        [StringLength(100, ErrorMessage = "La revista no puede exceder 100 caracteres")]
        public string Revista { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de publicación es requerido")]
        [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100")]
        public int AnioPublicacion { get; set; }

        [Required(ErrorMessage = "El idioma de publicación es requerido")]
        [StringLength(50, ErrorMessage = "El idioma no puede exceder 50 caracteres")]
        public string IdiomaPublicacion { get; set; } = string.Empty;
    }
}