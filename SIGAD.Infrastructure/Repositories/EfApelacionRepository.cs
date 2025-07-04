// ...existing code...
using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de apelaciones usando Entity Framework
    /// </summary>
    public class EfApelacionRepository : IApelacionRepository
    {
        private readonly SigadDbContext _context;

        public EfApelacionRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Apelacion apelacion)
        {
            _context.Apelaciones.Update(apelacion);
            await _context.SaveChangesAsync();
        }

        public async Task<Apelacion> GetByIdAsync(int id)
        {
            var apelacion = await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .FirstOrDefaultAsync(a => a.Id == id);

            return apelacion ?? throw new InvalidOperationException($"Apelación con ID {id} no encontrada");
        }

        public async Task<IReadOnlyList<Apelacion>> GetAllAsync()
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .ToListAsync();
        }

        public async Task<IEnumerable<Apelacion>> FindAsync(System.Linq.Expressions.Expression<Func<Apelacion, bool>> expression)
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(expression)
                .ToListAsync();
        }

        public async Task AddAsync(Apelacion entity)
        {
            await _context.Apelaciones.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<Apelacion> entities)
        {
            await _context.Apelaciones.AddRangeAsync(entities);
        }

        public void Update(Apelacion entity)
        {
            _context.Apelaciones.Update(entity);
        }

        public void Remove(Apelacion entity)
        {
            _context.Apelaciones.Remove(entity);
        }

        public void RemoveRange(IEnumerable<Apelacion> entities)
        {
            _context.Apelaciones.RemoveRange(entities);
        }

        // Métodos específicos de IApelacionRepository

        public async Task<IEnumerable<Apelacion>> GetApelacionesPendientesAsync()
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.Estado == EstadoApelacion.Pendiente)
                .ToListAsync();
        }

        public async Task<IEnumerable<Apelacion>> GetApelacionesPorSolicitudAsync(Guid solicitudId)
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.SolicitudAscensoId == solicitudId)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Apelacion>> GetApelacionesVencidasAsync()
        {
            var fechaLimite = DateTime.UtcNow.AddDays(-3); // Más de 3 días sin resolución

            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.Estado == EstadoApelacion.Pendiente &&
                           a.FechaCreacion < fechaLimite)
                .ToListAsync();
        }

        public async Task<bool> TieneApelacionPendienteAsync(Guid solicitudId)
        {
            return await _context.Apelaciones
                .AnyAsync(a => a.SolicitudAscensoId == solicitudId &&
                              a.Estado == EstadoApelacion.Pendiente);
        }

        public async Task<IEnumerable<Apelacion>> GetApelacionesPorEstadoAsync(EstadoApelacion estado)
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.Estado == estado)
                .ToListAsync();
        }

        public async Task<IEnumerable<Apelacion>> GetApelacionesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.FechaCreacion >= fechaInicio && a.FechaCreacion <= fechaFin)
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Apelacion?> GetUltimaApelacionPorSolicitudAsync(Guid solicitudId)
        {
            return await _context.Apelaciones
                .Include(a => a.SolicitudAscenso)
                .Where(a => a.SolicitudAscensoId == solicitudId)
                .OrderByDescending(a => a.FechaCreacion)
                .FirstOrDefaultAsync();
        }
    }
}
