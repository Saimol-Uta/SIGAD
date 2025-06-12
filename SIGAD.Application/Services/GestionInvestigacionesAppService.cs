using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionInvestigacionesAppService
    {
        // 1. Los campos se declaran aquí
        private readonly IInvestigacionRepository _investigacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;

        // 2. Solo hay un constructor
        public GestionInvestigacionesAppService(IInvestigacionRepository investigacionRepository, IDocenteRepository docenteRepository, IUnitOfWork unitOfWork)
        {
            _investigacionRepository = investigacionRepository;
            _docenteRepository = docenteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CrearInvestigacionAsync(CrearInvestigacionDto investigacionDto, string docenteCedula)
        {
            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            var nuevaInvestigacion = new Investigacion
            {
                Titulo = investigacionDto.Titulo,
                InformeRuta = investigacionDto.InformeRuta,
                RolEnInvestigacion = investigacionDto.RolEnInvestigacion,
                MesesDeInvestigacion = investigacionDto.MesesDeInvestigacion,
                FechaInicio = investigacionDto.FechaInicio,
                ContenidoHash = investigacionDto.ContenidoHash,
                FechaFinalizacion = investigacionDto.FechaFinalizacion,
                DocenteCedula = docenteCedula
            };

            await _investigacionRepository.AddAsync(nuevaInvestigacion);
            await _unitOfWork.SaveChangesAsync();
            return nuevaInvestigacion.Id;
        }

        public async Task<IEnumerable<VerInvestigacionDto>> GetInvestigacionesPorDocenteAsync(string docenteCedula)
        {
            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }
            
            // 3. Lógica corregida: Usa el método de la interfaz
            var investigaciones = await _investigacionRepository.GetByDocenteAsync(docenteCedula);

            var investigacionesDto = investigaciones.Select(i => new VerInvestigacionDto
            {
                Id = i.Id,
                Titulo = i.Titulo,
                MesesDeInvestigacion = i.MesesDeInvestigacion,
            });

            return investigacionesDto;
        }
    }
}

