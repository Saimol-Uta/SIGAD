using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface IRangoRepository
    {
        Task<Rango> GetByIdAsync(Guid id);
        Task<IEnumerable<Rango>> GetAllAsync();
        Task AddAsync(Rango rango);
        Task UpdateAsync(Rango rango);
        Task DeleteAsync(Guid id);
    }
}