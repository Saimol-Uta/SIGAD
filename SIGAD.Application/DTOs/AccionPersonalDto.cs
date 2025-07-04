using System;
using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    /// <summary>
    /// DTO con los datos necesarios para generar un documento de acción de personal
    /// </summary>
    public class AccionPersonalDto
    {
        /// <summary>
        /// Nombre completo del docente
        /// </summary>
        [Required(ErrorMessage = "El nombre completo del docente es obligatorio")]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// Número de cédula del docente
        /// </summary>
        [Required(ErrorMessage = "La cédula del docente es obligatoria")]
        public string Cedula { get; set; }

        /// <summary>
        /// Categoría o rango anterior del docente
        /// </summary>
        [Required(ErrorMessage = "El rango anterior es obligatorio")]
        public string RangoAnterior { get; set; }

        /// <summary>
        /// Categoría o rango nuevo al que se promueve al docente
        /// </summary>
        [Required(ErrorMessage = "El rango nuevo es obligatorio")]
        public string RangoNuevo { get; set; }

        /// <summary>
        /// Fecha de la sesión del Consejo Universitario donde se aprobó la promoción
        /// </summary>
        [Required(ErrorMessage = "La fecha de sesión es obligatoria")]
        public string FechaSesion { get; set; }

        /// <summary>
        /// Período de convocatoria al que corresponde la promoción
        /// </summary>
        [Required(ErrorMessage = "El período de convocatoria es obligatorio")]
        public string PeriodoConvocatoria { get; set; }

        /// <summary>
        /// Fecha a partir de la cual rige la promoción
        /// </summary>
        [Required(ErrorMessage = "La fecha efectiva de promoción es obligatoria")]
        public string FechaEfectivaPromocion { get; set; }

        /// <summary>
        /// Año del documento (para el número de consecutivo)
        /// </summary>
        [Required(ErrorMessage = "El año es obligatorio")]
        public string Anio { get; set; }

        /// <summary>
        /// Número consecutivo del documento
        /// </summary>
        [Required(ErrorMessage = "El número consecutivo es obligatorio")]
        public string Consecutivo { get; set; }

        /// <summary>
        /// ID de la solicitud de ascenso relacionada (opcional)
        /// </summary>
        public Guid? SolicitudId { get; set; }
    }
} 