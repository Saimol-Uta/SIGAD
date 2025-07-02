using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfEvaluacionDocenteRepository : IEvaluacionDocenteRepository
    {
        private readonly SigadDbContext _context;

        public EfEvaluacionDocenteRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetAllAsync()
        {
            return await _context.EvaluacionesDocentes
                .Include(e => e.Docente)
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToListAsync();
        }

        public async Task<EvaluacionDocente?> GetByIdAsync(int id)
        {
            return await _context.EvaluacionesDocentes
                .Include(e => e.Docente)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.EvaluacionesDocentes
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.EvaluacionesPorSolicitud
                .Where(eps => eps.SolicitudId == solicitudId)
                .Include(eps => eps.Evaluacion)
                .ThenInclude(e => e!.Docente)
                .Select(eps => eps.Evaluacion!)
                .ToListAsync();
        }

        public async Task AddAsync(EvaluacionDocente evaluacion)
        {
            await _context.EvaluacionesDocentes.AddAsync(evaluacion);
        }

        public async Task UpdateAsync(EvaluacionDocente evaluacion)
        {
            _context.EvaluacionesDocentes.Update(evaluacion);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var evaluacion = await _context.EvaluacionesDocentes.FindAsync(id);
            if (evaluacion != null)
            {
                _context.EvaluacionesDocentes.Remove(evaluacion);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EvaluacionesDocentes.AnyAsync(e => e.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int evaluacionId)
        {
            var evaluacionPorSolicitud = new EvaluacionesPorSolicitud
            {
                SolicitudId = solicitudId,
                EvaluacionId = evaluacionId
            };

            await _context.EvaluacionesPorSolicitud.AddAsync(evaluacionPorSolicitud);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int evaluacionId)
        {
            var evaluacionPorSolicitud = await _context.EvaluacionesPorSolicitud
                .FirstOrDefaultAsync(eps => eps.SolicitudId == solicitudId && eps.EvaluacionId == evaluacionId);

            if (evaluacionPorSolicitud != null)
            {
                _context.EvaluacionesPorSolicitud.Remove(evaluacionPorSolicitud);
            }
        }
        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _context.EvaluacionesDocentes.AnyAsync(e => e.ContenidoHash == hash);
        }

        public async Task AgregarAsync(EvaluacionDocente evaluacion)
        {
            await _context.EvaluacionesDocentes.AddAsync(evaluacion);
        }

        // Métodos específicos del reglamento
        public async Task<decimal> GetPromedioUltimas4EvaluacionesAsync(string docenteCedula)
        {
            var ultimas4 = await GetUltimas4EvaluacionesAsync(docenteCedula);
            if (!ultimas4.Any())
                return 0;

            return ultimas4.Average(e => e.PuntajePorcentual);
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetUltimas4EvaluacionesAsync(string docenteCedula)
        {
            return await _context.EvaluacionesDocentes
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Take(4)
                .ToListAsync();
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetUltimas2EvaluacionesAsync(string docenteCedula)
        {
            return await _context.EvaluacionesDocentes
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Take(2)
                .ToListAsync();
        }

        public async Task<bool> CumpleRequisitoEvaluacionParaRangoAsync(string docenteCedula, decimal puntajeMinimo = 75)
        {
            var promedio = await GetPromedioUltimas4EvaluacionesAsync(docenteCedula);
            return promedio >= puntajeMinimo;
        }

        public async Task<bool> TieneEvaluacionesSuficientesAsync(string docenteCedula, int cantidadMinima = 4)
        {
            var cantidadEvaluaciones = await _context.EvaluacionesDocentes
                .Where(e => e.DocenteCedula == docenteCedula)
                .CountAsync();

            return cantidadEvaluaciones >= cantidadMinima;
        }

        public async Task<bool> EstaEvaluacionYaUsadaAsync(int evaluacionId)
        {
            // Verificar si la evaluación está asociada a una solicitud aprobada o enviada
            var estaUsada = await _context.EvaluacionesPorSolicitud
                .Include(eps => eps.Solicitud)
                .AnyAsync(eps => eps.EvaluacionId == evaluacionId && 
                    eps.Solicitud != null &&
                    (eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Aprobada || 
                     eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Enviada));

            return estaUsada;
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetEvaluacionesDisponiblesParaSolicitudAsync(string docenteCedula, Guid? solicitudActualId = null)
        {
            // Obtener las últimas 4 evaluaciones del docente
            var ultimasEvaluaciones = await _context.EvaluacionesDocentes
                .Where(e => e.DocenteCedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Take(4)
                .ToListAsync();

            // Filtrar las que no estén ya usadas en otras solicitudes aprobadas/enviadas
            var evaluacionesDisponibles = new List<EvaluacionDocente>();

            foreach (var evaluacion in ultimasEvaluaciones)
            {
                var estaUsadaEnOtraSolicitud = await _context.EvaluacionesPorSolicitud
                    .Include(eps => eps.Solicitud)
                    .AnyAsync(eps => eps.EvaluacionId == evaluacion.Id && 
                        eps.SolicitudId != solicitudActualId &&
                        eps.Solicitud != null &&
                        (eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Aprobada || 
                         eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Enviada));

                if (!estaUsadaEnOtraSolicitud)
                {
                    evaluacionesDisponibles.Add(evaluacion);
                }
            }

            return evaluacionesDisponibles;
        }

        public async Task<IEnumerable<EvaluacionDocente>> GetEvaluacionesUsadasEnSolicitudesAsync(string docenteCedula)
        {
            // Obtener evaluaciones del docente que están asociadas a solicitudes aprobadas o enviadas
            var evaluacionesUsadas = await _context.EvaluacionesPorSolicitud
                .Include(eps => eps.Evaluacion)
                .ThenInclude(e => e!.Docente)
                .Include(eps => eps.Solicitud)
                .Where(eps => eps.Evaluacion!.DocenteCedula == docenteCedula &&
                    eps.Solicitud != null &&
                    (eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Aprobada || 
                     eps.Solicitud.Estado == Domain.Enums.EstadoSolicitud.Enviada))
                .Select(eps => eps.Evaluacion!)
                .Distinct()
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToListAsync();

            return evaluacionesUsadas;
        }
    }
}