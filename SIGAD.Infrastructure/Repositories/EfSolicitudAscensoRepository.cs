using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Repositories
{
    public class EfSolicitudAscensoRepository : ISolicitudAscensoRepository
    {
        public Task AddAsync(SolicitudAscenso solicitud)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SolicitudAscenso>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SolicitudAscenso?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(SolicitudAscenso solicitud)
        {
            throw new NotImplementedException();
        }
    }
}
