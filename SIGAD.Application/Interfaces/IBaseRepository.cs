using System.Linq.Expressions;

namespace SIGAD.Domain.Interfaces
{
    /// <summary>
    /// Interfaz base para repositorios genéricos.
    /// Define las operaciones CRUD estándar para todas las entidades.
    /// </summary>
    /// <typeparam name="T">La entidad con la que trabajará el repositorio.</typeparam>
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene una entidad por su identificador.
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las entidades.
        /// </summary>
        Task<IReadOnlyList<T>> GetAllAsync();

        /// <summary>
        /// Busca entidades que coincidan con una expresión.
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Agrega una nueva entidad.
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Agrega un rango de entidades.
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Elimina una entidad.
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Elimina un rango de entidades.
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);
    }
}
