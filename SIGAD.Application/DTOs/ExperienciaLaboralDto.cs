using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class ExperienciaLaboralDto
    {
        public int Id { get; set; }
        public int OrganizacionId { get; set; }
        public string OrganizacionNombre { get; set; } = string.Empty;
        public string OrganizacionTipo { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CertificadoRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
        public decimal AniosExperiencia { get; set; }
    }

    public class CreateExperienciaLaboralDto
    {
        [Required(ErrorMessage = "La organización es requerida")]
        [StringLength(100, ErrorMessage = "El nombre de la organización no puede exceder los 100 caracteres")]
        public string OrganizacionNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula del docente es requerida")]
        [StringLength(10, ErrorMessage = "La cédula debe tener 10 caracteres")]
        public string DocenteCedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El cargo es requerido")]
        [StringLength(100, ErrorMessage = "El cargo no puede exceder los 100 caracteres")]
        public string Cargo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public Guid? SolicitudId { get; set; }
    }

    public class UpdateExperienciaLaboralDto
    {
        [Required(ErrorMessage = "El cargo es requerido")]
        [StringLength(100, ErrorMessage = "El cargo no puede exceder los 100 caracteres")]
        public string Cargo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }
    }

    public class AsociarExperienciaSolicitudDto
    {
        [Required(ErrorMessage = "El ID de la experiencia es requerido")]
        public int ExperienciaId { get; set; }

        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }
    }
} 