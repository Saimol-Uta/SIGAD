using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfArticuloRepository : IArticuloRepository
    {
        public Task AddAsync(Articulo articulo)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Articulo>> GetAllByDocenteAsync(string docenteCedula)
        {
            throw new NotImplementedException();
        }

        public Task<Articulo?> GetByDoiAsync(string doi)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Articulo articulo)
        {
            throw new NotImplementedException();
        }
    }
}
