using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SIGAD.Domain.Enums;


namespace SIGAD.Domain.Entities
{
    public class Usuario
    {
    }
        public string Correo { get; set; } = null!;
        public string PrimerNombre { get; set; } = null!;
            public string SegundoNombre { get; set; } = null!;
            public string PrimerApellido { get; set; } = null!;
            public string SegundoApellido { get; set; } = null!;
            public string Clave { get; set; } = null!;
            public RolesUsuario Rol { get; set; }
            public int? NivelTituloDocente { get; set; }

            // Relaciones
            public ICollection<Solicitud>? SolicitudesHechas { get; set; }
            public ICollection<Documento>? Documentos { get; set; }
        }

    }
}
