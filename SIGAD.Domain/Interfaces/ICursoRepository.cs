using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ICursoRepository
    {
        Task<IEnumerable<Curso>> GetByDocenteAsync(string cedula);
    }
}