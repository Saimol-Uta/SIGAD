using System;
using System.Collections.Generic;

namespace SIGAD.Application.DTOs
{
    public class ApelacionDetalleDto
    {
        public int Id { get; set; }
        public Guid SolicitudId { get; set; }
        public string DocenteNombre { get; set; } = "";
        public string DocenteCedula { get; set; } = "";
        public string DocenteEmail { get; set; } = "";
        public string Justificacion { get; set; } = "";
        public List<string> DocumentosAdjuntos { get; set; } = new();
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; } = "";
        public string RangoSolicitado { get; set; } = "";
        public DateTime FechaSolicitud { get; set; }
        public string EstadoSolicitud { get; set; } = "";
        public string? ObservacionesRechazo { get; set; }
    }
}
