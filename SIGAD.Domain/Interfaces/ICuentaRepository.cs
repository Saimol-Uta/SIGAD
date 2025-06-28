using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ICuentaRepository
    {
        Task<Cuenta?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByDocenteCedulaAsync(string docenteCedula);
        Task AddAsync(Cuenta cuenta);
        Task UpdateAsync(Cuenta cuenta);

        Task<bool> ExistePorCorreoAsync(string correo);
        Task AgregarAsync(Cuenta cuenta);
        Task<bool> ExistePorCedulaAsync(string cedula);
        Task<bool> VerificarCodigoRecuperacionAsync(string email, string codigo);

    }
} 