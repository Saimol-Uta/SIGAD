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

        // Implementación básica - se completará más adelante
        public GestionArticulosAppService(){

        private readonly IArticuloRepository _articuloRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;
        public GestionArticulosAppService(IArticuloRepository articuloRepository, IDocenteRepository docenteRepository, IUnitOfWork unitOfWork)
        {
            _articuloRepository = articuloRepository;
            _unitOfWork = unitOfWork;
            _docenteRepository = docenteRepository;
        }

        public async Task<Articulo> CrearArticuloAsync(CrearArticuloDto articuloDto, string docenteCedula)
        {
            var articuloExistente = await _articuloRepository.GetByDoiAsync(articuloDto.DOI);
            if (articuloExistente == null)
            {
                throw new InvalidOperationException("El artículo ya ha sido ingresado con anterioridad");
            }

            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            var nuevoArticulo = new Articulo();
            nuevoArticulo.Titulo = articuloDto.Titulo;
            nuevoArticulo.DOI = articuloDto.DOI;
            nuevoArticulo.ArchivoRuta = articuloDto.ArchivoRuta;
            nuevoArticulo.AnioPublicacion = articuloDto.AnioPublicacion;
            nuevoArticulo.Revista = articuloDto.Revista;
            nuevoArticulo.ContenidoHash = articuloDto.ContenidoHash;
            nuevoArticulo.DocenteCedula = docenteCedula;

            await _articuloRepository.AddAsync(nuevoArticulo);

            await _unitOfWork.SaveChangesAsync();

            return nuevoArticulo;
        }

        public async Task<IEnumerable<VerArticuloDto>> GetArticulosPorDocenteAsync(string docenteCedula)
        {
            // 1. Verifica si el docente existe
            bool docenteExiste = await _docenteRepository.ExistsByCedulaAsync(docenteCedula);
            if (!docenteExiste)
            {
                throw new InvalidOperationException("El docente con la cédula proporcionada no existe.");
            }

            // 2. Obtiene los artículos asociados al docente
            var articulos = await _articuloRepository.GetAllByDocenteAsync(docenteCedula);

            // 3. Mapea los artículos a DTOs para visualización
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