using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
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
    }
}