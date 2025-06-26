using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfRangoRepository : IRangoRepository
    {
        private readonly SigadDbContext _context;

        public EfRangoRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rango>> GetAllAsync()
        {
            return await _context.Rangos.AsNoTracking().ToListAsync();
        }

        public async Task<Rango?> GetByIdAsync(int id)
        {
            return await _context.Rangos.FindAsync(id);
        }

        // Métodos adicionales CRUD
        public async Task AddAsync(Rango rango)
        {
            await _context.Rangos.AddAsync(rango);
        }

        public async Task UpdateAsync(Rango rango)
        {
            _context.Rangos.Update(rango);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var rango = await _context.Rangos.FindAsync(id);
            if (rango != null)
            {
                _context.Rangos.Remove(rango);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rangos.AnyAsync(r => r.Id == id);
        }

        // Métodos específicos para validación de promoción
        public async Task<Rango?> GetByNombreAsync(string nombre)
        {
            return await _context.Rangos
                .FirstOrDefaultAsync(r => r.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<Rango?> GetRangoSiguienteAsync(int rangoActualId)
        {
            var rangoActual = await _context.Rangos.FindAsync(rangoActualId);
            if (rangoActual == null) return null;

            // Lógica de jerarquía: Auxiliar -> Agregado -> Principal
            // Asumiendo que el Id menor representa rango más alto
            return await _context.Rangos
                .Where(r => r.Id < rangoActualId) // Rango superior
                .OrderByDescending(r => r.Id) // El más cercano al actual
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Rango>> GetRangosDisponiblesParaPromocionAsync(int rangoActualId)
        {
            // Retorna rangos superiores al actual
            return await _context.Rangos
                .Where(r => r.Id < rangoActualId) // Rangos superiores
                .OrderBy(r => r.Id)
                .ToListAsync();
        }

        // Para validación de requisitos según el reglamento
        public async Task<bool> ValidarRequisitosArticulosAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var cantidadArticulos = await _context.Articulos
                .CountAsync(a => a.DocenteCedula == docenteCedula && a.EsVerificado);

            return cantidadArticulos >= rango.ArticulosRequeridos;
        }

        public async Task<bool> ValidarRequisitosExperienciaAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var experiencias = await _context.ExperienciasLaborales
                .Where(e => e.DocenteCedula == docenteCedula)
                .ToListAsync();

            var totalAnios = experiencias.Sum(e =>
            {
                var fechaFin = e.FechaFin ?? DateTime.Now;
                return (fechaFin - e.FechaInicio).Days / 365.0;
            });

            return totalAnios >= rango.AniosExperienciaRequeridos;
        }

        public async Task<bool> ValidarRequisitosCursosAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var totalHoras = await _context.Cursos
                .Where(c => c.DocenteCedula == docenteCedula)
                .SumAsync(c => c.NumeroHoras);

            return totalHoras >= rango.HorasCursoRequeridas;
        }

        public async Task<bool> ValidarRequisitosInvestigacionAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var totalMeses = await _context.Investigaciones
                .Where(i => i.DocenteCedula == docenteCedula)
                .SumAsync(i => i.MesesDeInvestigacion);

            return totalMeses >= rango.MesesInvestigacionRequeridos;
        }

        public async Task<bool> ValidarRequisitosTesisAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var cantidadTesis = await _context.TesisDirigidas
                .CountAsync(t => t.DocenteCedula == docenteCedula);

            return cantidadTesis >= rango.TesisDirigidasRequeridas;
        }

        public async Task<bool> ValidarRequisitosEvaluacionAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var rango = await _context.Rangos.FindAsync(rangoSolicitadoId);
            if (rango == null) return false;

            var evaluaciones = await _context.EvaluacionesDocentes
                .Where(e => e.DocenteCedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Take(4) // Últimas 4 evaluaciones según reglamento
                .ToListAsync();

            if (evaluaciones.Count < 4) return false;

            var promedio = evaluaciones.Average(e => e.PuntajePorcentual);
            return promedio >= rango.PuntajePromedioEvaluacionesRequerido;
        }

        // Método integral de validación
        public async Task<Dictionary<string, bool>> ValidarTodosRequisitosAsync(string docenteCedula, int rangoSolicitadoId)
        {
            var resultados = new Dictionary<string, bool>();

            resultados["Articulos"] = await ValidarRequisitosArticulosAsync(docenteCedula, rangoSolicitadoId);
            resultados["Experiencia"] = await ValidarRequisitosExperienciaAsync(docenteCedula, rangoSolicitadoId);
            resultados["Cursos"] = await ValidarRequisitosCursosAsync(docenteCedula, rangoSolicitadoId);
            resultados["Investigacion"] = await ValidarRequisitosInvestigacionAsync(docenteCedula, rangoSolicitadoId);
            resultados["Tesis"] = await ValidarRequisitosTesisAsync(docenteCedula, rangoSolicitadoId);
            resultados["Evaluaciones"] = await ValidarRequisitosEvaluacionAsync(docenteCedula, rangoSolicitadoId);

            return resultados;
        }
    }
}