using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfSolicitudAscensoRepository : ISolicitudAscensoRepository
    {
        private readonly SigadDbContext _context;

        public EfSolicitudAscensoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SolicitudAscenso solicitud)
        {
            await _context.SolicitudesAscenso.AddAsync(solicitud);
        }

        public async Task<SolicitudAscenso?> GetByIdAsync(Guid id)
        {
            return await _context.SolicitudesAscenso.FindAsync(id);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync()
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoSolicitado)
                .OrderByDescending(s => s.FechaEnvio)
                .ToListAsync();
        }

        public async Task<SolicitudAscenso?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Include(s => s.ArticulosPorSolicitud)
                    .ThenInclude(aps => aps.Articulo)
                        .ThenInclude(a => a.Docente)
                .Include(s => s.CursosPorSolicitud)
                    .ThenInclude(cps => cps.Curso)
                        .ThenInclude(c => c.Organizacion)
                .Include(s => s.CursosPorSolicitud)
                    .ThenInclude(cps => cps.Curso)
                        .ThenInclude(c => c.Docente)
                .Include(s => s.InvestigacionesPorSolicitud)
                    .ThenInclude(ips => ips.Investigacion)
                        .ThenInclude(i => i.Docente)
                .Include(s => s.ExperienciaPorSolicitud)
                    .ThenInclude(eps => eps.ExperienciaLaboral)
                        .ThenInclude(el => el.Organizacion)
                .Include(s => s.ExperienciaPorSolicitud)
                    .ThenInclude(eps => eps.ExperienciaLaboral)
                        .ThenInclude(el => el.Docente)
                .Include(s => s.EvaluacionesPorSolicitud)
                    .ThenInclude(evps => evps.Evaluacion)

                 .Include(s => s.TesisPorSolicitud)
            .ThenInclude(tps => tps.TesisDirigida)

                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task UpdateAsync(SolicitudAscenso solicitud)
        {
            _context.SolicitudesAscenso.Update(solicitud);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.SolicitudesAscenso.AnyAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllAsync()
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .ToListAsync();
        }

        // Métodos específicos para el proceso de promoción
        public async Task<IEnumerable<SolicitudAscenso>> GetByDocenteAsync(string docenteCedula)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.DocenteCedula == docenteCedula)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }

        public async Task<SolicitudAscenso?> GetActiveSolicitudByDocenteAsync(string docenteCedula)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.DocenteCedula == docenteCedula &&
                           (s.Estado == EstadoSolicitud.Borrador ||
                            s.Estado == EstadoSolicitud.Enviada ||
                            s.Estado == EstadoSolicitud.EnRevision))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetByEstadoAsync(EstadoSolicitud estado)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.Estado == estado)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetPendientesRevisionAsync()
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.Estado == EstadoSolicitud.Enviada || s.Estado == EstadoSolicitud.EnRevision)
                .OrderBy(s => s.FechaEnvio)
                .ToListAsync();
        }

        // Métodos para validación de requisitos de promoción
        public async Task<bool> HasActiveSolicitudAsync(string docenteCedula)
        {
            return await _context.SolicitudesAscenso
                .AnyAsync(s => s.DocenteCedula == docenteCedula &&
                              (s.Estado == EstadoSolicitud.Borrador ||
                               s.Estado == EstadoSolicitud.Enviada ||
                               s.Estado == EstadoSolicitud.EnRevision));
        }

        public async Task<int> GetTiempoEnRangoActualAsync(string docenteCedula)
        {
            var docente = await _context.Docentes
                .Include(d => d.RangoActual)
                .FirstOrDefaultAsync(d => d.Cedula == docenteCedula);

            if (docente?.RangoActual == null) return 0;            // Buscar la última promoción aprobada o fecha de inicio del docente
            var ultimaPromocion = await _context.SolicitudesAscenso
                .Where(s => s.DocenteCedula == docenteCedula &&
                           s.Estado == EstadoSolicitud.Aprobada &&
                           s.RangoSolicitadoId == docente.RangoActualId)
                .OrderByDescending(s => s.FechaResolucion)
                .FirstOrDefaultAsync();

            var fechaInicio = ultimaPromocion?.FechaResolucion ?? DateTime.Now.AddYears(-5); // Valor por defecto
            return (int)((DateTime.Now - fechaInicio).TotalDays / 365.25);
        }

        public async Task<bool> CumpleRequisitosParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            // TODO: Implementar validación completa según reglamento UTA
            // Por ahora, validación básica de tiempo en rango actual
            var tiempoEnRango = await GetTiempoEnRangoActualAsync(docenteCedula);

            // Requisito mínimo: al menos 2 años en el rango actual
            return tiempoEnRango >= 2;
        }

        // Métodos para el workflow del reglamento
        public async Task EnviarSolicitudAsync(Guid solicitudId)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null && solicitud.Estado == EstadoSolicitud.Borrador)
            {
                solicitud.Estado = EstadoSolicitud.Enviada;
                solicitud.FechaEnvio = DateTime.UtcNow;
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }
        public async Task AprobarSolicitudAsync(Guid solicitudId, string? observaciones = null)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null && (solicitud.Estado == EstadoSolicitud.Enviada || solicitud.Estado == EstadoSolicitud.EnRevision))
            {
                solicitud.Estado = EstadoSolicitud.Aprobada;
                solicitud.FechaResolucion = DateTime.UtcNow;
                solicitud.ObservacionesAdmin = observaciones;
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }

        public async Task RechazarSolicitudAsync(Guid solicitudId, string observaciones)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null && (solicitud.Estado == EstadoSolicitud.Enviada || solicitud.Estado == EstadoSolicitud.EnRevision))
            {
                solicitud.Estado = EstadoSolicitud.Rechazada;
                solicitud.FechaResolucion = DateTime.UtcNow;
                solicitud.ObservacionesAdmin = observaciones;
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }

        // Métodos específicos para el proceso de dos etapas según Reglamento UTA
        public async Task AprobarPorComisionAsync(Guid solicitudId, string? observaciones = null)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.AprobarPorComision(observaciones);
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }

        public async Task AprobarPorConsejoAsync(Guid solicitudId, string? observaciones = null)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.AprobarPorConsejo(observaciones);
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }

        public async Task FinalizarProcesoAsync(Guid solicitudId, string? observaciones = null)
        {
            var solicitud = await _context.SolicitudesAscenso.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.FinalizarProceso(observaciones);
                _context.SolicitudesAscenso.Update(solicitud);
            }
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetHistorialByDocenteAsync(string docenteCedula)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.DocenteCedula == docenteCedula)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }

        // Métodos para reportes y estadísticas
        public async Task<int> GetCantidadSolicitudesByEstadoAsync(EstadoSolicitud estado)
        {
            return await _context.SolicitudesAscenso
                .CountAsync(s => s.Estado == estado);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetSolicitudesByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking()
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .Where(s => s.FechaCreacion >= fechaInicio && s.FechaCreacion <= fechaFin)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }
    }
}