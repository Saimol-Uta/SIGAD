using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class ActualizarRangoDto
    {
        [Required(ErrorMessage = "El Nombre es requerido.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Range(0, 100)]
        public int ArticulosRequeridos { get; set; }

        [Range(0, 50)]
        public int AniosExperienciaRequeridos { get; set; }

        [Range(0, 1000)]
        public int HorasCursoRequeridas { get; set; }

        [Range(0, 120)]
        public int MesesInvestigacionRequeridos { get; set; }

        [Range(0, 100.00)]
        public decimal PuntajePromedioEvaluacionesRequerido { get; set; }
    }
}
