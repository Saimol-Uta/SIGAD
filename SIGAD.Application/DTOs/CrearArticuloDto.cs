using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class CrearArticuloDto
    {
        public string DOI { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public int AnioPublicacion { get; set; } 
        public string ArchivoRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
    }
}
