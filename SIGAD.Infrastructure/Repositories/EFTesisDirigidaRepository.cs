using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfTesisDirigidaRepository : ITesisDirigidaRepository
    {
        private readonly SigadDbContext _context;

        public EfTesisDirigidaRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TesisDirigida>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .Where(t => t.DocenteCedula == docenteCedula)
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TesisDirigida>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.TesisPorSolicitud
                .Where(tps => tps.SolicitudId == solicitudId)
                .Include(tps => tps.TesisDirigida)
                .ThenInclude(t => t!.Docente)
                .Select(tps => tps.TesisDirigida!)
                .ToListAsync();
        }

        public async Task<TesisDirigida?> GetByIdAsync(int id)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TesisDirigida tesis)
        {
            await _context.TesisDirigidas.AddAsync(tesis);
        }

        public async Task UpdateAsync(TesisDirigida tesis)
        {
            _context.TesisDirigidas.Update(tesis);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var tesis = await _context.TesisDirigidas.FindAsync(id);
            if (tesis != null)
            {
                _context.TesisDirigidas.Remove(tesis);
            }
        }

        public async Task<bool> ExistsByHashAsync(string hash)
        {
            return await _context.TesisDirigidas.AnyAsync(t => t.ContenidoHash == hash);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int tesisId)
        {
            var relacion = new TesisPorSolicitud
            {
                SolicitudId = solicitudId,
                TesisDirigidaId = tesisId
            };

            await _context.TesisPorSolicitud.AddAsync(relacion);
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int tesisId)
        {
            var relacion = await _context.TesisPorSolicitud
                .FirstOrDefaultAsync(tps => tps.SolicitudId == solicitudId && tps.TesisDirigidaId == tesisId);

            if (relacion != null)
            {
                _context.TesisPorSolicitud.Remove(relacion);
            }
        }

        // Métodos específicos para el reglamento de promoción
        public async Task<int> GetCantidadTesisDirigidasAsync(string docenteCedula, string? nivelAcademico = null)
        {
            var query = _context.TesisDirigidas
                .Where(t => t.DocenteCedula == docenteCedula);

            if (!string.IsNullOrEmpty(nivelAcademico))
            {
                // Convertir string a enum para comparación
                if (Enum.TryParse<NivelAcademico>(nivelAcademico, true, out var nivel))
                {
                    query = query.Where(t => t.NivelAcademico == nivel);
                }
            }

            return await query.CountAsync();
        }
        public async Task<IEnumerable<TesisDirigida>> GetTesisActivasAsync(string docenteCedula)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .Where(t => t.DocenteCedula == docenteCedula &&
                           (t.Estado == EstadoTesis.EnProceso || t.Estado == EstadoTesis.AprobadaPendienteDefensa))
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<TesisDirigida>> GetTesisByNivelAsync(string docenteCedula, string nivelAcademico)
        {
            if (Enum.TryParse<NivelAcademico>(nivelAcademico, true, out var nivel))
            {
                return await _context.TesisDirigidas
                    .Include(t => t.Docente)
                    .Where(t => t.DocenteCedula == docenteCedula && t.NivelAcademico == nivel)
                    .OrderByDescending(t => t.FechaInicio)
                    .ToListAsync();
            }

            return new List<TesisDirigida>();
        }

        // Para validación de requisitos específicos del reglamento
        public async Task<int> GetCantidadTesisDoctoradoAsync(string docenteCedula)
        {
            return await _context.TesisDirigidas
                .CountAsync(t => t.DocenteCedula == docenteCedula &&
                                t.NivelAcademico == NivelAcademico.Doctorado);
        }

        public async Task<bool> CumpleRequisitoTesisParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false; var cantidadTesis = await _context.TesisDirigidas
                .CountAsync(t => t.DocenteCedula == docenteCedula &&
                                t.Estado == EstadoTesis.Culminada);

            return cantidadTesis >= rango.TesisDirigidasRequeridas;
        }

        // Para reportes y estadísticas
        public async Task<IEnumerable<TesisDirigida>> GetTesisByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.TesisDirigidas
                .Include(t => t.Docente)
                .Where(t => t.DocenteCedula == docenteCedula &&
                           t.FechaInicio >= fechaInicio &&
                           (t.FechaFin == null || t.FechaFin <= fechaFin))
                .OrderByDescending(t => t.FechaInicio)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetEstadisticasPorNivelAsync(string docenteCedula)
        {
            var tesis = await _context.TesisDirigidas
                .Where(t => t.DocenteCedula == docenteCedula)
                .ToListAsync();

            var estadisticas = new Dictionary<string, int>();

            foreach (NivelAcademico nivel in Enum.GetValues<NivelAcademico>())
            {
                var cantidad = tesis.Count(t => t.NivelAcademico == nivel);
                estadisticas[nivel.ToString()] = cantidad;
            }

            return estadisticas;
        }
    }
}
