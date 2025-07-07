using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio para la entidad Notificacion.
    /// Implementa directamente la interfaz ya que no hay una clase BaseRepository.
    /// </summary>
    public class EfNotificacionRepository : INotificacionRepository
    {
        protected readonly SigadDbContext _context;
        protected readonly DbSet<Notificacion> _dbSet;

        public EfNotificacionRepository(SigadDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Notificacion>();
        }

        #region Implementación de IBaseRepository<Notificacion>

        public async Task<Notificacion> GetByIdAsync(int id)
        {
            // FindAsync es la forma más eficiente de buscar por clave primaria.
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<Notificacion>> GetAllAsync()
        {
            // ToListAsync materializa la consulta y devuelve la lista.
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<Notificacion>> FindAsync(Expression<Func<Notificacion, bool>> expression)
        {
            // Aplica el filtro (expresión) y devuelve los resultados.
            return await _dbSet.Where(expression).ToListAsync();
        }

        public async Task AddAsync(Notificacion entity)
        {
            // Marca la entidad como nueva para ser insertada.
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<Notificacion> entities)
        {
            // Marca un conjunto de entidades como nuevas.
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(Notificacion entity)
        {
            // Adjunta la entidad al contexto y la marca como modificada.
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(Notificacion entity)
        {
            // Marca la entidad para ser eliminada.
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Notificacion> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public async Task<int> CountUnreadByCedulaAsync(string cedula)
        {
            return await _context.Notificaciones
                .CountAsync(n => n.DocenteCedula == cedula && !n.EsLeida);
        }
        public async Task<IEnumerable<Notificacion>> GetAllByCedulaOrderedByDateAsync(string cedula)
        {
            return await _context.Notificaciones
                .Where(n => n.DocenteCedula == cedula)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();
        }
        #endregion
    }
}