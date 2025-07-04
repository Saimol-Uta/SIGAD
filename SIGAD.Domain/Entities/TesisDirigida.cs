using SIGAD.Domain.Enums; ﻿
namespace SIGAD.Domain.Entities

{
    /// <summary>
    /// Entidad actualizada para tesis dirigidas según requerimientos del Reglamento UTA
    /// Diferencia entre tesis de grado/maestría y tesis DOCTORALES (para rangos principales)
    /// </summary>
    public class TesisDirigida
    {
        public int Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public NivelAcademico NivelAcademico { get; set; }
        public string TituloTesis { get; set; } = string.Empty;
        public EstadoTesis Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Institucion { get; set; } = string.Empty;
        public string CertificacionRuta { get; set; } = string.Empty;
        public string? UrlCloudinary { get; set; }
        public string ContenidoHash { get; set; } = string.Empty;

        public Docente? Docente { get; set; }
        public ICollection<TesisPorSolicitud>? TesisPorSolicitud { get; set; }

        /// <summary>
        /// Verifica si es una tesis doctoral (requerida para rangos principales)
        /// </summary>
        public bool EsTesisDoctorado()
        {
            return NivelAcademico == NivelAcademico.Doctorado;
        }

        /// <summary>
        /// Verifica si la tesis está culminada y puede contar para promoción
        /// </summary>
        public bool PuedeContarParaPromocion()
        {
            return Estado == EstadoTesis.Culminada && FechaFin.HasValue;
        }

        /// <summary>
        /// Calcula los meses de duración de la dirección de tesis
        /// </summary>
        public int GetMesesDireccion()
        {
            if (!FechaFin.HasValue) return 0;

            var duracion = FechaFin.Value - FechaInicio;
            return (int)(duracion.TotalDays / 30.44); // Promedio días por mes
        }

        /// <summary>
        /// Verifica si cumple requisitos para rangos principales según Anexo 1
        /// (debe ser tesis doctoral culminada)
        /// </summary>
        public bool CumpleRequisitoRangoPrincipal()
        {
            return EsTesisDoctorado() && PuedeContarParaPromocion();
        }
    }
}