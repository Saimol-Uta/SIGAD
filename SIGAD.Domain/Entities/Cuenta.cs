using SIGAD.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Entities
{
    public class Cuenta
    {
        public string Correo { get; set; } // Mapea a: Correo VARCHAR(100) PRIMARY KEY
        public string ClaveHash { get; set; } // Mapea a: ClaveHash VARCHAR(255) NOT NULL
        public string DocenteCedula { get; set; } // Mapea a: DocenteCedula VARCHAR(10) NOT NULL UNIQUE
        public Rol Rol { get; set; } // Mapea a: Rol VARCHAR(20) NOT NULL

        // Propiedad de navegación para la relación uno a uno
        public virtual Docente Docente { get; set; }
    }
}
