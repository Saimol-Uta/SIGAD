using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfRangoRepository : IRangoRepository
    {
        private readonly SigadDbContext _context;

        public EfRangoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetByIdAsync(int id)
        {
            // Implementación básica - se completará más adelante
            await Task.CompletedTask;
            return null;
        }

        public async Task<IEnumerable<object>> GetAllAsync()
        {
            // Implementación básica - se completará más adelante
            await Task.CompletedTask;
            return new List<object>();
        }
    }
} 