using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IRangoRepository
    {
        // Métodos básicos - se implementarán más adelante
        Task<Rango?> GetByIdAsync(int id);
        Task<IEnumerable<Rango>> GetAllAsync();

        // Métodos adicionales CRUD
        Task AddAsync(Rango rango);
        Task UpdateAsync(Rango rango);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Métodos específicos para validación de promoción
        Task<Rango?> GetByNombreAsync(string nombre);
        Task<Rango?> GetRangoSiguienteAsync(int rangoActualId);
        Task<IEnumerable<Rango>> GetRangosDisponiblesParaPromocionAsync(int rangoActualId);

        // Para validación de requisitos según el reglamento
        Task<bool> ValidarRequisitosArticulosAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarRequisitosExperienciaAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarRequisitosCursosAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarRequisitosInvestigacionAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarRequisitosTesisAsync(string docenteCedula, int rangoSolicitadoId);
        Task<bool> ValidarRequisitosEvaluacionAsync(string docenteCedula, int rangoSolicitadoId);

        // Método integral de validación
        Task<Dictionary<string, bool>> ValidarTodosRequisitosAsync(string docenteCedula, int rangoSolicitadoId);
    }
}