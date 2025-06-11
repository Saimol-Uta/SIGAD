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
    public class GestionRangoAppService
    {
        private readonly IRangoRepository _rangoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GestionRangoAppService(IRangoRepository rangoRepository, IUnitOfWork unitOfWork)
        {
            _rangoRepository = rangoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Rango> CrearRangoAsync(CrearRangoDto rangoDto)
        {
            // Aquí podrías añadir validaciones de negocio, ej: verificar que el Id no exista.
            var rangoExistente = await _rangoRepository.GetByIdAsync(rangoDto.Id);

            if (rangoExistente != null)
            {
                throw new InvalidOperationException("Ya existe un rango con este Id.");
            }

            // Mapear el DTO a la Entidad del Dominio
            var nuevoRango = new Rango(
                rangoDto.Id,
                rangoDto.Nombre

            );

            // Asignar los valores de los requisitos
            nuevoRango.ArticulosRequeridos = rangoDto.ArticulosRequeridos;
            nuevoRango.AniosExperienciaRequeridos = rangoDto.AniosExperienciaRequeridos;
            nuevoRango.HorasCursoRequeridas = rangoDto.HorasCursoRequeridas;
            nuevoRango.MesesInvestigacionRequeridos = rangoDto.MesesInvestigacionRequeridos;
            nuevoRango.PuntajePromedioEvaluacionesRequerido = rangoDto.PuntajePromedioEvaluacionesRequerido;


            // 1. Usar el repositorio para marcar la nueva entidad para ser agregada
            await _rangoRepository.AddAsync(nuevoRango);

            // 2. Usar la Unidad de Trabajo para confirmar TODOS los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync();

            return nuevoRango;
        }
    }
}
