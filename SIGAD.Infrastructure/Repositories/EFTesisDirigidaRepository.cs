using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfTesisDirigidaRepository : ITesisDirigidaRepository
    {
        private readonly SigadDbContext _context;

        public EfTesisDirigidaRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TesisDirigida>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .Where(t => t.DocenteCedula == docenteCedula)
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TesisDirigida>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.TesisPorSolicitud
                .Where(tps => tps.SolicitudId == solicitudId)
                .Include(tps => tps.TesisDirigida)
                .ThenInclude(t => t!.Docente)
                .Select(tps => tps.TesisDirigida!)
                .ToListAsync();
        }

        public async Task<TesisDirigida?> GetByIdAsync(int id)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TesisDirigida tesis)
        {
            await _context.TesisDirigidas.AddAsync(tesis);
        }

        public async Task UpdateAsync(TesisDirigida tesis)
        {
            _context.TesisDirigidas.Update(tesis);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var tesis = await _context.TesisDirigidas.FindAsync(id);
            if (tesis != null)
            {
                _context.TesisDirigidas.Remove(tesis);
            }
        }

        public async Task<bool> ExistsByHashAsync(string hash)
        {
            return await _context.TesisDirigidas.AnyAsync(t => t.ContenidoHash == hash);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int tesisId)
        {
            var relacion = new TesisPorSolicitud
            {
                SolicitudId = solicitudId,
                TesisDirigidaId = tesisId
            };

            await _context.TesisPorSolicitud.AddAsync(relacion);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int tesisId)
        {
            var relacion = await _context.TesisPorSolicitud
                .FirstOrDefaultAsync(tps => tps.SolicitudId == solicitudId && tps.TesisDirigidaId == tesisId);

            if (relacion != null)
            {
                _context.TesisPorSolicitud.Remove(relacion);
            }
        }
    }
}
