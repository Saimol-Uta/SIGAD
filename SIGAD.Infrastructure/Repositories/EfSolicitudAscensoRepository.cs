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
                .Include(s => s.CursosPorSolicitud)
                    .ThenInclude(cps => cps.Curso)
                .Include(s => s.InvestigacionesPorSolicitud)
                    .ThenInclude(ips => ips.Investigacion)
                .Include(s => s.ExperienciaPorSolicitud)
                    .ThenInclude(eps => eps.ExperienciaLaboral)
                .Include(s => s.EvaluacionesPorSolicitud)
                    .ThenInclude(evps => evps.Evaluacion)
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
    }
}