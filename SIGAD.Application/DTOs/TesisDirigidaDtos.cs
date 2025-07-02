namespace SIGAD.Application.DTOs
{
    public class TesisDirigidaDto
    {
        public int Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string NivelAcademico { get; set; } = string.Empty;
        public string TituloTesis { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Institucion { get; set; } = string.Empty;
        public string CertificacionRuta { get; set; } = string.Empty;
        public string? UrlCloudinary { get; set; }
    }

}
