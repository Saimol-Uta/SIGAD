namespace SIGAD.BlazorApp.Models
{
    public class RangoDto
    {
        public Guid Id { get; set; }
        public string? Nombre { get; set; } // Usar string? para C# moderno con nulabilidad habilitada
        public string? Descripcion { get; set; }
    }
}
