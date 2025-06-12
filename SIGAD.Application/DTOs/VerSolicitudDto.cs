using System;

namespace SIGAD.Application.DTOs
{
    public class VerSolicitudDto
    {
        public Guid Id { get; set; }

        // Se inicializan con string.Empty para cumplir con la regla de no-nulabilidad
        public string NombreDocente { get; set; } = string.Empty;
        public string RangoActual { get; set; } = string.Empty;
        public string RangoSolicitado { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }
    }
}