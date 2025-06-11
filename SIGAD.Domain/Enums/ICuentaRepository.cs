using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ICuentaRepository
    {
        Task<Cuenta?> GetByCorreoAsync(string correo);
        Task<Cuenta?> GetByCorreoWithDocenteAsync(string correo);
        Task<bool> ExistsByCorreoAsync(string correo);
        Task<bool> ExistsByDocenteCedulaAsync(string docenteCedula);
        Task AddAsync(Cuenta cuenta);
        Task UpdateAsync(Cuenta cuenta);
    }
}