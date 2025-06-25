using System.ComponentModel.DataAnnotations;

namespace SIGAD.BlazorApp.Models
{
    public class ReporteDataDto
    {
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}