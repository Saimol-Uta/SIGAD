using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfArticuloRepository : IArticuloRepository
    {
        private readonly SigadDbContext _context;

        public EfArticuloRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Articulo>> GetAllAsync()
        {
            return await _context.Articulos.AsNoTracking().ToListAsync();
        }

        public async Task<Articulo?> GetByIdAsync(string doi)
        {
            return await _context.Articulos.FindAsync(doi);
        }

        public async Task<IEnumerable<Articulo>> GetByDocenteAsync(string cedula)
        {
            return await _context.Articulos
                .AsNoTracking()
                .Where(a => a.DocenteCedula == cedula)
                .ToListAsync();
        }

        // --- MÉTODO AÑADIDO ---
        public async Task AddAsync(Articulo articulo)
        {
            await _context.Articulos.AddAsync(articulo);
        }
    }
}