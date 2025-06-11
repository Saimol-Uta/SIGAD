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

        // --- VAMOS A IMPLEMENTAR ESTE MÉTODO ---
        public async Task<IEnumerable<Rango>> GetAllAsync()
        {
            // Usa el DbContext para acceder a la tabla Rangos y convertirla a una lista de forma asíncrona.
            // Esto se traduce en un "SELECT * FROM Rangos" en SQL.
            return await _context.Rangos.AsNoTracking().ToListAsync();
        }

        // --- Dejaremos los otros métodos para más tarde, pero así se verían ---
        public async Task<Rango?> GetByIdAsync(Guid id)
        {
            // Lo cambiamos a int porque en la BD el ID de Rango es INT
            // return await _context.Rangos.FindAsync(id);
            // NOTA: Como el Id de Rango es INT, no Guid, lo buscamos así:
            return await _context.Rangos.FirstOrDefaultAsync(r => r.Id == (int)(object)id); // Conversión temporal, idealmente el parámetro sería int
        }
        public async Task<Rango> GetByIdAsync(int id) // Método sobrecargado con el tipo correcto
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

        public async Task DeleteAsync(Guid id) // También necesitaría ser int
        {
            var rango = await GetByIdAsync((int)(object)id);
            if (rango != null)
            {
                _context.Rangos.Remove(rango);
            }
        }
    }
}