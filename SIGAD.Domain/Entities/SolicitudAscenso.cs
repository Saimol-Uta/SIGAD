using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    public class SolicitudAscenso
    {
        public Guid Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public int? RangoActualId { get; set; }
        public int RangoSolicitadoId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public string? ObservacionesAdmin { get; set; }

        // Propiedades de navegación
        public virtual Docente Docente { get; set; } = null!;
        public virtual Rango? RangoActual { get; set; }
        public virtual Rango RangoSolicitado { get; set; } = null!;

        // Navegación a las tablas de vínculo
        public virtual ICollection<ArticulosPorSolicitud> ArticulosPorSolicitud { get; set; } = new List<ArticulosPorSolicitud>();
        public virtual ICollection<CursosPorSolicitud> CursosPorSolicitud { get; set; } = new List<CursosPorSolicitud>();
        public virtual ICollection<InvestigacionesPorSolicitud> InvestigacionesPorSolicitud { get; set; } = new List<InvestigacionesPorSolicitud>();
        public virtual ICollection<ExperienciaPorSolicitud> ExperienciaPorSolicitud { get; set; } = new List<ExperienciaPorSolicitud>();
        public virtual ICollection<EvaluacionPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionPorSolicitud>();
    }
} 