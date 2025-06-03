using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<Rango> GetByIdAsync(Guid id)
        {
            // FindAsync es eficiente para buscar por clave primaria.
            // Retorna null si no lo encuentra.
            return await _context.Rangos.FindAsync(id);
        }

        public async Task<IEnumerable<Rango>> GetAllAsync()
        {
            // ToListAsync ejecuta la consulta a la base de datos y trae todos los resultados.
            return await _context.Rangos.ToListAsync();
        }

        public async Task AddAsync(Rango rango)
        {
            // AddAsync marca la entidad para ser insertada.
            await _context.Rangos.AddAsync(rango);
            // NOTA: SaveChangesAsync() no se llama aquí usualmente en un patrón Repositorio puro.
            // Se maneja a un nivel superior (ej. Unit of Work en el Application Service)
            // para agrupar múltiples operaciones en una sola transacción.
            // Por ahora, para simplificar y ver resultados rápidos, podríamos llamarlo aquí
            // o lo llamaremos explícitamente en el Application Service después de esta operación.
            // Para nuestro primer ejemplo, lo dejaremos sin SaveChangesAsync aquí.
        }

        public Task UpdateAsync(Rango rango)
        {
            // EF Core rastrea cambios en entidades que ha cargado.
            // Simplemente marcar el estado como modificado es una forma.
            _context.Entry(rango).State = EntityState.Modified;
            // O si ya está siendo rastreado y modificaste sus propiedades, SaveChangesAsync lo detectará.
            // De nuevo, SaveChangesAsync() se manejaría idealmente a un nivel superior.
            return Task.CompletedTask; // Si no hay operaciones async directas
        }

        public async Task DeleteAsync(Guid id)
        {
            var rango = await _context.Rangos.FindAsync(id);
            if (rango != null)
            {
                _context.Rangos.Remove(rango);
                // SaveChangesAsync() se manejaría a un nivel superior.
            }
        }
    }
}