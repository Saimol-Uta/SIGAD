using System.ComponentModel.DataAnnotations;

namespace SIGAD.BlazorApp.Models
{
    public class ApelacionDto
    {
        public Guid Id { get; set; }
        public Guid SolicitudId { get; set; }
        public string Justificacion { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public string EstadoApelacion { get; set; } = "";
        public string? ObservacionesResolucion { get; set; }
        public string? DocumentoRuta { get; set; }
        public string DocenteCedula { get; set; } = "";
        public string DocenteNombre { get; set; } = "";
    }

    public class CrearApelacionDto
    {
        [Required(ErrorMessage = "El ID de la solicitud es requerido")]
        public Guid SolicitudId { get; set; }

        [Required(ErrorMessage = "La justificación es requerida")]
        [StringLength(2000, ErrorMessage = "La justificación no puede exceder 2000 caracteres")]
        public string Justificacion { get; set; } = "";
    }
}
