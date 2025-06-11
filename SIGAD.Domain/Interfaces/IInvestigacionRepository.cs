using SIGAD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Domain.Interfaces
{
    public interface IInvestigacionRepository
    {
        Task<Investigacion?> GetByIdAsync(int id);
        Task<IEnumerable<Investigacion>> GetAllByDocenteAsync(string docenteCedula);
        Task AddAsync(Investigacion investigacion);
    }
}
