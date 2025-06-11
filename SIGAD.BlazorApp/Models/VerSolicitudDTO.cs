namespace SIGAD.BlazorApp.Models
{
    public class VerSolicitudDTO
    {
            public Guid Id { get; set; }
            public string NombreDocente { get; set; }
            public string RangoActual { get; set; }
            public string RangoSolicitado { get; set; }
            public string Estado { get; set; }
            public DateTime FechaCreacion { get; set; }
        }
}
