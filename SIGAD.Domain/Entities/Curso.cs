using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int OrganizacionId { get; set; }
        public int NumeroHoras { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string CertificadoRuta { get; set; }
        public string ContenidoHash { get; set; }
        public string DocenteCedula { get; set; }

        public string TipoCurso { get; set; } = string.Empty;
        public bool ImpartidoPorDocente { get; set; }




        public virtual Docente Docente { get; set; }
        public Organizacion Organizacion { get; set; } = default!;

    }
}
