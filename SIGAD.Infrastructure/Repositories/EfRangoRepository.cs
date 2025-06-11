// En: SIGAD.Infrastructure/Repositories/EfRangoRepository.cs
using Microsoft.EntityFrameworkCore; // ¡Importante para ToListAsync!
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System;
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

        public async Task<Rango> GetByIdAsync(int id)
        {
            return await _context.Rangos.FindAsync(id);
        }

        public async Task AddAsync(Rango rango)
        {
            await _context.Rangos.AddAsync(rango);
        }

        public Task UpdateAsync(Rango rango)
        {
            _context.Rangos.Update(rango);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var rango = await GetByIdAsync(id);
            if (rango != null)
            {
                _context.Rangos.Remove(rango);
            }
        }
    }
}