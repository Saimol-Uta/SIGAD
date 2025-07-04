using System;

namespace SIGAD.Application.DTOs
{
    public class SolicitudConApelacionDto
    {
        public Guid Id { get; set; }
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string RangoSolicitadoNombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool TieneApelacion { get; set; }
        public DateTime? FechaApelacion { get; set; }
        public DateTime? FechaLimiteApelacion { get; set; }
        public bool ApelacionVencida { get; set; }
        public int DiasRestantesApelacion { get; set; }
        // Nuevo campo para mostrar el estado real de la apelación (Pendiente, Aceptada, Rechazada)
        public string EstadoApelacion { get; set; } = string.Empty;
    }
}
