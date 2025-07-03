namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class ExperienciaDto
    {
        public string Organizacion { get; set; } = default!;
        public string Cargo { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string CertificadoRuta { get; set; } = default!;
        public string ContenidoHash { get; set; } = default!;
        public string DocenteCedula { get; set; } = default!;
        
        // Nuevo campo para el PDF en binario desde BD externa
        public byte[]? PdfDocumento { get; set; }
    }
}
