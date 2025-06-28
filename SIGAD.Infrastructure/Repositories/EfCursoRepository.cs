using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfCursoRepository : ICursoRepository
    {
        private readonly SigadDbContext _context;

        public EfCursoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .ToListAsync();
        }

        public async Task<Curso?> GetByIdAsync(int id)
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> GetByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .Where(c => c.DocenteCedula == docenteCedula)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetBySolicitudIdAsync(Guid solicitudId)
        {
            return await _context.CursosPorSolicitud
                .Include(cps => cps.Curso)
                    .ThenInclude(c => c.Docente)
                .Include(cps => cps.Curso)
                    .ThenInclude(c => c.Organizacion)
                .Where(cps => cps.SolicitudId == solicitudId)
                .Select(cps => cps.Curso)
                .ToListAsync();
        }

        public async Task AddAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Curso curso)
        {
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var curso = await GetByIdAsync(id);
            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Cursos.AnyAsync(c => c.Id == id);
        }

        public async Task AddToSolicitudAsync(Guid solicitudId, int cursoId)
        {
            var cursoPorSolicitud = new CursosPorSolicitud
            {
                SolicitudId = solicitudId,
                CursoId = cursoId
            };

            await _context.CursosPorSolicitud.AddAsync(cursoPorSolicitud);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromSolicitudAsync(Guid solicitudId, int cursoId)
        {
            var cursoPorSolicitud = await _context.CursosPorSolicitud
                .FirstOrDefaultAsync(cps => cps.SolicitudId == solicitudId && cps.CursoId == cursoId);

            if (cursoPorSolicitud != null)
            {
                _context.CursosPorSolicitud.Remove(cursoPorSolicitud);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _context.Cursos.AnyAsync(c => c.ContenidoHash == hash);
        }

        public async Task AgregarAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
        }

        // ✅ MÉTODOS ESPECÍFICOS PARA EL REGLAMENTO DE PROMOCIÓN

        public async Task<int> GetTotalHorasCapacitacionAsync(string docenteCedula, int ultimosAnios = 3)
        {
            var fechaLimite = DateTime.Now.AddYears(-ultimosAnios);

            return await _context.Cursos
                .Where(c => c.DocenteCedula == docenteCedula &&
                           c.FechaFinalizacion >= fechaLimite)
                .SumAsync(c => c.NumeroHoras);
        }

        public async Task<int> GetHorasActualizacionPedagogicaAsync(string docenteCedula, int ultimosAnios = 3)
        {
            var fechaLimite = DateTime.Now.AddYears(-ultimosAnios);

            return await _context.Cursos
                .Where(c => c.DocenteCedula == docenteCedula &&
                           c.FechaFinalizacion >= fechaLimite &&
                           c.TipoCurso == TipoCurso.ActualizacionPedagogica)
                .SumAsync(c => c.NumeroHoras);
        }

        public async Task<int> GetHorasActualizacionCientificaAsync(string docenteCedula, int ultimosAnios = 3)
        {
            var fechaLimite = DateTime.Now.AddYears(-ultimosAnios);

            return await _context.Cursos
                .Where(c => c.DocenteCedula == docenteCedula &&
                           c.FechaFinalizacion >= fechaLimite &&
                           c.TipoCurso == TipoCurso.ActualizacionCientifica)
                .SumAsync(c => c.NumeroHoras);
        }

        public async Task<bool> CumpleRequisitoHorasParaRangoAsync(string docenteCedula, int rangoSolicitadoId)
        {
            // Obtener el rango y sus requisitos
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var totalHoras = await GetTotalHorasCapacitacionAsync(docenteCedula);
            var horasPedagogicas = await GetHorasActualizacionPedagogicaAsync(docenteCedula);

            // Verificar que cumple el total y el 25% mínimo de pedagógicas
            var cumpleTotal = totalHoras >= rango.HorasCursoRequeridas;
            var cumplePedagogicas = horasPedagogicas >= (rango.HorasCursoRequeridas * 0.25);

            return cumpleTotal && cumplePedagogicas;
        }

        public async Task<IEnumerable<Curso>> GetCursosByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .Where(c => c.DocenteCedula == docenteCedula &&
                           c.FechaFinalizacion >= fechaInicio &&
                           c.FechaFinalizacion <= fechaFin)
                .OrderByDescending(c => c.FechaFinalizacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetCursosByTipoAsync(string docenteCedula, string tipoCurso)
        {
            // Convertir string a enum para compatibilidad con interfaces existentes
            if (!Enum.TryParse<TipoCurso>(tipoCurso, out var tipoEnum))
            {
                return new List<Curso>();
            }

            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .Where(c => c.DocenteCedula == docenteCedula && c.TipoCurso == tipoEnum)
                .OrderByDescending(c => c.FechaFinalizacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> GetCursosInstitucionesAcreditadasAsync(string docenteCedula)
        {
            // Lista de instituciones acreditadas (esto podría venir de una tabla de configuración)
            var institucionesAcreditadas = new[]
            {
                "Universidad Técnica de Ambato",
                "Universidad Central del Ecuador",
                "Escuela Politécnica Nacional",
                "Universidad San Francisco de Quito",
                "ESPOL"
                // Se pueden agregar más instituciones desde base de datos
            };

            return await _context.Cursos
                .Include(c => c.Docente)
                .Include(c => c.Organizacion)
                .Where(c => c.DocenteCedula == docenteCedula &&
                           institucionesAcreditadas.Contains(c.Organizacion.Nombre))
                .OrderByDescending(c => c.FechaFinalizacion)
                .ToListAsync();
        }

        public async Task<bool> EsInstitucionAcreditadaAsync(string institucion)
        {
            // Validar si una institución está acreditada
            var institucionesAcreditadas = new[]
            {
                "Universidad Técnica de Ambato",
                "Universidad Central del Ecuador",
                "Escuela Politécnica Nacional",
                "Universidad San Francisco de Quito",
                "ESPOL",
                "Pontificia Universidad Católica del Ecuador",
                "Universidad de las Fuerzas Armadas ESPE"
            };

            return await Task.FromResult(institucionesAcreditadas.Contains(institucion));
        }

        public async Task<int> GetHorasEquivalenciasFacilitacionAsync(string docenteCedula)
        {
            // Buscar cursos impartidos por el docente (equivalencias del Art. 3)
            return await _context.Cursos
                .Where(c => c.DocenteCedula == docenteCedula &&
                           c.ImpartidoPorDocente &&
                           c.TipoCurso == TipoCurso.CapacitacionImpartida)
                .SumAsync(c => c.NumeroHoras);
        }

        public async Task RegistrarEquivalenciaFacilitacionAsync(string docenteCedula, string tipoFacilitacion, int horasEquivalentes)
        {
            // Crear un curso equivalente por facilitación externa
            var cursoEquivalente = new Curso
            {
                DocenteCedula = docenteCedula,
                Nombre = $"Equivalencia - {tipoFacilitacion}",
                NumeroHoras = horasEquivalentes,
                TipoCurso = TipoCurso.CapacitacionImpartida,
                ImpartidoPorDocente = true,
                FechaFinalizacion = DateTime.Now,
                CertificadoRuta = $"Equivalencia_{tipoFacilitacion}_{DateTime.Now:yyyyMMdd}",
                ContenidoHash = $"equiv_{docenteCedula}_{DateTime.Now.Ticks}",
                OrganizacionId = 1 // Asumir UTA como organización por defecto
            }; await _context.Cursos.AddAsync(cursoEquivalente); await _context.SaveChangesAsync();
        }
    }
}