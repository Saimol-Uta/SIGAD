using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGAD.Domain.Entities
{
    [Table("EvaluacionesPorSolicitud")]
    public class EvaluacionPorSolicitud
    {
        [Required]
        public Guid SolicitudId { get; set; }

        [Required]
        public int EvaluacionId { get; set; }

        // Propiedades de navegación
        [ForeignKey("SolicitudId")]
        public virtual SolicitudAscenso? Solicitud { get; set; }

        [ForeignKey("EvaluacionId")]
        public virtual EvaluacionDocente? Evaluacion { get; set; }
    }
} 