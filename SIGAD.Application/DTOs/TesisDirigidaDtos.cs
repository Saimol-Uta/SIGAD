namespace SIGAD.Application.DTOs
{
    public class TesisDirigidaDto
    {
        public int Id { get; set; }
        public string DocenteCedula { get; set; }
        public string NivelAcademico { get; set; }
        public string TituloTesis { get; set; }
        public string Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Institucion { get; set; }
        public string CertificacionRuta { get; set; }
    }

}
