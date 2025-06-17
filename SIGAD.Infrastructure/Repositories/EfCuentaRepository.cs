using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfCuentaRepository : ICuentaRepository
    {
        private readonly SigadDbContext _context;

        public EfCuentaRepository(SigadDbContext context)
        {
            _context = context;
        }

        public async Task<Cuenta?> GetByEmailAsync(string email)
        {
            return await _context.Cuentas
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.Correo == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Cuentas
                .AnyAsync(c => c.Correo == email);
        }

        public async Task<bool> ExistsByDocenteCedulaAsync(string docenteCedula)
        {
            return await _context.Cuentas
                .AnyAsync(c => c.DocenteCedula == docenteCedula);
        }

        public async Task AddAsync(Cuenta cuenta)
        {
            await _context.Cuentas.AddAsync(cuenta);
        }

        public async Task UpdateAsync(Cuenta cuenta)
        {
            _context.Cuentas.Update(cuenta);
            await Task.CompletedTask;
        }
        public async Task<bool> ExistePorCorreoAsync(string correo)
        {
            return await _context.Cuentas.AnyAsync(c => c.Correo.ToLower() == correo.ToLower());
        }
        public async Task AgregarAsync(Cuenta cuenta)
        {
            await _context.Cuentas.AddAsync(cuenta);
        }
        public async Task<bool> ExistePorCedulaAsync(string cedula)
        {
            return await _context.Cuentas.AnyAsync(c => c.DocenteCedula == cedula);
        }


    }
}