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
                .Include(a => a.ArticulosPorSolicitud!)
                    .ThenInclude(aps => aps.SolicitudAscenso)
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
            Console.WriteLine($"[REPOSITORY] AddToSolicitudAsync - SolicitudId: {solicitudId}, ArticuloDOI: '{articuloDoi}'");
            
            // Verificar si ya existe la asociación
            var existeAsociacion = await _context.ArticulosPorSolicitud
                .AnyAsync(aps => aps.SolicitudId == solicitudId && aps.ArticuloDOI == articuloDoi);
            
            Console.WriteLine($"[REPOSITORY] Ya existe asociación: {existeAsociacion}");
            
            if (existeAsociacion)
            {
                Console.WriteLine($"[REPOSITORY] Asociación ya existe, saltando...");
                return;
            }

            var articuloPorSolicitud = new ArticulosPorSolicitud
            {
                SolicitudId = solicitudId,
                ArticuloDOI = articuloDoi
            };

            await _context.ArticulosPorSolicitud.AddAsync(articuloPorSolicitud);
            Console.WriteLine($"[REPOSITORY] Asociación agregada al contexto");
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, string articuloDoi)
        {
            // Intentar encontrar con el GUID normal
            var articuloPorSolicitud = await _context.ArticulosPorSolicitud
                .FirstOrDefaultAsync(aps => aps.SolicitudId == solicitudId && aps.ArticuloDOI == articuloDoi);
            
            // Si no se encuentra, intentar con el GUID sin guiones (por si acaso)
            if (articuloPorSolicitud == null)
            {
                var solicitudIdSinGuiones = solicitudId.ToString("N"); // Formato sin guiones
                
                // Como no podemos convertir string a GUID directamente en LINQ, buscamos todos y comparamos en memoria
                var todosLosRegistros = await _context.ArticulosPorSolicitud
                    .Where(aps => aps.ArticuloDOI == articuloDoi)
                    .ToListAsync();
                
                articuloPorSolicitud = todosLosRegistros
                    .FirstOrDefault(aps => aps.SolicitudId.ToString("N").Equals(solicitudIdSinGuiones, StringComparison.OrdinalIgnoreCase) ||
                                          aps.SolicitudId.ToString().Equals(solicitudId.ToString(), StringComparison.OrdinalIgnoreCase));
            }
            
            if (articuloPorSolicitud != null)
            {
                _context.ArticulosPorSolicitud.Remove(articuloPorSolicitud);
                await _context.SaveChangesAsync();
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

        // Métodos específicos para el reglamento de promoción
        public async Task<int> GetCantidadArticulosVerificadosAsync(string docenteCedula)
        {
            return await _context.Articulos
                .Where(a => a.DocenteCedula == docenteCedula && a.EsVerificado)
                .CountAsync();
        }

        public async Task<IEnumerable<Articulo>> GetArticulosVerificadosAsync(string docenteCedula)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => a.DocenteCedula == docenteCedula && a.EsVerificado)
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPendientesVerificacionAsync(string docenteCedula)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => a.DocenteCedula == docenteCedula && !a.EsVerificado && string.IsNullOrEmpty(a.ObservacionesVerificacion))
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }

        // Para validación de obras relevantes según reglamento
        public async Task<bool> CumpleRequisitoArticulosParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            // TODO: Implementar lógica específica según reglamento UTA
            // Por ahora, validación básica de cantidad de artículos verificados
            var cantidadArticulos = await GetCantidadArticulosVerificadosAsync(docenteCedula);

            // Lógica básica: más artículos requeridos para rangos superiores
            return rangoSolicitadoId switch
            {
                1 => cantidadArticulos >= 2, // Profesor Principal
                2 => cantidadArticulos >= 1, // Profesor Agregado
                3 => cantidadArticulos >= 0, // Profesor Auxiliar
                _ => false
            };
        }

        public async Task<IEnumerable<Articulo>> GetArticulosByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => a.DocenteCedula == docenteCedula &&
                           a.AnioPublicacion >= fechaInicio.Year &&
                           a.AnioPublicacion <= fechaFin.Year)
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }

        // Para verificación institucional (DIDE, DINNOVA, COMITÉ EDITORIAL)
        public async Task VerificarArticuloAsync(string doi, string unidadVerificadora)
        {
            var articulo = await _context.Articulos.FindAsync(doi);
            if (articulo != null)
            {
                articulo.EsVerificado = true;
                articulo.UnidadVerificadora = unidadVerificadora;
                articulo.FechaVerificacion = DateTime.UtcNow;
                articulo.ObservacionesVerificacion = null; // Limpiar observaciones previas
                _context.Articulos.Update(articulo);
            }
        }

        public async Task RechazarVerificacionAsync(string doi, string observaciones)
        {
            var articulo = await _context.Articulos.FindAsync(doi);
            if (articulo != null)
            {
                articulo.EsVerificado = false;
                articulo.ObservacionesVerificacion = observaciones;
                articulo.FechaVerificacion = DateTime.UtcNow;
                _context.Articulos.Update(articulo);
            }
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorVerificarAsync()
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => !a.EsVerificado && string.IsNullOrEmpty(a.ObservacionesVerificacion))
                .OrderBy(a => a.FechaCreacion)
                .ToListAsync();
        }

        // Para indexación y relevancia
        public async Task<IEnumerable<Articulo>> GetArticulosIndexadosAsync(string docenteCedula)
        {
            return await _context.Articulos
                .Include(a => a.Docente)
                .Where(a => a.DocenteCedula == docenteCedula && a.EsIndexado)
                .OrderByDescending(a => a.AnioPublicacion)
                .ToListAsync();
        }
        public async Task<bool> EsArticuloRelevante(string doi)
        {
            var articulo = await _context.Articulos.FindAsync(doi);
            if (articulo == null) return false;

            // TODO: Implementar lógica específica de relevancia según reglamento UTA
            // Por ahora, consideramos relevante si está indexado y verificado
            return articulo.EsIndexado && articulo.EsVerificado;
        }
    }
}