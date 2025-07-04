using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfInvestigacionRepository : IInvestigacionRepository
    {
        private readonly SigadDbContext _context;

        public EfInvestigacionRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Investigacion>> GetAllAsync()
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .ToListAsync();
        }

        public async Task<Investigacion?> GetByIdAsync(int id)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Investigacion>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .Include(i => i.InvestigacionesPorSolicitud!)
                    .ThenInclude(ips => ips.SolicitudAscenso)
                .Where(i => i.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<Investigacion>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.InvestigacionesPorSolicitud
                .Include(ips => ips.Investigacion)
                    .ThenInclude(i => i.Docente)
                .Where(ips => ips.SolicitudId == solicitudId)
                .Select(ips => ips.Investigacion)
                .ToListAsync();
        }

        public async Task AddAsync(Investigacion investigacion)
        {
            await _context.Investigaciones.AddAsync(investigacion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Investigacion investigacion)
        {
            _context.Investigaciones.Update(investigacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var investigacion = await GetByIdAsync(id);
            if (investigacion != null)
            {
                _context.Investigaciones.Remove(investigacion);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Investigaciones.AnyAsync(i => i.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int investigacionId)
        {
            // Verificar si la asociación ya existe
            var existeAsociacion = await _context.InvestigacionesPorSolicitud
                .AnyAsync(ips => ips.SolicitudId == solicitudId && ips.InvestigacionId == investigacionId);

            if (existeAsociacion)
            {
                // La asociación ya existe, no hacer nada
                return;
            }

            var investigacionPorSolicitud = new InvestigacionesPorSolicitud
            {
                SolicitudId = solicitudId,
                InvestigacionId = investigacionId
            };

            await _context.InvestigacionesPorSolicitud.AddAsync(investigacionPorSolicitud);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int investigacionId)
        {
            var investigacionPorSolicitud = await _context.InvestigacionesPorSolicitud
                .FirstOrDefaultAsync(ips => ips.SolicitudId == solicitudId && ips.InvestigacionId == investigacionId);

            if (investigacionPorSolicitud != null)
            {
                _context.InvestigacionesPorSolicitud.Remove(investigacionPorSolicitud);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _context.Investigaciones.AnyAsync(i => i.ContenidoHash == hash);
        }

        public async Task AgregarAsync(Investigacion investigacion)
        {
            await _context.Investigaciones.AddAsync(investigacion);
        }

        // Métodos específicos para el reglamento de promoción
        public async Task<int> GetTotalMesesInvestigacionAsync(string docenteCedula)
        {
            var investigaciones = await _context.Investigaciones
                .Where(i => i.DocenteCedula == docenteCedula)
                .ToListAsync();

            return investigaciones.Sum(i => i.MesesDeInvestigacion);
        }
        public async Task<int> GetMesesInvestigacionEnPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin)
        {
            var investigaciones = await _context.Investigaciones
                .Where(i => i.DocenteCedula == docenteCedula &&
                           i.FechaInicio >= fechaInicio &&
                           i.FechaFinalizacion <= fechaFin)
                .ToListAsync();

            return investigaciones.Sum(i => i.MesesDeParticipacion > 0 ? i.MesesDeParticipacion : i.MesesDeInvestigacion);
        }

        public async Task<bool> CumpleRequisitoInvestigacionParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            // Obtener el rango solicitado para ver los meses requeridos
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var totalMeses = await GetTotalMesesInvestigacionAsync(docenteCedula);
            return totalMeses >= rango.MesesInvestigacionRequeridos;
        }

        public async Task<IEnumerable<Investigacion>> GetInvestigacionesConFilacionUTAAsync(string docenteCedula)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .Where(i => i.DocenteCedula == docenteCedula &&
                           (i.Titulo.Contains("UTA") ||
                            i.Titulo.Contains("Universidad Técnica de Ambato") ||
                            i.UnidadVerificadora != null && i.UnidadVerificadora.Contains("UTA")))
                .OrderByDescending(i => i.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Investigacion>> GetInvestigacionesComoCoordinadorAsync(string docenteCedula)
        {
            return await _context.Investigaciones
                .Include(i => i.Docente)
                .Where(i => i.DocenteCedula == docenteCedula &&
                           (i.RolEnInvestigacion.ToLower().Contains("coordinador") ||
                            i.RolEnInvestigacion.ToLower().Contains("director") ||
                            i.RolEnInvestigacion.ToLower().Contains("líder")))
                .OrderByDescending(i => i.FechaInicio)
                .ToListAsync();
        }

        public async Task<decimal> CalcularTiempoEquivalenteCoordinacionAsync(string docenteCedula)
        {
            var investigacionesCoordinador = await GetInvestigacionesComoCoordinadorAsync(docenteCedula);

            decimal tiempoEquivalente = 0; foreach (var investigacion in investigacionesCoordinador)
            {
                // Según reglamento UTA: coordinación de proyecto equivale a tiempo específico
                // Por ahora, usamos una fórmula básica: 1.5x los meses de participación
                var mesesParticipacion = investigacion.MesesDeParticipacion > 0 ?
                    investigacion.MesesDeParticipacion : investigacion.MesesDeInvestigacion;
                tiempoEquivalente += mesesParticipacion * 1.5m;
            }

            return tiempoEquivalente;
        }
    }
}
