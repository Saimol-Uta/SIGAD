using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfDocenteRepository : IDocenteRepository
    {
        private readonly SigadDbContext _context;

        public EfDocenteRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<Docente?> GetByCedulaAsync(string cedula)
        {
            return await _context.Docentes
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            return await _context.Docentes
                .AnyAsync(d => d.Cedula == cedula);
        }

        public async Task AddAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
        }

        public async Task UpdateAsync(Docente docente)
        {
            _context.Docentes.Update(docente);
            await Task.CompletedTask;
        }
    }
}