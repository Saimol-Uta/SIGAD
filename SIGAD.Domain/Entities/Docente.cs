using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIGAD.Domain.Entities
{
    [Table("Docentes")]
    public class Docente
    {
        [Key]
        [StringLength(10)]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Nombre1 { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Nombre2 { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido1 { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Apellido2 { get; set; } = string.Empty;

        // Propiedades de navegación
        public virtual Cuenta? Cuenta { get; set; }
        public virtual ICollection<EvaluacionDocente> Evaluaciones { get; set; } = new List<EvaluacionDocente>();
        public virtual ICollection<SolicitudAscenso> Solicitudes { get; set; } = new List<SolicitudAscenso>();
    }
} 