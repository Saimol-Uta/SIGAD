using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfInvestigacionRepository : IInvestigacionRepository
    {
        private readonly SigadDbContext _context;

        public EfInvestigacionRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Investigacion>> GetAllAsync()
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .ToListAsync();
        }

        public async Task<Investigacion?> GetByIdAsync(int id)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Investigacion>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .Where(i => i.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<Investigacion>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.InvestigacionesPorSolicitud
                .Include(ips => ips.Investigacion)
                    .ThenInclude(i => i.Docente)
                .Where(ips => ips.SolicitudId == solicitudId)
                .Select(ips => ips.Investigacion)
                .ToListAsync();
        }

        public async Task AddAsync(Investigacion investigacion)
        {
            await _context.Investigaciones.AddAsync(investigacion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Investigacion investigacion)
        {
            _context.Investigaciones.Update(investigacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var investigacion = await GetByIdAsync(id);
            if (investigacion != null)
            {
                _context.Investigaciones.Remove(investigacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Investigaciones.AnyAsync(i => i.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int investigacionId)
        {
            var investigacionPorSolicitud = new InvestigacionesPorSolicitud
            {
                SolicitudId = solicitudId,
                InvestigacionId = investigacionId
            };

            await _context.InvestigacionesPorSolicitud.AddAsync(investigacionPorSolicitud);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int investigacionId)
        {
            var investigacionPorSolicitud = await _context.InvestigacionesPorSolicitud
                .FirstOrDefaultAsync(ips => ips.SolicitudId == solicitudId && ips.InvestigacionId == investigacionId);

            if (investigacionPorSolicitud != null)
            {
                _context.InvestigacionesPorSolicitud.Remove(investigacionPorSolicitud);
                await _context.SaveChangesAsync();
            }
        }
    }
}