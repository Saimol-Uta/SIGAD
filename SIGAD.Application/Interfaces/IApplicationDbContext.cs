using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SIGAD.Domain.Entities;

namespace SIGAD.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<SolicitudAscenso> SolicitudesAscenso { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
