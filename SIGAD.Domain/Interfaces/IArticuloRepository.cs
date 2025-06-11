using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        Task<Articulo?> GetByDoiAsync(string doi);
        Task<IEnumerable<Articulo>> GetAllByDocenteAsync(string docenteCedula);
        Task AddAsync(Articulo articulo);
        Task UpdateAsync(Articulo articulo);
    }
}
