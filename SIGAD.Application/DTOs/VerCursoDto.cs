using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class VerCursoDto
    {
        public int Id { get; set; }

        [Display(Name = "Curso")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Organización")]
        public string NombreOrganizacion { get; set; } = string.Empty;

        [Display(Name = "Horas")]
        public int NumeroHoras { get; set; }

        [Display(Name = "Fecha Finalización")]
        [DataType(DataType.Date)]
        public DateTime FechaFinalizacion { get; set; }

        [Display(Name = "Docente")]
        public string NombreDocente { get; set; } = string.Empty;

        [Display(Name = "Cédula")]
        public string DocenteCedula { get; set; } = string.Empty;

        [Display(Name = "Archivo")]
        public bool TieneCertificado { get; set; }
    }
} 