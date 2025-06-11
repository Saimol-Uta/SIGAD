using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class VerArticuloDto
    {
        public string DOI { get; set; }
        public string Titulo { get; set; }
        public string Revista { get; set; }
        public int AnioPublicacion { get; set; }
    }
}
