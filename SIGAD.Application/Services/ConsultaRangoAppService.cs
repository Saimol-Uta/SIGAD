using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    // Opcional: definir una interfaz IConsultaRangoAppService si planeas tener múltiples implementaciones
    // o si otros servicios de aplicación dependerán de este. Por simplicidad, la omitimos por ahora.
    // public interface IConsultaRangoAppService
    // {
    //     Task<IEnumerable<RangoDto>> GetAllRangosAsync();
    // }

    public class ConsultaRangoAppService // Si tuvieras la interfaz: public class ConsultaRangoAppService : IConsultaRangoAppService
    {
        private readonly IRangoRepository _rangoRepository;

        // El constructor recibe la implementación de IRangoRepository mediante Inyección de Dependencias
        public ConsultaRangoAppService(IRangoRepository rangoRepository)
        {
            _rangoRepository = rangoRepository;
        }

       
    }
}