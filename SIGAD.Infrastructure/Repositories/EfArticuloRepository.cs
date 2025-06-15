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
            return await _context.Articulos
                .Include(a => a.Docente)
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }

        public async Task<Articulo?> GetByIdAsync(string doi)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .FirstOrDefaultAsync(a => a.DOI == doi);
        }

        public async Task<IEnumerable<Articulo>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => a.DocenteCedula == docenteCedula)
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.ArticulosPorSolicitud
                .Where(aps => aps.SolicitudId == solicitudId)
                .Include(aps => aps.Articulo)
                .ThenInclude(a => a!.Docente)
                .Select(aps => aps.Articulo!)
                .ToListAsync();
        }

        public async Task AddAsync(Articulo articulo)
        {
            await _context.Articulos.AddAsync(articulo);
        }

        public async Task UpdateAsync(Articulo articulo)
        {
            _context.Articulos.Update(articulo);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string doi)
        {
            var articulo = await _context.Articulos.FindAsync(doi);
            if (articulo != null)
            {
                _context.Articulos.Remove(articulo);
            }
        }

        public async Task<bool> ExistsAsync(string doi)
        {
            return await _context.Articulos.AnyAsync(a => a.DOI == doi);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, string articuloDoi)
        {
            var articuloPorSolicitud = new ArticulosPorSolicitud
            {
                SolicitudId = solicitudId,
                ArticuloDOI = articuloDoi
            };

            await _context.ArticulosPorSolicitud.AddAsync(articuloPorSolicitud);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, string articuloDoi)
        {
            var articuloPorSolicitud = await _context.ArticulosPorSolicitud
                .FirstOrDefaultAsync(aps => aps.SolicitudId == solicitudId && aps.ArticuloDOI == articuloDoi);

            if (articuloPorSolicitud != null)
            {
                _context.ArticulosPorSolicitud.Remove(articuloPorSolicitud);
            }
        }
        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _context.Articulos.AnyAsync(a => a.ContenidoHash == hash);
        }

        public async Task AgregarAsync(Articulo articulo)
        {
            await _context.Articulos.AddAsync(articulo);
        }
    }
}