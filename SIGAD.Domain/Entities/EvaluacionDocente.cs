using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGAD.Domain.Entities
{
    [Table("EvaluacionesDocentes")]
    public class EvaluacionDocente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string PeriodoAcademico { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEvaluacion { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PuntajePorcentual { get; set; }

        [Required]
        public string InformeRuta { get; set; } = string.Empty;

        [Required]
        [StringLength(64)]
        public string ContenidoHash { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string DocenteCedula { get; set; } = string.Empty;

        // Propiedades de navegación
        [ForeignKey("DocenteCedula")]
        public virtual Docente? Docente { get; set; }

        public virtual ICollection<EvaluacionPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionPorSolicitud>();
    }
} 