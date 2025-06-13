/*
using Microsoft.Extensions.Logging;
using SIGAD.Application.DTOs.Validacion;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class ValidacionRequisitosService : IValidacionRequisitosService
    {
        private readonly ILogger<ValidacionRequisitosService> _logger;
        private readonly IRangoRepository _rangoRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IEvaluacionDocenteRepository _evaluacionRepository;
        private readonly IArticuloRepository _articuloRepository;
        private readonly IInvestigacionRepository _investigacionRepository;
        private readonly ICursoRepository _cursoRepository;

        public ValidacionRequisitosService(
            ILogger<ValidacionRequisitosService> logger,
            IRangoRepository rangoRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IEvaluacionDocenteRepository evaluacionRepository,
            IArticuloRepository articuloRepository,
            IInvestigacionRepository investigacionRepository,
            ICursoRepository cursoRepository)
        {
            _logger = logger;
            _rangoRepository = rangoRepository;
            _solicitudRepository = solicitudRepository;
            _evaluacionRepository = evaluacionRepository;
            _articuloRepository = articuloRepository;
            _investigacionRepository = investigacionRepository;
            _cursoRepository = cursoRepository;
        }

        public async Task<ProgresoRequisitosDto> VerificarProgresoAsync(string docenteCedula, int rangoId)
        {
            _logger.LogInformation("Iniciando verificación de progreso para docente {Cedula} hacia rango {RangoId}", docenteCedula, rangoId);

            var resultado = new ProgresoRequisitosDto
            {
                Antiguedad = new RequisitoProgresoDto(),
                PromedioEvaluacion = new RequisitoProgresoDto(),
                Articulos = new RequisitoProgresoDto(),
                Investigaciones = new RequisitoProgresoDto(),
                Cursos = new RequisitoProgresoDto()
            };

            var rangoRequisitos = await _rangoRepository.GetByIdAsync(rangoId);
            if (rangoRequisitos == null)
            {
                throw new KeyNotFoundException($"Rango con ID {rangoId} no encontrado.");
            }

            var todasLasSolicitudes = await _solicitudRepository.GetAllAsync();
            var ultimaSolicitudAprobada = todasLasSolicitudes
                .Where(s => s.DocenteCedula == docenteCedula && s.Estado == EstadoSolicitud.Aprobada)
                .OrderByDescending(s => s.FechaResolucion)
                .FirstOrDefault();

            DateTime fechaInicioPeriodo = ultimaSolicitudAprobada?.FechaResolucion?.Date ?? DateTime.MinValue;

            // Antigüedad (Años en el rango actual)
            resultado.Antiguedad.Requerido = rangoRequisitos.AniosExperienciaRequeridos;
            resultado.Antiguedad.Actual = fechaInicioPeriodo == DateTime.MinValue ? 0 : (decimal)(DateTime.Now - fechaInicioPeriodo).TotalDays / 365.25m;
            resultado.Antiguedad.Mensaje = $"Antigüedad: {resultado.Antiguedad.Actual:F1} de {resultado.Antiguedad.Requerido} años requeridos.";

            // Evaluaciones (Promedio de puntaje) - USANDO EL MÉTODO CORRECTO
            var evaluaciones = await _evaluacionRepository.GetByDocenteCedulaAsync(docenteCedula);
            var evaluacionesDelPeriodo = evaluaciones.Where(e => e.FechaEvaluacion >= fechaInicioPeriodo).ToList();
            resultado.PromedioEvaluacion.Requerido = rangoRequisitos.PuntajePromedioEvaluacionesRequerido;
            resultado.PromedioEvaluacion.Actual = evaluacionesDelPeriodo.Any() ? evaluacionesDelPeriodo.Average(e => e.PuntajePorcentual) : 0;
            resultado.PromedioEvaluacion.Mensaje = $"Promedio de Evaluaciones: {resultado.PromedioEvaluacion.Actual:F2}% de {resultado.PromedioEvaluacion.Requerido}% requerido.";

            // Artículos (Cantidad)
            var articulos = await _articuloRepository.GetByDocenteAsync(docenteCedula);
            var articulosDelPeriodo = articulos.Where(a => a.AnioPublicacion >= fechaInicioPeriodo.Year).ToList();
            resultado.Articulos.Requerido = rangoRequisitos.ArticulosRequeridos;
            resultado.Articulos.Actual = articulosDelPeriodo.Count;
            resultado.Articulos.Mensaje = $"Artículos: {resultado.Articulos.Actual} de {resultado.Articulos.Requerido} requeridos.";

            // Investigaciones (Suma de meses)
            var investigaciones = await _investigacionRepository.GetByDocenteAsync(docenteCedula);
            var investigacionesDelPeriodo = investigaciones.Where(i => i.FechaFinalizacion >= fechaInicioPeriodo).ToList();
            resultado.Investigaciones.Requerido = rangoRequisitos.MesesInvestigacionRequeridos;
            resultado.Investigaciones.Actual = investigacionesDelPeriodo.Sum(i => i.MesesDeInvestigacion);
            resultado.Investigaciones.Mensaje = $"Investigaciones: {resultado.Investigaciones.Actual} de {resultado.Investigaciones.Requerido} meses requeridos.";

            // Cursos (Suma de horas)
            var cursos = await _cursoRepository.GetByDocenteAsync(docenteCedula);
            var cursosDelPeriodo = cursos.Where(c => c.FechaFinalizacion >= fechaInicioPeriodo).ToList();
            resultado.Cursos.Requerido = rangoRequisitos.HorasCursoRequeridas;
            resultado.Cursos.Actual = cursosDelPeriodo.Sum(c => c.NumeroHoras);
            resultado.Cursos.Mensaje = $"Cursos: {resultado.Cursos.Actual} de {resultado.Cursos.Requerido} horas requeridas.";

            resultado.PuedeAscender = resultado.Antiguedad.Cumple &&
                                      resultado.PromedioEvaluacion.Cumple &&
                                      resultado.Articulos.Cumple &&
                                      resultado.Investigaciones.Cumple &&
                                      resultado.Cursos.Cumple;

            return resultado;
        }
    }
}
*/