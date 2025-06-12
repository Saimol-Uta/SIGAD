using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfCursoRepository : ICursoRepository
    {
        private readonly SigadDbContext _context;

        public EfCursoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetByDocenteAsync(string cedula)
        {
            return await _context.Cursos
                .AsNoTracking()
                .Where(c => c.DocenteCedula == cedula)
                .ToListAsync();
        }
    }
}