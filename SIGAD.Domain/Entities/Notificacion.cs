using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Notificacion
    {
        public int Id { get; set; }

        public string DocenteCedula { get; set; }

        public string Mensaje { get; set; }

        public string? UrlRedireccion { get; set; }

        public bool EsLeida { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaLeida { get; set; }

        // Propiedad de navegación para la relación con Docente
        public virtual Docente Docente { get; set; }
    }
}
