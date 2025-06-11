using SIGAD.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionArticulosAppService
    {
        public GestionArticulosAppService(/* dependencias como IArticuloRepository */)
        {
        }

        public async Task CrearArticuloAsync(CrearArticuloDto dto, string docenteCedula)
        {
            // TAREA para el Equipo Backend B
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<VerArticuloDto>> GetArticulosPorDocenteAsync(string docenteCedula)
        {
            // TAREA para el Equipo Backend B
            await Task.CompletedTask;
            return new List<VerArticuloDto>();
        }
    }
}
