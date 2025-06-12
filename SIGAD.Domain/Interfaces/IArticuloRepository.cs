using SIGAD.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        // Método que necesita el servicio de validación
        Task<IEnumerable<Articulo>> GetByDocenteAsync(string cedula);

        // Puedes mantener o añadir otros métodos aquí si los necesitas
        Task<Articulo?> GetByIdAsync(string doi);
        Task<IEnumerable<Articulo>> GetAllAsync();

        Task AddAsync(Articulo articulo);
    }
} 