using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfDocenteRepository : IDocenteRepository
    {
        private readonly SigadDbContext _context; public async Task<Docente?> GetByIdWithDetailsAsync(string cedula)
        {
            return await _context.Docentes
                .Include(d => d.RangoActual)
                .Include(d => d.Cuenta)
                .Include(d => d.Articulos)
                .Include(d => d.Cursos)
                    .ThenInclude(c => c.Organizacion)
                .Include(d => d.Investigaciones)
                .Include(d => d.TesisDirigidas)
                .Include(d => d.ExperienciasLaborales)
                    .ThenInclude(e => e.Organizacion)
                .Include(d => d.Evaluaciones)
                .Include(d => d.AccionesDePersonal)
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }

        public EfDocenteRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<Docente?> GetByCedulaAsync(string cedula)
        {
            return await _context.Docentes
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            return await _context.Docentes
                .AnyAsync(d => d.Cedula == cedula);
        }

        public async Task AddAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
        }

        public async Task UpdateAsync(Docente docente)
        {
            _context.Docentes.Update(docente);
            await Task.CompletedTask;
        }
        public async Task<Docente?> ObtenerPorCedulaAsync(string cedula)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Cedula == cedula);
        }
        public async Task AgregarAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
        }

        // Métodos específicos requeridos por la interfaz
        public async Task<IEnumerable<Docente>> GetAllAsync()
        {
            return await _context.Docentes
                .Include(d => d.RangoActual)
                .Include(d => d.Cuenta)
                .OrderBy(d => d.Apellido1)
                .ThenBy(d => d.Apellido2)
                .ThenBy(d => d.Nombre1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Docente>> GetByRangoAsync(int rangoId)
        {
            return await _context.Docentes
                .Include(d => d.RangoActual)
                .Include(d => d.Cuenta)
                .Where(d => d.RangoActualId == rangoId)
                .OrderBy(d => d.Apellido1)
                .ThenBy(d => d.Apellido2)
                .ThenBy(d => d.Nombre1)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string cedula)
        {
            return await _context.Docentes.AnyAsync(d => d.Cedula == cedula);
        }

        public async Task DeleteAsync(string cedula)
        {
            var docente = await _context.Docentes.FindAsync(cedula);
            if (docente != null)
            {
                _context.Docentes.Remove(docente);
            }
        }

        public async Task<IEnumerable<Docente>> GetDocentesElegiblesPromocionAsync()
        {
            // TODO: Implementar lógica específica según reglamento UTA
            // Por ahora, retornamos docentes que tengan al menos un artículo y una evaluación
            return await _context.Docentes
                .Include(d => d.RangoActual)
                .Include(d => d.Cuenta)
                .Include(d => d.Articulos)
                .Include(d => d.Evaluaciones)
                .Where(d => d.Articulos.Any() && d.Evaluaciones.Any())
                .OrderBy(d => d.Apellido1)
                .ThenBy(d => d.Apellido2)
                .ToListAsync();
        }

        public async Task<Docente?> GetWithSolicitudesAsync(string cedula)
        {
            return await _context.Docentes
                .Include(d => d.RangoActual)
                .Include(d => d.Cuenta)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.RangoSolicitado)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.ArticulosPorSolicitud)
                        .ThenInclude(aps => aps.Articulo)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.CursosPorSolicitud)
                        .ThenInclude(cps => cps.Curso)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.InvestigacionesPorSolicitud)
                        .ThenInclude(ips => ips.Investigacion)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.TesisPorSolicitud)
                        .ThenInclude(tps => tps.TesisDirigida)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.EvaluacionesPorSolicitud)
                        .ThenInclude(eps => eps.Evaluacion)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.ExperienciaPorSolicitud)
                        .ThenInclude(exps => exps.ExperienciaLaboral)
                .Include(d => d.Solicitudes)
                    .ThenInclude(s => s.AccionesDePersonalPorSolicitud)
                        .ThenInclude(apps => apps.AccionDePersonal)
                .FirstOrDefaultAsync(d => d.Cedula == cedula);
        }
    }
}