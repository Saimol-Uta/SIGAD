// En: SIGAD.Application/Services/ConsultaRangoAppService.cs
using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class ConsultaRangoAppService
    {
        private readonly IRangoRepository _rangoRepository;

        public ConsultaRangoAppService(IRangoRepository rangoRepository)
        {
            _rangoRepository = rangoRepository;
        }

        public async Task<IEnumerable<RangoDto>> GetAllRangosAsync()
        {
            // 1. Llama al método del repositorio para obtener las entidades del dominio.
            var rangosEntidades = await _rangoRepository.GetAllAsync();

            // 2. Mapea la lista de entidades a una lista de DTOs.
            // Esta transformación es una responsabilidad clave del Servicio de Aplicación.
            var rangosDto = rangosEntidades.Select(r => new RangoDto
            {
                Id = r.Id, // El Id es INT en este caso
                Nombre = r.Nombre,
                Descripcion = "Requisitos: " + r.ArticulosRequeridos + " artículos, " + r.AniosExperienciaRequeridos + " años de exp." // Ejemplo de cómo podrías transformar los datos
            }).ToList();

            return rangosDto;
        }
    }
}