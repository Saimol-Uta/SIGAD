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
    public class EfSolicitudAscensoRepository : ISolicitudAscensoRepository
    {
        private readonly SigadDbContext _context;

        public EfSolicitudAscensoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SolicitudAscenso solicitud)
        {
            await _context.SolicitudesAscenso.AddAsync(solicitud);
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllAsync()
        {
            return await _context.SolicitudesAscenso.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync()
        {
            return await _context.SolicitudesAscenso
                .AsNoTracking() 
                .Include(s => s.Docente)          
                .Include(s => s.RangoActual)      
                .Include(s => s.RangoSolicitado)  
                .ToListAsync();
        }
        public async Task<SolicitudAscenso?> GetByIdAsync(Guid id)
        {
            return await _context.SolicitudesAscenso.FindAsync(id);
        }
        public Task UpdateAsync(SolicitudAscenso solicitud)
        {
            // No es un método asíncrono porque solo cambia el estado del objeto en memoria.
            // La operación de guardado real (I/O) la hará el UnitOfWork.
            _context.Entry(solicitud).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}
