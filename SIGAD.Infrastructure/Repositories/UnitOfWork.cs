using Microsoft.EntityFrameworkCore.Storage;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;

namespace SIGAD.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SigadDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(SigadDbContext context)
        {
            _context = context;
            Articulos = new EfArticuloRepository(context);
            Cursos = new EfCursoRepository(context);
            Evaluaciones = new EfEvaluacionDocenteRepository(context);
            Investigaciones = new EfInvestigacionRepository(context);
            Experiencias = new ExperienciaLaboralRepository(context);
            Docentes = new EfDocenteRepository(context);
            Organizaciones = new EfOrganizacionRepository(context);
            Cuentas = new EfCuentaRepository(context);




        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
        public IArticuloRepository Articulos { get; }
        public ICursoRepository Cursos { get; }
        public IEvaluacionDocenteRepository Evaluaciones { get; }
        public IInvestigacionRepository Investigaciones { get; }
        public IExperienciaLaboralRepository Experiencias { get; }
        public IDocenteRepository Docentes { get; }
        public IOrganizacionRepository Organizaciones { get; }
        public ICuentaRepository Cuentas { get; }







        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 