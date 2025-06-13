using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class ExperienciaLaboralRepository : IExperienciaLaboralRepository
    {
        private readonly SigadDbContext _context;

        public ExperienciaLaboralRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetAllAsync()
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .ToListAsync();
        }

        public async Task<ExperienciaLaboral?> GetByIdAsync(int id)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .Where(e => e.ExperienciasPorSolicitud.Any(eps => eps.SolicitudId == solicitudId))
                .ToListAsync();
        }

        public async Task AddAsync(ExperienciaLaboral experiencia)
        {
            await _context.ExperienciasLaborales.AddAsync(experiencia);
        }

        public async Task UpdateAsync(ExperienciaLaboral experiencia)
        {
            _context.ExperienciasLaborales.Update(experiencia);
        }

        public async Task DeleteAsync(int id)
        {
            var experiencia = await _context.ExperienciasLaborales.FindAsync(id);
            if (experiencia != null)
            {
                _context.ExperienciasLaborales.Remove(experiencia);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ExperienciasLaborales.AnyAsync(e => e.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int experienciaId)
        {
            var experienciaPorSolicitud = new ExperienciaPorSolicitud
            {
                SolicitudId = solicitudId,
                ExperienciaId = experienciaId
            };

            await _context.ExperienciasPorSolicitud.AddAsync(experienciaPorSolicitud);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int experienciaId)
        {
            var experienciaPorSolicitud = await _context.ExperienciasPorSolicitud
                .FirstOrDefaultAsync(eps => eps.SolicitudId == solicitudId && eps.ExperienciaId == experienciaId);

            if (experienciaPorSolicitud != null)
            {
                _context.ExperienciasPorSolicitud.Remove(experienciaPorSolicitud);
            }
        }
    }
} 