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
        public virtual ICollection<EvaluacionesPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionesPorSolicitud>();

        public ICollection<TesisPorSolicitud> TesisPorSolicitud { get; set; } = new List<TesisPorSolicitud>();



        public void Aprobar(string? observaciones)
        {
            // Regla de negocio: Solo se puede aprobar si está en estado 'Enviada' o 'EnRevision'
            if (this.Estado != EstadoSolicitud.Enviada && this.Estado != EstadoSolicitud.EnRevision)
            {
                throw new InvalidOperationException("Solo se puede aprobar una solicitud que ha sido enviada o está en revisión.");
            }

            this.Estado = EstadoSolicitud.Aprobada;
            this.FechaResolucion = DateTime.UtcNow;
            this.ObservacionesAdmin = observaciones;
        }

        private SolicitudAscenso() { }


        public SolicitudAscenso(string docenteCedula, int rangoSolicitadoId, int? rangoActualId)
        {
            Id = Guid.NewGuid();
            DocenteCedula = docenteCedula;
            RangoSolicitadoId = rangoSolicitadoId;
            RangoActualId = rangoActualId;
            FechaCreacion = DateTime.UtcNow;
            Estado = EstadoSolicitud.Enviada; // O 'Borrador', según tu lógica inicial
            FechaEnvio = DateTime.UtcNow;
        }

        public SolicitudAscenso(
        string docenteCedula,
        int rangoActualId,
        int rangoSolicitadoId,
        string observacionesAdmin = null)
        {
            Id = Guid.NewGuid();
            DocenteCedula = docenteCedula;
            RangoActualId = rangoActualId;
            RangoSolicitadoId = rangoSolicitadoId;
            FechaCreacion = DateTime.UtcNow;
            FechaEnvio = null;
            FechaResolucion = null;
            Estado = EstadoSolicitud.Borrador;
            ObservacionesAdmin = observacionesAdmin;
        }

        public SolicitudAscenso(
    string docenteCedula,
    int? rangoActualId,
    int rangoSolicitadoId,
    DateTime fechaCreacion,
    DateTime? fechaEnvio,
    DateTime? fechaResolucion,
    SIGAD.Domain.Enums.EstadoSolicitud estado,
    string observacionesAdmin = null)
        {
            Id = Guid.NewGuid();
            DocenteCedula = docenteCedula;
            RangoActualId = rangoActualId;
            RangoSolicitadoId = rangoSolicitadoId;
            FechaCreacion = fechaCreacion;
            FechaEnvio = fechaEnvio;
            FechaResolucion = fechaResolucion;
            Estado = estado;
            ObservacionesAdmin = observacionesAdmin;
        }

        public void Rechazar(string? observaciones)
        {
            if (this.Estado != EstadoSolicitud.Enviada && this.Estado != EstadoSolicitud.EnRevision)
            {
                throw new InvalidOperationException("Solo se puede rechazar una solicitud que ha sido enviada o está en revisión.");
            }

            if (string.IsNullOrWhiteSpace(observaciones))
            {
                throw new ArgumentException("Se requiere una observación para rechazar la solicitud.");
            }

            this.Estado = EstadoSolicitud.Rechazada;
            this.FechaResolucion = DateTime.UtcNow;
            this.ObservacionesAdmin = observaciones;
        }

    }
}