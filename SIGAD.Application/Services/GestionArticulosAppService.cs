using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionArticulosAppService
    {
        private readonly IArticuloRepository _articuloRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GestionArticulosAppService(IArticuloRepository articuloRepository, IUnitOfWork unitOfWork)
        {
            _articuloRepository = articuloRepository;
            _unitOfWork = unitOfWork;
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
