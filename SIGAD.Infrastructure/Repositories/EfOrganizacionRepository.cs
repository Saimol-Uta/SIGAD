using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfOrganizacionRepository : IOrganizacionRepository
    {
        private readonly SigadDbContext _context;

        public EfOrganizacionRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Organizacion>> GetAllAsync()
        {
            return await _context.Organizaciones.ToListAsync();
        }

        public async Task<Organizacion?> GetByIdAsync(int id)
        {
            return await _context.Organizaciones.FindAsync(id);
        }

        public async Task<Organizacion?> GetByNombreAsync(string nombre)
        {
            return await _context.Organizaciones
                .FirstOrDefaultAsync(o => o.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task AddAsync(Organizacion organizacion)
        {
            await _context.Organizaciones.AddAsync(organizacion);
        }

        public async Task UpdateAsync(Organizacion organizacion)
        {
            _context.Organizaciones.Update(organizacion);
        }

        public async Task DeleteAsync(int id)
        {
            var organizacion = await _context.Organizaciones.FindAsync(id);
            if (organizacion != null)
            {
                _context.Organizaciones.Remove(organizacion);
            }
        }
        public async Task<Organizacion?> ObtenerPorNombreAsync(string nombre)
        {
            return await _context.Organizaciones
                .FirstOrDefaultAsync(o => o.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task AgregarAsync(Organizacion organizacion)
        {
            await _context.Organizaciones.AddAsync(organizacion);
        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Organizaciones.AnyAsync(o => o.Id == id);
        }
    }
} 