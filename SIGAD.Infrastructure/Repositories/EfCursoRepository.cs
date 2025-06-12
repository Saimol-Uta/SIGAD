using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfCursoRepository : ICursoRepository
    {
        private readonly SigadDbContext _context;

        public EfCursoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .ToListAsync();
        }

        public async Task<Curso?> GetByIdAsync(int id)
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .Where(c => c.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.CursosPorSolicitud
                .Include(cps => cps.Curso)
                    .ThenInclude(c => c.Docente)
                .Include(cps => cps.Curso)
                    .ThenInclude(c => c.Organizacion)
                .Where(cps => cps.SolicitudId == solicitudId)
                .Select(cps => cps.Curso)
                .ToListAsync();
        }

        public async Task AddAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Curso curso)
        {
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var curso = await GetByIdAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Cursos.AnyAsync(c => c.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int cursoId)
        {
            var cursoPorSolicitud = new CursosPorSolicitud
            {
                SolicitudId = solicitudId,
                CursoId = cursoId
            };

            await _context.CursosPorSolicitud.AddAsync(cursoPorSolicitud);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int cursoId)
        {
            var cursoPorSolicitud = await _context.CursosPorSolicitud
                .FirstOrDefaultAsync(cps => cps.SolicitudId == solicitudId && cps.CursoId == cursoId);

            if (cursoPorSolicitud != null)
            {
                _context.CursosPorSolicitud.Remove(cursoPorSolicitud);
                await _context.SaveChangesAsync();
            }
        }
    }
} 