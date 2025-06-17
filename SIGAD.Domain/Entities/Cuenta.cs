using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIGAD.Domain.Enums;
using System;

namespace SIGAD.Domain.Entities
{
    [Table("Cuentas")]
    public class Cuenta
    {
        [Key]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string ClaveHash { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string DocenteCedula { get; set; } = string.Empty;

        [Required]
        public Rol Rol { get; set; }

        // Propiedades de navegación
        [ForeignKey("DocenteCedula")]
        public virtual Docente? Docente { get; set; }

        public string? CodigoRecuperacion { get; set; }
        public DateTime? CodigoExpiracion { get; set; }
    }
} 