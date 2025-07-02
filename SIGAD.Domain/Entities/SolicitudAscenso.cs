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

        // NUEVOS CAMPOS PARA APELACIONES según Artículo 6 del Reglamento UTA
        public DateTime? FechaLimiteApelacion { get; set; } // FechaNotificacion + 3 días
        public bool NotificacionEnviada { get; set; } = false;
        public string? TipoResolucion { get; set; } // "Aprobada", "Rechazada", "ApelacionAceptada", etc.

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

        // NUEVA RELACIÓN PARA APELACIONES según Artículo 6 del Reglamento UTA
        public virtual ICollection<Apelacion> Apelaciones { get; set; } = new List<Apelacion>();



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

        /// <summary>
        /// Notifica el resultado de la solicitud al docente según Artículo 6.5 del Reglamento UTA
        /// </summary>
        public void NotificarResultado()
        {
            if (this.Estado != EstadoSolicitud.Rechazada && this.Estado != EstadoSolicitud.Aprobada)
            {
                throw new InvalidOperationException("Solo se puede notificar el resultado de solicitudes aprobadas o rechazadas.");
            }

            this.FechaNotificacion = DateTime.UtcNow;
            this.FechaLimiteApelacion = DateTime.UtcNow.AddDays(3); // 3 días según reglamento
            this.NotificacionEnviada = true;
            this.TipoResolucion = this.Estado == EstadoSolicitud.Aprobada ? "Aprobada" : "Rechazada";
        }

        /// <summary>
        /// Verifica si la solicitud puede ser apelada según Artículo 6.5 del Reglamento UTA
        /// </summary>
        public bool PuedeApelar()
        {
            return Estado == EstadoSolicitud.Rechazada &&
                   FechaNotificacion.HasValue &&
                   DateTime.UtcNow <= FechaLimiteApelacion &&
                   !Apelaciones.Any(a => a.Estado == EstadoApelacion.Pendiente);
        }

        /// <summary>
        /// Verifica si está en plazo para apelar (dentro de los 3 días)
        /// </summary>
        public bool EstaEnPlazoParaApelar()
        {
            return FechaNotificacion.HasValue &&
                   DateTime.UtcNow <= FechaLimiteApelacion;
        }

        /// <summary>
        /// Crea una apelación para esta solicitud según Artículo 6.6 del Reglamento UTA
        /// </summary>
        public Apelacion CrearApelacion(string motivo, string creadoPor, string? documentosRespaldo = null)
        {
            if (!PuedeApelar())
                throw new InvalidOperationException("No se puede apelar esta solicitud en su estado actual o fuera del plazo.");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("El motivo de la apelación es requerido.");

            var apelacion = new Apelacion(Id, motivo, creadoPor);

            if (!string.IsNullOrEmpty(documentosRespaldo))
                apelacion.DocumentosRespaldo = documentosRespaldo;

            Apelaciones.Add(apelacion);

            // Cambiar estado de la solicitud
            Estado = EstadoSolicitud.EnApelacion;
            TipoResolucion = "EnApelacion";

            return apelacion;
        }

        /// <summary>
        /// Resuelve una apelación según Artículo 6.6 del Reglamento UTA
        /// </summary>
        public void ResolverApelacion(int apelacionId, bool aceptada, string observaciones, string resueltoPor)
        {
            var apelacion = Apelaciones.FirstOrDefault(a => a.Id == apelacionId);
            if (apelacion == null)
                throw new ArgumentException("Apelación no encontrada.");

            if (string.IsNullOrWhiteSpace(observaciones))
                throw new ArgumentException("Se requieren observaciones para resolver la apelación.");

            apelacion.Resolver(aceptada, observaciones, resueltoPor);

            // Actualizar estado de la solicitud según resultado
            if (aceptada)
            {
                Estado = EstadoSolicitud.AprobadaPorApelacion;
                FechaResolucion = DateTime.UtcNow;
                TipoResolucion = "AprobadaPorApelacion";
                ObservacionesAdmin = $"Aprobada por resolución de apelación: {observaciones}";
            }
            else
            {
                Estado = EstadoSolicitud.RechazadaDefinitiva;
                TipoResolucion = "RechazadaDefinitiva";
                ObservacionesAdmin = $"Rechazada definitivamente: {observaciones}";
            }

            FechaResolucionApelacion = DateTime.UtcNow;
        }

        /// <summary>
        /// Marca apelaciones vencidas (más de 3 días sin resolución)
        /// </summary>
        public void MarcarApelacionesVencidas()
        {
            foreach (var apelacion in Apelaciones.Where(a => a.EstaVencida()))
            {
                apelacion.Estado = EstadoApelacion.Vencida;
                apelacion.FechaModificacion = DateTime.UtcNow;
                apelacion.ModificadoPor = "Sistema";
            }
        }
    }
}