using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IAccionesDePersonalRepository
    {
        // Métodos básicos CRUD
        Task<AccionesDePersonal?> GetByIdAsync(int id);
        Task<IEnumerable<AccionesDePersonal>> GetAllAsync();
        Task AddAsync(AccionesDePersonal accion);
        Task UpdateAsync(AccionesDePersonal accion);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Métodos específicos para el docente
        Task<IEnumerable<AccionesDePersonal>> GetByDocenteAsync(string docenteCedula);
        Task<IEnumerable<AccionesDePersonal>> GetBySolicitudIdAsync(Guid solicitudId);

        // Para validación de requisitos del reglamento (Art. 7 - Excepcionalidades)
        Task<bool> TieneExperienciaAdministrativaAsync(string docenteCedula, int mesesMinimos);
        Task<IEnumerable<AccionesDePersonal>> GetExperienciaAdministrativaAsync(string docenteCedula);
        Task<int> GetMesesExperienciaAdministrativaAsync(string docenteCedula);

        // Para cargos específicos mencionados en el reglamento
        Task<bool> HasEjercidoComoAutoridadAsync(string docenteCedula, int aniosMinimos);
        Task<IEnumerable<AccionesDePersonal>> GetCargosAutoridadAsync(string docenteCedula);

        // Métodos para relación con solicitudes
        Task AddToSolicitudAsync(Guid solicitudId, int accionId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int accionId);
        Task<bool> ExistsByHashAsync(string hash);

        // Para reportes y estadísticas
        Task<IEnumerable<AccionesDePersonal>> GetByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin);
        Task<Dictionary<string, int>> GetEstadisticasPorTipoAsync(string docenteCedula);
    }
}
