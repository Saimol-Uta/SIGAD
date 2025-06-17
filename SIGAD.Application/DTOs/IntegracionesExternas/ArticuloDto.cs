namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class ArticuloDto
    {
        public string DOI { get; set; } = default!;
        public string Titulo { get; set; } = default!;
        public string Revista { get; set; } = default!;
        public int AnioPublicacion { get; set; }
        public string ArchivoRuta { get; set; } = default!;
        public string ContenidoHash { get; set; } = default!;
        public string DocenteCedula { get; set; } = default!;
    }
}
