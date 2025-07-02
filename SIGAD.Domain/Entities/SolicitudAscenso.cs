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
        public DateTime? FechaNotificacion { get; set; }
        public string? AceptacionODemanda { get; set; }
        public DateTime? FechaResolucionApelacion { get; set; }

        // Estados de aprobación según Reglamento UTA - Artículo 5
        public bool AprobadoPorComision { get; set; } = false;
        public bool AprobadoPorConsejo { get; set; } = false;
        public DateTime? FechaAprobacionComision { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }
        public string? ObservacionesComision { get; set; }
        public string? ObservacionesConsejo { get; set; }

        // Propiedades de navegación
        public virtual Docente Docente { get; set; } = null!;
        public virtual Rango? RangoActual { get; set; }
        public virtual Rango RangoSolicitado { get; set; } = null!;        // Navegación a las tablas de vínculo
        public virtual ICollection<ArticulosPorSolicitud> ArticulosPorSolicitud { get; set; } = new List<ArticulosPorSolicitud>();
        public virtual ICollection<CursosPorSolicitud> CursosPorSolicitud { get; set; } = new List<CursosPorSolicitud>();
        public virtual ICollection<InvestigacionesPorSolicitud> InvestigacionesPorSolicitud { get; set; } = new List<InvestigacionesPorSolicitud>();
        public virtual ICollection<ExperienciaPorSolicitud> ExperienciaPorSolicitud { get; set; } = new List<ExperienciaPorSolicitud>();
        public virtual ICollection<EvaluacionesPorSolicitud> EvaluacionesPorSolicitud { get; set; } = new List<EvaluacionesPorSolicitud>();
        public virtual ICollection<AccionesDePersonalPorSolicitud> AccionesDePersonalPorSolicitud { get; set; } = new List<AccionesDePersonalPorSolicitud>();

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

        public SolicitudAscenso() { }


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
        string? observacionesAdmin = null)
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
    string? observacionesAdmin = null)
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

        /// <summary>
        /// Aprueba la solicitud por parte de la Comisión Académica de Escalafón y Promoción
        /// según el Artículo 5.2.c del Reglamento UTA
        /// </summary>
        public void AprobarPorComision(string? observaciones = null)
        {
            if (this.Estado != EstadoSolicitud.Enviada)
            {
                throw new InvalidOperationException("Solo se puede aprobar por Comisión una solicitud enviada.");
            }

            this.AprobadoPorComision = true;
            this.FechaAprobacionComision = DateTime.UtcNow;
            this.ObservacionesComision = observaciones;
            // No cambiamos el estado aquí, se mantiene como "Enviada" hasta completar todo el proceso
        }

        /// <summary>
        /// Aprueba la solicitud por parte del Honorable Consejo Universitario
        /// según el Artículo 5.1.b del Reglamento UTA
        /// </summary>
        public void AprobarPorConsejo(string? observaciones = null)
        {
            if (!this.AprobadoPorComision)
            {
                throw new InvalidOperationException("La solicitud debe ser aprobada primero por la Comisión según el Reglamento UTA.");
            }

            if (this.Estado != EstadoSolicitud.Enviada)
            {
                throw new InvalidOperationException("La solicitud debe estar en estado 'Enviada' para ser aprobada por el Consejo.");
            }

            this.AprobadoPorConsejo = true;
            this.FechaAprobacionConsejo = DateTime.UtcNow;
            this.ObservacionesConsejo = observaciones;
            // No cambiamos el estado aquí, se mantiene como "Enviada" hasta finalizar el proceso
        }

        /// <summary>
        /// Finaliza el proceso de ascenso una vez aprobado por ambas instancias
        /// según el Artículo 8 del Reglamento UTA (emisión de constancia)
        /// </summary>
        public void FinalizarProceso(string? observacionesFinales = null)
        {
            if (!this.AprobadoPorComision || !this.AprobadoPorConsejo)
            {
                throw new InvalidOperationException("El proceso solo puede finalizarse tras aprobación de Comisión y Consejo Universitario.");
            }

            this.Estado = EstadoSolicitud.Aprobada;
            this.FechaResolucion = DateTime.UtcNow;
            this.ObservacionesAdmin = observacionesFinales;
        }
    }
}