using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.Investigaciones.AsNoTracking().ToListAsync();
        }

        public async Task<Investigacion?> GetByIdAsync(int id)
        {
            return await _context.Investigaciones.FindAsync(id);
        }

        public async Task<IEnumerable<Investigacion>> GetByDocenteAsync(string cedula)
        {
            return await _context.Investigaciones
                .AsNoTracking()
                .Where(i => i.DocenteCedula == cedula)
                .ToListAsync();
        }

        // --- MÉTODO AÑADIDO ---
        public async Task AddAsync(Investigacion investigacion)
        {
            await _context.Investigaciones.AddAsync(investigacion);
        }
    }
}