using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Articulo
    {
        public required string DOI { get; set; }
        public required string Titulo { get; set; }
        public required string Revista { get; set; }
        public int AnioPublicacion { get; set; }
        public required string ArchivoRuta { get; set; }
        public required string ContenidoHash { get; set; }
        public required string DocenteCedula { get; set; } // Clave foránea
        public string UnidadVerificadora { get; set; } = string.Empty;
        public bool EsVerificado { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string? ObservacionesVerificacion { get; set; }
        public bool EsIndexado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Propiedades para compatibilidad
        public bool Verificado
        {
            get => EsVerificado;
            set => EsVerificado = value;
        }

        // Propiedad de navegación hacia el Docente dueño del artículo
        public virtual Docente? Docente { get; set; }
        public string? IdiomaPublicacion { get; set; }



    }
}
