using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class ActualizarCursoDto
    {
        [Required(ErrorMessage = "El ID del curso es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del curso es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La organización es requerida")]
        [Display(Name = "ID de la Organización")]
        public int OrganizacionId { get; set; }

       
        

        [Required(ErrorMessage = "La fecha de finalización es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFinalizacion { get; set; }

        [Required(ErrorMessage = "La cédula del docente es requerida")]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "La cédula debe tener entre 8 y 10 caracteres")]
        [Display(Name = "Cédula del Docente")]
        public string DocenteCedula { get; set; } = string.Empty;
        [Required(ErrorMessage = "El tipo de curso es requerido")]
        [Display(Name = "Tipo de Curso")]
        public string TipoCurso { get; set; } = string.Empty;
        [Display(Name = "Impartido por el Docente")]
        public bool ImpartidoPorDocente { get; set; } = false;

        [Required(ErrorMessage = "El número de horas es requerido")]
        [Range(1, 1000, ErrorMessage = "El número de horas debe estar entre 1 y 1000")]
        [Display(Name = "Número de Horas")]
        public int NumeroHoras { get; set; }

        [Range(1, 1000, ErrorMessage = "Las horas impartidas deben estar entre 1 y 1000")]
        [Display(Name = "Horas impartidas")]
        public int? HorasImpartidas { get; set; } // Nuevo campo para horas impartidas
        [Display(Name = "Contenido Hash")]
        public string ContenidoHash { get; set; } = string.Empty;
        [Display(Name = "Ruta del Certificado")]
        public string CertificadoRuta { get; set; } = string.Empty;
    }
       
} 