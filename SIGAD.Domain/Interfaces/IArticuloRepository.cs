using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        Task<IEnumerable<Articulo>> GetAllAsync();
        Task<Articulo?> GetByIdAsync(string doi);
        Task<IEnumerable<Articulo>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<Articulo>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(Articulo articulo);
        Task UpdateAsync(Articulo articulo);
        Task DeleteAsync(string doi);
        Task<bool> ExistsAsync(string doi);
        Task AddToSolicitudAsync(Guid solicitudId, string articuloDoi);
        Task RemoveFromSolicitudAsync(Guid solicitudId, string articuloDoi);
    }
}