using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class EnviarSolicitudDto
    {
        public int RangoSolicitadoId { get; set; }

        // Listas con los IDs de la evidencia que el usuario seleccionó en la UI
        public List<string> ArticulosDOI { get; set; } = new();
        public List<int> CursosId { get; set; } = new();
        public List<int> InvestigacionesId { get; set; } = new();
        public List<int> ExperienciasId { get; set; } = new();
        public List<int> EvaluacionesId { get; set; } = new();
        public List<int> TesisId { get; set; } = new();

    }
}
