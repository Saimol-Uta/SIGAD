/*
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionArticulosAppService
    {
        // 1. Los campos se declaran aquí, a nivel de la clase
        private readonly IArticuloRepository _articuloRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;

        // 2. Solo hay un constructor que recibe las dependencias
        public GestionArticulosAppService(IArticuloRepository articuloRepository, IDocenteRepository docenteRepository, IUnitOfWork unitOfWork)
        {
            _articuloRepository = articuloRepository;
            _unitOfWork = unitOfWork;
            _docenteRepository = docenteRepository;
        }

        public async Task<Articulo> CrearArticuloAsync(CrearArticuloDto articuloDto, string docenteCedula)
        {
            // 3. Lógica corregida: Lanza error si el artículo YA EXISTE (!= null)
            var articuloExistente = await _articuloRepository.GetByIdAsync(articuloDto.DOI);
            if (articuloExistente != null)
            {
                throw new InvalidOperationException("Un artículo con el mismo DOI ya ha sido ingresado.");
            }

            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            var nuevoArticulo = new Articulo
            {
                Titulo = articuloDto.Titulo,
                DOI = articuloDto.DOI,
                ArchivoRuta = articuloDto.ArchivoRuta,
                AnioPublicacion = articuloDto.AnioPublicacion,
                Revista = articuloDto.Revista,
                ContenidoHash = articuloDto.ContenidoHash,
                DocenteCedula = docenteCedula
            };

            await _articuloRepository.AddAsync(nuevoArticulo);
            await _unitOfWork.SaveChangesAsync();
            return nuevoArticulo;
        }

        public async Task<IEnumerable<VerArticuloDto>> GetArticulosPorDocenteAsync(string docenteCedula)
        {
            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            // 4. Lógica corregida: Usa el método que definimos en la interfaz
            var articulos = await _articuloRepository.GetByDocenteAsync(docenteCedula);

            var articulosDto = articulos.Select(a => new VerArticuloDto
            {
                Titulo = a.Titulo,
                DOI = a.DOI,
                Revista = a.Revista,
                AnioPublicacion = a.AnioPublicacion,
            });

            return articulosDto;
        }
    }
}
*/