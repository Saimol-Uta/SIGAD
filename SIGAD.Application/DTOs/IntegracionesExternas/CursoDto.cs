namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class CursoDto
    {
        public string Nombre { get; set; } = default!;
        public string Organizacion { get; set; } = default!;
        public int NumeroHoras { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string CertificadoRuta { get; set; } = default!;
        public string ContenidoHash { get; set; } = default!;
        public string DocenteCedula { get; set; } = default!;
        public string TipoCurso { get; set; } = string.Empty;
        public bool ImpartidoPorDocente { get; set; }
        public int? HorasImpartidas { get; set; } // Nuevo campo


    }
}
