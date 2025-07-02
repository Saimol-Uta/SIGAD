using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    /// <summary>
    /// Entidad para manejar las apelaciones según Artículo 6 del Reglamento UTA
    /// </summary>
    public class Apelacion
    {
        public int Id { get; set; }

        // Relación con SolicitudAscenso
        public Guid SolicitudAscensoId { get; set; }
        public virtual SolicitudAscenso SolicitudAscenso { get; set; } = null!;

        // Información de la apelación
        public string Motivo { get; set; } = string.Empty; // Razón de la apelación
        public string? DocumentosRespaldo { get; set; } // Lista de documentos adicionales
        public DateTime FechaPresentacion { get; set; }
        public DateTime FechaLimiteRespuesta { get; set; } // FechaPresentacion + 3 días según reglamento

        // Estado y resolución
        public EstadoApelacion Estado { get; set; }
        public string? ObservacionesComision { get; set; }
        public DateTime? FechaResolucion { get; set; }
        public bool Aceptada { get; set; } = false;

        // Auditoría
        public DateTime FechaCreacion { get; set; }
        public string CreadoPor { get; set; } = string.Empty;
        public DateTime? FechaModificacion { get; set; }
        public string? ModificadoPor { get; set; }

        // Constructor por defecto
        public Apelacion()
        {
            FechaCreacion = DateTime.UtcNow;
            Estado = EstadoApelacion.Pendiente;
        }

        // Constructor con parámetros
        public Apelacion(Guid solicitudAscensoId, string motivo, string creadoPor) : this()
        {
            SolicitudAscensoId = solicitudAscensoId;
            Motivo = motivo;
            CreadoPor = creadoPor;
            FechaPresentacion = DateTime.UtcNow;
            FechaLimiteRespuesta = DateTime.UtcNow.AddDays(3); // 3 días según reglamento Art. 6
        }

        /// <summary>
        /// Resuelve la apelación según Artículo 6.6 del Reglamento UTA
        /// </summary>
        public void Resolver(bool aceptada, string observaciones, string resueltoPor)
        {
            if (Estado != EstadoApelacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden resolver apelaciones pendientes.");

            if (string.IsNullOrWhiteSpace(observaciones))
                throw new ArgumentException("Se requieren observaciones para resolver la apelación.");

            Aceptada = aceptada;
            ObservacionesComision = observaciones;
            Estado = aceptada ? EstadoApelacion.Aceptada : EstadoApelacion.Rechazada;
            FechaResolucion = DateTime.UtcNow;
            FechaModificacion = DateTime.UtcNow;
            ModificadoPor = resueltoPor;
        }

        /// <summary>
        /// Verifica si la apelación está vencida (más de 3 días sin resolución)
        /// </summary>
        public bool EstaVencida()
        {
            return DateTime.UtcNow > FechaLimiteRespuesta && Estado == EstadoApelacion.Pendiente;
        }

        /// <summary>
        /// Agrega documentos de respaldo a la apelación
        /// </summary>
        public void AgregarDocumentoRespaldo(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("El documento no puede estar vacío.");

            if (string.IsNullOrEmpty(DocumentosRespaldo))
                DocumentosRespaldo = documento;
            else
                DocumentosRespaldo += $";{documento}";

            FechaModificacion = DateTime.UtcNow;
        }
    }
}
