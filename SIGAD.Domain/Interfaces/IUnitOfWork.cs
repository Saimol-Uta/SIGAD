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
        ITesisDirigidaRepository TesisDirigidas { get; }

        ISolicitudAscensoRepository SolicitudesAscenso { get; } // ⚠️ CRÍTICO
        IAccionesDePersonalRepository AccionesDePersonal { get; } // ⚠️ CRÍTICO
        IRangoRepository Rangos { get; } // ⚠️ IMPORTANTE
        IApelacionRepository Apelaciones { get; } // ⚠️ NUEVO - Para proceso de apelaciones según Reglamento UTA

        IPromocionService PromocionService { get; } // ⚠️ CRÍTICO

        Task CompleteAsync();
    }
}