using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IDocenteRepository
    {
        Task<Docente?> GetByCedulaAsync(string cedula);
        Task<bool> ExistsByCedulaAsync(string cedula);
        Task AddAsync(Docente docente);
        Task UpdateAsync(Docente docente);
        Task<Docente?> GetByIdWithDetailsAsync(string cedula);
        Task<Docente?> ObtenerPorCedulaAsync(string cedula);

        // Métodos específicos:
        Task<IEnumerable<Docente>> GetAllAsync();
        Task<IEnumerable<Docente>> GetByRangoAsync(int rangoId);
        Task<bool> ExistsAsync(string cedula);
        Task DeleteAsync(string cedula);
        Task<IEnumerable<Docente>> GetDocentesElegiblesPromocionAsync();
        Task<Docente?> GetWithSolicitudesAsync(string cedula);
    }
}