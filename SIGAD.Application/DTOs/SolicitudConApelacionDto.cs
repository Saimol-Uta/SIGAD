using System;

namespace SIGAD.Application.DTOs
{
    public class SolicitudConApelacionDto
    {
        public Guid Id { get; set; }
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string RangoSolicitadoNombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool TieneApelacion { get; set; }
        public int? ApelacionId { get; set; } // Nuevo: ID de la apelación activa o última apelación
        public DateTime? FechaApelacion { get; set; }
        public DateTime? FechaLimiteApelacion { get; set; }
        public bool ApelacionVencida { get; set; }
        public int DiasRestantesApelacion { get; set; }
        public string EstadoApelacion { get; set; } = string.Empty;
    }
}
