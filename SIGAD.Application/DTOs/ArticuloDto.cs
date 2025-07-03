namespace SIGAD.Application.DTOs
{
    public class ArticuloDto
    {
        public string DOI { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; }
        public string IdiomaPublicacion { get; set; } = string.Empty;
        public string ArchivoRuta { get; set; } = string.Empty;
        public string? UrlCloudinary { get; set; }
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombreCompleto { get; set; } = string.Empty;
        public string UnidadVerificadora { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        
        // Información de asociación con solicitudes
        public string? SolicitudId { get; set; }
        public List<SolicitudBasicaDto>? Solicitudes { get; set; }

    }
} 