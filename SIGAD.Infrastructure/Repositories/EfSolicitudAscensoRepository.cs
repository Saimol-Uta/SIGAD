using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfSolicitudAscensoRepository : ISolicitudAscensoRepository
    {
        private readonly SigadDbContext _context;

        public EfSolicitudAscensoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<SolicitudAscenso?> GetByIdAsync(Guid id)
        {
            return await _context.SolicitudesAscenso.FindAsync(id);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllAsync()
        {
            return await _context.SolicitudesAscenso.ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync()
        {
            return await _context.SolicitudesAscenso
                .Include(s => s.Docente)
                .Include(s => s.RangoActual)
                .Include(s => s.RangoSolicitado)
                .ToListAsync();
        }

        public async Task AddAsync(SolicitudAscenso solicitud)
        {
            await _context.SolicitudesAscenso.AddAsync(solicitud);
        }

        public async Task UpdateAsync(SolicitudAscenso solicitud)
        {
            await Task.Run(() => _context.SolicitudesAscenso.Update(solicitud));
        }

        public async Task DeleteAsync(Guid id)
        {
            var solicitud = await GetByIdAsync(id);
            if (solicitud != null)
            {
                _context.SolicitudesAscenso.Remove(solicitud);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.SolicitudesAscenso.AnyAsync(s => s.Id == id);
        }
    }
} 