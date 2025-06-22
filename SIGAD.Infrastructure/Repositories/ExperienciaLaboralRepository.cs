using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class ExperienciaLaboralRepository : IExperienciaLaboralRepository
    {
        private readonly SigadDbContext _context;

        public ExperienciaLaboralRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetAllAsync()
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .ToListAsync();
        }

        public async Task<ExperienciaLaboral?> GetByIdAsync(int id)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.ExperienciasPorSolicitud
                .Include(eps => eps.ExperienciaLaboral)
                    .ThenInclude(e => e.Organizacion)
                .Include(eps => eps.ExperienciaLaboral)
                    .ThenInclude(e => e.Docente)
                .Where(eps => eps.SolicitudId == solicitudId)
                .Select(eps => eps.ExperienciaLaboral)
                .ToListAsync();
        }

        public async Task AddAsync(ExperienciaLaboral experiencia)
        {
            await _context.ExperienciasLaborales.AddAsync(experiencia);
        }
        public async Task UpdateAsync(ExperienciaLaboral experiencia)
        {
            _context.ExperienciasLaborales.Update(experiencia);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var experiencia = await _context.ExperienciasLaborales.FindAsync(id);
            if (experiencia != null)
            {
                _context.ExperienciasLaborales.Remove(experiencia);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ExperienciasLaborales.AnyAsync(e => e.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int experienciaId)
        {
            var experienciaPorSolicitud = new ExperienciaPorSolicitud
            {
                SolicitudId = solicitudId,
                ExperienciaId = experienciaId
            };

            await _context.ExperienciasPorSolicitud.AddAsync(experienciaPorSolicitud);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int experienciaId)
        {
            var experienciaPorSolicitud = await _context.ExperienciasPorSolicitud
                .FirstOrDefaultAsync(eps => eps.SolicitudId == solicitudId && eps.ExperienciaId == experienciaId);

            if (experienciaPorSolicitud != null)
            {
                _context.ExperienciasPorSolicitud.Remove(experienciaPorSolicitud);
            }
        }
        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _context.ExperienciasLaborales.AnyAsync(e => e.ContenidoHash == hash);
        }

        public async Task AgregarAsync(ExperienciaLaboral experiencia)
        {
            await _context.ExperienciasLaborales.AddAsync(experiencia);
        }

        // Métodos específicos del reglamento de promoción
        public async Task<int> GetAniosExperienciaDocenteAsync(string docenteCedula)
        {
            var experiencias = await _context.ExperienciasLaborales
                .Where(e => e.DocenteCedula == docenteCedula)
                .ToListAsync();

            var totalAnios = 0.0;
            foreach (var exp in experiencias)
            {
                var fechaFin = exp.FechaFin ?? DateTime.Now;
                var anios = (fechaFin - exp.FechaInicio).TotalDays / 365.25;
                totalAnios += anios;
            }

            return (int)Math.Floor(totalAnios);
        }

        public async Task<int> GetAniosExperienciaEnUTAAsync(string docenteCedula)
        {
            var experienciasUTA = await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Where(e => e.DocenteCedula == docenteCedula &&
                           (e.Organizacion.Nombre.ToLower().Contains("uta") ||
                            e.Organizacion.Nombre.ToLower().Contains("universidad técnica de ambato") ||
                            e.Organizacion.Nombre.ToLower().Contains("universidad tecnica de ambato")))
                .ToListAsync();

            var totalAnios = 0.0;
            foreach (var exp in experienciasUTA)
            {
                var fechaFin = exp.FechaFin ?? DateTime.Now;
                var anios = (fechaFin - exp.FechaInicio).TotalDays / 365.25;
                totalAnios += anios;
            }

            return (int)Math.Floor(totalAnios);
        }

        public async Task<bool> CumpleRequisitoExperienciaParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var aniosExperiencia = await GetAniosExperienciaDocenteAsync(docenteCedula);
            return aniosExperiencia >= rango.AniosExperienciaRequeridos;
        }

        public async Task<IEnumerable<ExperienciaLaboral>> GetExperienciaAcademicaAsync(string docenteCedula)
        {
            return await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Include(e => e.Docente)
                .Where(e => e.DocenteCedula == docenteCedula &&
                           (e.Cargo.ToLower().Contains("docente") ||
                            e.Cargo.ToLower().Contains("profesor") ||
                            e.Cargo.ToLower().Contains("investigador") ||
                            e.Cargo.ToLower().Contains("catedrático") ||
                            e.Organizacion.TipoOrganizacion.ToLower().Contains("universidad") ||
                            e.Organizacion.TipoOrganizacion.ToLower().Contains("instituto") ||
                            e.Organizacion.TipoOrganizacion.ToLower().Contains("educación")))
                .OrderBy(e => e.FechaInicio)
                .ToListAsync();
        }

        public async Task<DateTime?> GetFechaInicioEnUTAAsync(string docenteCedula)
        {
            var primeraExperienciaUTA = await _context.ExperienciasLaborales
                .Include(e => e.Organizacion)
                .Where(e => e.DocenteCedula == docenteCedula &&
                           (e.Organizacion.Nombre.ToLower().Contains("uta") ||
                            e.Organizacion.Nombre.ToLower().Contains("universidad técnica de ambato") ||
                            e.Organizacion.Nombre.ToLower().Contains("universidad tecnica de ambato")))
                .OrderBy(e => e.FechaInicio)
                .FirstOrDefaultAsync();

            return primeraExperienciaUTA?.FechaInicio;
        }
    }
}