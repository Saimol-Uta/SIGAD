using SIGAD.Domain.Enums; ﻿
namespace SIGAD.Domain.Entities

{
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
    }

    public class TesisPorSolicitud
    {
        public Guid SolicitudId { get; set; }
        public SolicitudAscenso? Solicitud { get; set; }

        public int TesisDirigidaId { get; set; }
        public TesisDirigida? TesisDirigida { get; set; }
    }
}