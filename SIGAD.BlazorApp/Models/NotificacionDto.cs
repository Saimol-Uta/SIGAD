namespace SIGAD.BlazorApp.Models
{
    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Mensaje { get; set; }
        public string? UrlRedireccion { get; set; }
        public bool EsLeida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TiempoTranscurrido { get; set; }
    }
}
