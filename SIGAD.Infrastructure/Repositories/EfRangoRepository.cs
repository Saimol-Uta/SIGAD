using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfRangoRepository : IRangoRepository
    {
        private readonly SigadDbContext _context;

        public EfRangoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rango>> GetAllAsync()
        {
            return await _context.Rangos.AsNoTracking().ToListAsync();
        }

        public async Task<Rango?> GetByIdAsync(int id)
        {
            return await _context.Rangos.FindAsync(id);
        }
    }
}