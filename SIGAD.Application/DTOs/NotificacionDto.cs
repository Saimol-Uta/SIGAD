using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
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
