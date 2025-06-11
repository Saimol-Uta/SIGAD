using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfInvestigacionRepository : IInvestigacionRepository
    {
        public Task AddAsync(Investigacion investigacion)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Investigacion>> GetAllByDocenteAsync(string docenteCedula)
        {
            throw new NotImplementedException();
        }

        public Task<Investigacion?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
