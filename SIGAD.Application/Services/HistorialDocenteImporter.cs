using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;

using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class HistorialDocenteImporter
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistorialDocenteImporter(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ImportarHistorialAsync(HistorialDocenteDto dto, string cedula)
        {
            var docente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(cedula);
            if (docente is null)
                throw new Exception($"Docente con cédula {cedula} no encontrado.");

            // --- Artículos ---
            foreach (var articuloDto in dto.Articulos)
            {
                if (!await _unitOfWork.Articulos.ExistePorHashAsync(articuloDto.ContenidoHash))
                {
                    var articulo = new Articulo
                    {
                        DOI = articuloDto.DOI,
                        Titulo = articuloDto.Titulo,
                        Revista = articuloDto.Revista,
                        AnioPublicacion = articuloDto.AnioPublicacion,
                        ArchivoRuta = articuloDto.ArchivoRuta,
                        ContenidoHash = articuloDto.ContenidoHash,
                        DocenteCedula = docente.Cedula
                    };
                    await _unitOfWork.Articulos.AgregarAsync(articulo);
                }
            }

            // --- Cursos ---
            foreach (var cursoDto in dto.Cursos)
            {
                if (!await _unitOfWork.Cursos.ExistePorHashAsync(cursoDto.ContenidoHash))
                {
                    var organizacion = await ObtenerORegistrarOrganizacionAsync(cursoDto.Organizacion);
                    var curso = new Curso
                    {
                        Nombre = cursoDto.Nombre,
                        NumeroHoras = cursoDto.NumeroHoras,
                        FechaFinalizacion = cursoDto.FechaFinalizacion,
                        CertificadoRuta = cursoDto.CertificadoRuta,
                        ContenidoHash = cursoDto.ContenidoHash,
                        DocenteCedula = docente.Cedula,
                        OrganizacionId = organizacion.Id
                    };
                    await _unitOfWork.Cursos.AgregarAsync(curso);
                }
            }

            // --- Evaluaciones ---
            foreach (var evalDto in dto.Evaluaciones)
            {
                if (!await _unitOfWork.Evaluaciones.ExistePorHashAsync(evalDto.ContenidoHash))
                {
                    var evaluacion = new EvaluacionDocente
                    {
                        PeriodoAcademico = evalDto.PeriodoAcademico,
                        FechaEvaluacion = evalDto.FechaEvaluacion,
                        PuntajePorcentual = evalDto.PuntajePorcentual,
                        InformeRuta = evalDto.InformeRuta,
                        ContenidoHash = evalDto.ContenidoHash,
                        DocenteCedula = docente.Cedula
                    };
                    await _unitOfWork.Evaluaciones.AgregarAsync(evaluacion);
                }
            }

            // --- Investigaciones ---
            foreach (var invDto in dto.Investigaciones)
            {
                if (!await _unitOfWork.Investigaciones.ExistePorHashAsync(invDto.ContenidoHash))
                {
                    var investigacion = new Investigacion
                    {
                        Titulo = invDto.Titulo,
                        FechaInicio = invDto.FechaInicio,
                        FechaFinalizacion = invDto.FechaFinalizacion,
                        RolEnInvestigacion = invDto.RolEnInvestigacion,
                        MesesDeInvestigacion = invDto.MesesDeInvestigacion,
                        InformeRuta = invDto.InformeRuta,
                        ContenidoHash = invDto.ContenidoHash,
                        DocenteCedula = docente.Cedula
                    };
                    await _unitOfWork.Investigaciones.AgregarAsync(investigacion);
                }
            }

            // --- Experiencia Laboral ---
            foreach (var expDto in dto.Experiencias)
            {
                if (!await _unitOfWork.Experiencias.ExistePorHashAsync(expDto.ContenidoHash))
                {
                    var organizacion = await ObtenerORegistrarOrganizacionAsync(expDto.Organizacion);
                    var experiencia = new ExperienciaLaboral
                    {
                        Cargo = expDto.Cargo,
                        FechaInicio = expDto.FechaInicio,
                        FechaFin = expDto.FechaFin,
                        CertificadoRuta = expDto.CertificadoRuta,
                        ContenidoHash = expDto.ContenidoHash,
                        DocenteCedula = docente.Cedula,
                        OrganizacionId = organizacion.Id
                    };
                    await _unitOfWork.Experiencias.AgregarAsync(experiencia);
                }
            }
            
            await _unitOfWork.CompleteAsync();
        }

        private async Task<Organizacion> ObtenerORegistrarOrganizacionAsync(string nombre)
        {
            var organizacion = await _unitOfWork.Organizaciones.ObtenerPorNombreAsync(nombre);
            if (organizacion == null)
            {
                organizacion = new Organizacion { Nombre = nombre };
                await _unitOfWork.Organizaciones.AgregarAsync(organizacion);
                await _unitOfWork.CompleteAsync();
            }
            return organizacion;
        }
    }
}
