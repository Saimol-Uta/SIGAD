using SIGAD.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionInvestigacionesAppService
    {
        public GestionInvestigacionesAppService(/* dependencias */) { }

        public async Task<int> CrearInvestigacionAsync(CrearInvestigacionDto dto, string docenteCedula)
        {
            // TAREA para el Equipo Backend B
            await Task.CompletedTask;
            return 1; // Devuelve un Id de prueba
        }

        public async Task<IEnumerable<VerInvestigacionDto>> GetInvestigacionesPorDocenteAsync(string docenteCedula)
        {
            // TAREA para el Equipo Backend B
            await Task.CompletedTask;
            return new List<VerInvestigacionDto>();
        }
    }
}
