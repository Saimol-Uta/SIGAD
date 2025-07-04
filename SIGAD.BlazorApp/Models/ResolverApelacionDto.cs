namespace SIGAD.BlazorApp.Models
{
    public class ResolverApelacionDto
    {
        public bool Aceptada { get; set; }
        public string ObservacionesComision { get; set; } = ""; // Modificado para coincidir con el backend
    }
}
