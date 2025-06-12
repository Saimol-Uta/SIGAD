using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfArticuloRepository : IArticuloRepository
    {
        private readonly SigadDbContext _context;

        public EfArticuloRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetByIdAsync(string doi)
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