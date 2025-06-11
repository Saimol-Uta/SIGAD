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
    public class ActualizarRangoService
    {
        private readonly IRangoRepository _rangoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarRangoService(IRangoRepository rangoRepository, IUnitOfWork unitOfWork)
        {
            _rangoRepository = rangoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ActualizarRangoAsync(int id, ActualizarRangoDto rangoDto)
        {
            // 1. Obtener la entidad existente de la base de datos
            var rangoExistente = await _rangoRepository.GetByIdAsync(id);
            if (rangoExistente == null)
            {
                // Lanza una excepción si el rango a actualizar no se encuentra
                throw new KeyNotFoundException($"No se encontró un rango con el Id {id}.");
            }

            // 2. Usar el método del dominio para actualizar la entidad
            rangoExistente.ActualizarRequisitos(
                rangoDto.Nombre,
                rangoDto.ArticulosRequeridos,
                rangoDto.AniosExperienciaRequeridos,
                rangoDto.HorasCursoRequeridas,
                rangoDto.MesesInvestigacionRequeridos,
                rangoDto.PuntajePromedioEvaluacionesRequerido
            );

            // 3. Usar el repositorio para marcar la entidad como actualizada
            await _rangoRepository.UpdateAsync(rangoExistente);

            // 4. Usar la Unidad de Trabajo para confirmar los cambios en la BD
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
