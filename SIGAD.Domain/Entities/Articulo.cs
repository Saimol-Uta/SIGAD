using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Articulo
    {
        public string DOI { get; set; } 
        public string Titulo { get; set; }
        public string Revista { get; set; }
        public int AnioPublicacion { get; set; }
        public string ArchivoRuta { get; set; }
        public string ContenidoHash { get; set; }
        public string DocenteCedula { get; set; } // Clave foránea
        public string UnidadVerificadora { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public DateTime? FechaVerificacion { get; set; }

        // Propiedad de navegación hacia el Docente dueño del artículo
        public virtual Docente Docente { get; set; }
    }
}
