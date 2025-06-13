using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class InvestigacionDto
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de Finalización")]
        [DataType(DataType.Date)]
        public DateTime FechaFinalizacion { get; set; }

        [Display(Name = "Rol en la Investigación")]
        public string RolEnInvestigacion { get; set; } = string.Empty;

        [Display(Name = "Meses de Investigación")]
        public int MesesDeInvestigacion { get; set; }

        [Display(Name = "Nombre del Docente")]
        public string NombreDocente { get; set; } = string.Empty;

        [Display(Name = "Cédula del Docente")]
        public string DocenteCedula { get; set; } = string.Empty;

        [Display(Name = "Informe")]
        public string InformeRuta { get; set; } = string.Empty;

        [Display(Name = "Hash del Contenido")]
        public string ContenidoHash { get; set; } = string.Empty;
    }
} 