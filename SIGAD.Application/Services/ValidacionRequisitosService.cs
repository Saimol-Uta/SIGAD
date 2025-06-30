// Servicio desde cero considerando TODOS los documentos y requisitos
using Microsoft.Extensions.Logging;
using SIGAD.Application.DTOs.Validacion;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class ValidacionRequisitosService : IValidacionRequisitosService
    {
        private readonly ILogger<ValidacionRequisitosService> _logger;
        private readonly IRangoRepository _rangoRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IArticuloRepository _articuloRepository;
        private readonly ICursoRepository _cursoRepository;
        private readonly IInvestigacionRepository _investigacionRepository;
        private readonly IEvaluacionDocenteRepository _evaluacionRepository;
        private readonly ITesisDirigidaRepository _tesisRepository;

        public ValidacionRequisitosService(
            ILogger<ValidacionRequisitosService> logger,
            IRangoRepository rangoRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IArticuloRepository articuloRepository,
            ICursoRepository cursoRepository,
            IInvestigacionRepository investigacionRepository,
            IEvaluacionDocenteRepository evaluacionRepository,
            ITesisDirigidaRepository tesisRepository)
        {
            _logger = logger;
            _rangoRepository = rangoRepository;
            _solicitudRepository = solicitudRepository;
            _articuloRepository = articuloRepository;
            _cursoRepository = cursoRepository;
            _investigacionRepository = investigacionRepository;
            _evaluacionRepository = evaluacionRepository;
            _tesisRepository = tesisRepository;
        }

        public async Task<ProgresoRequisitosDto> VerificarProgresoAsync(string docenteCedula, int rangoId)
        {
            var resultado = new ProgresoRequisitosDto
            {
                Antiguedad = new(),
                PromedioEvaluacion = new(),
                Articulos = new(),
                Investigaciones = new(),
                Cursos = new(),
                Tesis = new()
            };

            var rango = await _rangoRepository.GetByIdAsync(rangoId);
            if (rango == null) throw new Exception("Rango no encontrado");

            var solicitudes = await _solicitudRepository.GetAllAsync();
            var ultimaAprobada = solicitudes
                .Where(s => s.DocenteCedula == docenteCedula && s.Estado == EstadoSolicitud.Aprobada)
                .OrderByDescending(s => s.FechaResolucion)
                .FirstOrDefault();

            var fechaInicio = ultimaAprobada?.FechaResolucion?.Date ?? DateTime.MinValue;

            // Antigüedad
            resultado.Antiguedad.Requerido = rango.AniosExperienciaRequeridos;
            resultado.Antiguedad.Actual = fechaInicio == DateTime.MinValue ? 0 : (decimal)(DateTime.Now - fechaInicio).TotalDays / 365.25m;
            resultado.Antiguedad.Mensaje = $"Antigüedad: {resultado.Antiguedad.Actual:F1} de {resultado.Antiguedad.Requerido} años.";

            // Evaluaciones
            var evaluaciones = await _evaluacionRepository.GetByDocenteCedulaAsync(docenteCedula);
            var evalPeriodo = evaluaciones.Where(e => e.FechaEvaluacion >= fechaInicio);
            resultado.PromedioEvaluacion.Requerido = rango.PuntajePromedioEvaluacionesRequerido;
            resultado.PromedioEvaluacion.Actual = evalPeriodo.Any() ? evalPeriodo.Average(e => e.PuntajePorcentual) : 0;
            resultado.PromedioEvaluacion.Mensaje = $"Evaluaciones: {resultado.PromedioEvaluacion.Actual:F2}% de {resultado.PromedioEvaluacion.Requerido}%";

            // Artículos
            var articulos = await _articuloRepository.GetByDocenteCedulaAsync(docenteCedula);
            resultado.Articulos.Requerido = rango.ArticulosRequeridos;
            resultado.Articulos.Actual = articulos.Count(a => a.AnioPublicacion >= fechaInicio.Year);
            resultado.Articulos.Mensaje = $"Artículos: {resultado.Articulos.Actual} de {resultado.Articulos.Requerido}";

            // Investigaciones
            var investigaciones = await _investigacionRepository.GetByDocenteCedulaAsync(docenteCedula);
            resultado.Investigaciones.Requerido = rango.MesesInvestigacionRequeridos;
            resultado.Investigaciones.Actual = investigaciones.Where(i => i.FechaFinalizacion >= fechaInicio).Sum(i => i.MesesDeInvestigacion);
            resultado.Investigaciones.Mensaje = $"Meses Investigación: {resultado.Investigaciones.Actual} de {resultado.Investigaciones.Requerido}";

            // Cursos
            var cursos = await _cursoRepository.GetByDocenteCedulaAsync(docenteCedula);
            resultado.Cursos.Requerido = rango.HorasCursoRequeridas;
            resultado.Cursos.Actual = cursos.Where(c => c.FechaFinalizacion >= fechaInicio).Sum(c => c.NumeroHoras);
            resultado.Cursos.Mensaje = $"Horas de Curso: {resultado.Cursos.Actual} de {resultado.Cursos.Requerido}";

            // Tesis Dirigidas (si lo agregas como campo requerido en el rango)
            resultado.Tesis.Requerido = rango.TesisDirigidasRequeridas;
            var tesis = await _tesisRepository.GetByDocenteCedulaAsync(docenteCedula);
            resultado.Tesis.Actual = tesis.Count(t => t.FechaFin >= fechaInicio);
            resultado.Tesis.Mensaje = $"Tesis Dirigidas: {resultado.Tesis.Actual} de {resultado.Tesis.Requerido}";

            // Artículos en idioma extranjero (para Principal 1→2 y Principal 2→3)
            int articulosIdiomaExtranjero = 0;
            bool cumpleArticulosIdiomaExtranjero = true;
            if (rango.Nombre.Contains("Principal 1 a Principal 2"))
            {
                articulosIdiomaExtranjero = articulos.Count(a => a.AnioPublicacion >= fechaInicio.Year && !string.IsNullOrEmpty(a.IdiomaPublicacion) && !a.IdiomaPublicacion.ToLower().Contains("español"));
                resultado.Articulos.Mensaje += $" | Artículos en idioma extranjero: {articulosIdiomaExtranjero} (mínimo 1)";
                cumpleArticulosIdiomaExtranjero = articulosIdiomaExtranjero >= 1;
            }
            else if (rango.Nombre.Contains("Principal 2 a Principal 3"))
            {
                articulosIdiomaExtranjero = articulos.Count(a => a.AnioPublicacion >= fechaInicio.Year && !string.IsNullOrEmpty(a.IdiomaPublicacion) && !a.IdiomaPublicacion.ToLower().Contains("español"));
                resultado.Articulos.Mensaje += $" | Artículos en idioma extranjero: {articulosIdiomaExtranjero} (mínimo 2)";
                cumpleArticulosIdiomaExtranjero = articulosIdiomaExtranjero >= 2;
            }

            // Investigaciones internacionales (para Principal 1→2 y Principal 2→3)
            int investigacionesInternacionales = 0;
            bool cumpleInvestigacionesInternacionales = true;
            if (rango.Nombre.Contains("Principal 1 a Principal 2"))
            {
                investigacionesInternacionales = investigaciones.Count(i => i.FechaFinalizacion >= fechaInicio && i.EsInternacional);
                resultado.Investigaciones.Mensaje += $" | Proyectos internacionales: {investigacionesInternacionales} (mínimo 1)";
                cumpleInvestigacionesInternacionales = investigacionesInternacionales >= 1;
            }
            else if (rango.Nombre.Contains("Principal 2 a Principal 3"))
            {
                investigacionesInternacionales = investigaciones.Count(i => i.FechaFinalizacion >= fechaInicio && i.EsInternacional);
                resultado.Investigaciones.Mensaje += $" | Proyectos internacionales: {investigacionesInternacionales} (mínimo 2)";
                cumpleInvestigacionesInternacionales = investigacionesInternacionales >= 2;
            }

            // Horas de capacitación impartidas (para Principal 1→2 y Principal 2→3)
            int horasImpartidas = 0;
            bool cumpleHorasImpartidas = true;
            if (rango.Nombre.Contains("Principal 1 a Principal 2"))
            {
                horasImpartidas = cursos.Where(c => c.FechaFinalizacion >= fechaInicio && c.HorasImpartidas.HasValue).Sum(c => c.HorasImpartidas ?? 0);
                resultado.Cursos.Mensaje += $" | Horas impartidas: {horasImpartidas} (mínimo 40)";
                cumpleHorasImpartidas = horasImpartidas >= 40;
            }
            else if (rango.Nombre.Contains("Principal 2 a Principal 3"))
            {
                horasImpartidas = cursos.Where(c => c.FechaFinalizacion >= fechaInicio && c.HorasImpartidas.HasValue).Sum(c => c.HorasImpartidas ?? 0);
                resultado.Cursos.Mensaje += $" | Horas impartidas: {horasImpartidas} (mínimo 80)";
                cumpleHorasImpartidas = horasImpartidas >= 80;
            }

            resultado.PuedeAscender = resultado.Antiguedad.Cumple &&
                                      resultado.PromedioEvaluacion.Cumple &&
                                      resultado.Articulos.Cumple && cumpleArticulosIdiomaExtranjero &&
                                      resultado.Investigaciones.Cumple && cumpleInvestigacionesInternacionales &&
                                      resultado.Cursos.Cumple && cumpleHorasImpartidas &&
                                      resultado.Tesis.Cumple;

            return resultado;
        }
    }
}
