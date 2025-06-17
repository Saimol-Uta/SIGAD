namespace SIGAD.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        IDocenteRepository Docentes { get; }
        IArticuloRepository Articulos { get; }
        ICursoRepository Cursos { get; }
        IEvaluacionDocenteRepository Evaluaciones { get; }
        IInvestigacionRepository Investigaciones { get; }
        IExperienciaLaboralRepository Experiencias { get; }

         IOrganizacionRepository Organizaciones { get; }

        ICuentaRepository Cuentas { get; }



        Task CompleteAsync();
    }
} 