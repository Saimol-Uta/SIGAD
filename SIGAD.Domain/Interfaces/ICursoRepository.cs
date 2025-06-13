using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ICursoRepository
    {
        // Operaciones CRUD básicas
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<IEnumerable<Curso>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<Curso>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(Curso curso);
        Task UpdateAsync(Curso curso);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Operaciones de asociación con solicitudes
        Task AddToSolicitudAsync(Guid solicitudId, int cursoId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int cursoId);
    }
}