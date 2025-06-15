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
        Task AgregarAsync(Docente docente); // Asegúrate que exista este método
    }
}