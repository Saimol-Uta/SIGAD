using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Enums
{
    public enum EstadoSolicitud
    {
        Borrador = 1,
        Enviada = 2,
        EnRevision = 3,
        Aprobada = 4,
        Rechazada = 5,

        /// <summary>
        /// Solicitud en proceso de apelación según Artículo 6 del Reglamento UTA
        /// </summary>
        EnApelacion = 6,

        /// <summary>
        /// Solicitud rechazada definitivamente (después de apelación rechazada)
        /// </summary>
        RechazadaDefinitiva = 7,

        /// <summary>
        /// Solicitud aprobada por resolución de apelación
        /// </summary>
        AprobadaPorApelacion = 8
    }
}
