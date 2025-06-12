using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGAD.Domain.Entities
{
    [Table("SolicitudesAscenso")]
    public class SolicitudAscenso
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(10)]
        public string DocenteCedula { get; set; } = string.Empty;

        public int? RangoActualId { get; set; }

        [Required]
        public int RangoSolicitadoId { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public DateTime? FechaResolucion { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = string.Empty;

        public string? ObservacionesAdmin { get; set; }

        // Propiedades de navegación
        [ForeignKey("DocenteCedula")]
        public virtual Docente? Docente { get; set; }

        public virtual ICollection<EvaluacionPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionPorSolicitud>();
    }
} 