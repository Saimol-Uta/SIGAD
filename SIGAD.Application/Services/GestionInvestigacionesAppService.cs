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
    public class GestionInvestigacionesAppService
    {
        private readonly IInvestigacionRepository _investigacionRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GestionInvestigacionesAppService(IInvestigacionRepository investigacionRepository, IDocenteRepository docenteRepository, IUnitOfWork unitOfWork)
        {
            _investigacionRepository = investigacionRepository;
            _docenteRepository = docenteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CrearInvestigacionAsync(Investigacion investigacion, string docenteCedula, CrearInvestigacionDto investigacionDto)
        {
            var existeInvestigacion = await _investigacionRepository.GetByIdAsync(investigacion.Id);
            if (existeInvestigacion != null)
            {
                throw new InvalidOperationException("La investigación ya ha sido ingresada con anterioridad");
            }

            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            var nuevaInvestigacion = new Investigacion();
            nuevaInvestigacion.Titulo = investigacionDto.Titulo;
            nuevaInvestigacion.InformeRuta = investigacionDto.InformeRuta;
            nuevaInvestigacion.RolEnInvestigacion = investigacionDto.RolEnInvestigacion;
            nuevaInvestigacion.MesesDeInvestigacion = investigacionDto.MesesDeInvestigacion;
            nuevaInvestigacion.FechaInicio = investigacionDto.FechaInicio;
            nuevaInvestigacion.ContenidoHash = investigacionDto.ContenidoHash;
            nuevaInvestigacion.FechaFinalizacion = investigacionDto.FechaFinalizacion;
            nuevaInvestigacion.DocenteCedula = docenteCedula;

            await _investigacionRepository.AddAsync(nuevaInvestigacion);
            await _unitOfWork.SaveChangesAsync();

            return investigacion.Id;
        }

        public async Task<IEnumerable<VerInvestigacionDto>> GetInvestigacionesPorDocenteAsync(string docenteCedula)
        {
            // 1. Verifica si el docente existe
            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            // 2. Obtiene las investigaciones asociadas al docente
            var investigaciones = await _investigacionRepository.GetAllByDocenteAsync(docenteCedula);

            // 3. Mapea los artículos a DTOs para visualización
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
