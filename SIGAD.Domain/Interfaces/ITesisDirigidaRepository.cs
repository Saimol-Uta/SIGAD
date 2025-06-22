using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ITesisDirigidaRepository
    {
        Task<IEnumerable<TesisDirigida>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<TesisDirigida>> GetBySolicitudIdAsync(Guid solicitudId);
        Task<TesisDirigida?> GetByIdAsync(int id);
        Task AddAsync(TesisDirigida tesis);
        Task UpdateAsync(TesisDirigida tesis);
        Task DeleteAsync(int id);
        Task<bool> ExistsByHashAsync(string hash);
        Task AddToSolicitudAsync(Guid solicitudId, int tesisId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int tesisId);
    }
}
