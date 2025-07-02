using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class CursoDto
    {
        public int Id { get; set; }

        [Display(Name = "Nombre del Curso")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Organización")]
        public string NombreOrganizacion { get; set; } = string.Empty;

        [Display(Name = "Tipo de Organización")]
        public string TipoOrganizacion { get; set; } = string.Empty;

        [Display(Name = "Número de Horas")]
        public int NumeroHoras { get; set; }

        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime FechaFinalizacion { get; set; }

        [Display(Name = "Nombre del Docente")]
        public string NombreDocente { get; set; } = string.Empty;

        [Display(Name = "Cédula del Docente")]
        public string DocenteCedula { get; set; } = string.Empty;

        [Display(Name = "Certificado")]
        public string CertificadoRuta { get; set; } = string.Empty;

        [Display(Name = "Hash del Contenido")]
        public string ContenidoHash { get; set; } = string.Empty;

        // Propiedades adicionales para el DTO
        public int OrganizacionId { get; set; }

        public string TipoCurso { get; set; } = string.Empty;
        public bool ImpartidoPorDocente { get; set; }
        public int? HorasImpartidas { get; set; }
    }
} 