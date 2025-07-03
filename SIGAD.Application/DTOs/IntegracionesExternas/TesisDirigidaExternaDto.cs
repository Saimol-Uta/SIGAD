namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class TesisDirigidaExternaDto
    {
        public string DocenteCedula { get; set; } = string.Empty;
        public string NivelAcademico { get; set; } = string.Empty;
        public string TituloTesis { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Institucion { get; set; } = string.Empty;
        public string CertificacionRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
        
        // Nuevo campo para el PDF en binario desde BD externa
        public byte[]? PdfDocumento { get; set; }
    }
}
