using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class CrearSolicitudDto
    {
        // Para el MVP, solo necesitamos saber qué rango se solicita.
        // Asumiremos que sabemos qué docente lo pide por el login.
        public int RangoSolicitadoId { get; set; }

        // Podríamos añadir una lista de IDs de la evidencia seleccionada
        // Ejemplo: public List<string> ArticulosDOI { get; set; } = new();
    }
}
