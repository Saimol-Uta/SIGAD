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
    }
}