// En: SIGAD.Application/DTOs/VerSolicitudDto.cs
namespace SIGAD.Application.DTOs
{
    public class VerSolicitudDto
    {
        public Guid Id { get; set; }
        public string DocenteNombreCompleto { get; set; } // <-- FALTABA
        public string RangoSolicitadoNombre { get; set; } // <-- FALTABA
        public string Estado { get; set; }
        public DateTime? FechaEnvio { get; set; } // <-- FALTABA (y debe permitir nulos)
    }
}