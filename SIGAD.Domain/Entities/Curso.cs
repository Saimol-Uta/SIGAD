using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    public class Curso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int OrganizacionId { get; set; }
        public int NumeroHoras { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string CertificadoRuta { get; set; } = string.Empty;
        public string? UrlCloudinary { get; set; }
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;

        public TipoCurso TipoCurso { get; set; } = TipoCurso.ActualizacionCientifica;
        public bool ImpartidoPorDocente { get; set; }

        public int? HorasImpartidas { get; set; }

        // Propiedades de navegación
        public virtual Docente Docente { get; set; } = default!;
        public Organizacion Organizacion { get; set; } = default!;
        
        // Propiedad de navegación hacia las solicitudes que incluyen este curso
        public virtual ICollection<CursosPorSolicitud>? CursosPorSolicitud { get; set; }
    }
}
