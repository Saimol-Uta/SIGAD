using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class VerInvestigacionDto
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Rol en la Investigación")]
        public string RolEnInvestigacion { get; set; } = string.Empty; [Display(Name = "Meses de Investigación")]
        public int MesesDeInvestigacion { get; set; }

        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime FechaFinalizacion { get; set; }

        [Display(Name = "Nombre del Docente")]
        public string NombreDocente { get; set; } = string.Empty;
    }
}
