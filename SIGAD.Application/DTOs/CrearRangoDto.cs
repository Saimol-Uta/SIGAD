using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class CrearRangoDto
    {
        [Required(ErrorMessage = "El Id es requerido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El Nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El Nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "El valor debe estar entre 0 y 100.")]
        public int ArticulosRequeridos { get; set; }

        [Range(0, 50, ErrorMessage = "El valor debe estar entre 0 y 50.")]
        public int AniosExperienciaRequeridos { get; set; }

        [Range(0, 1000, ErrorMessage = "El valor debe estar entre 0 y 1000.")]
        public int HorasCursoRequeridas { get; set; }

        [Range(0, 120, ErrorMessage = "El valor debe estar entre 0 y 120.")]
        public int MesesInvestigacionRequeridos { get; set; }

        [Range(0, 100.00, ErrorMessage = "El valor debe estar entre 0 y 100.00.")]
        public decimal PuntajePromedioEvaluacionesRequerido { get; set; }
    }
}
