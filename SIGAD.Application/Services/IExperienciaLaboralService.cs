using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Services
{
    public interface IExperienciaLaboralService
    {
        Task<IEnumerable<ExperienciaLaboralDto>> GetAllExperienciasAsync();
        Task<ExperienciaLaboralDto?> GetExperienciaByIdAsync(int id);
        Task<IEnumerable<ExperienciaLaboralDto>> GetExperienciasByDocenteAsync(string docenteCedula);
        Task<IEnumerable<ExperienciaLaboralDto>> GetExperienciasBySolicitudAsync(Guid solicitudId);
        Task<ExperienciaLaboralDto> CreateExperienciaAsync(CreateExperienciaLaboralDto createDto, IFormFile? archivo);
        Task<ExperienciaLaboralDto> UpdateExperienciaAsync(int id, UpdateExperienciaLaboralDto updateDto, IFormFile? archivo);
        Task<bool> DeleteExperienciaAsync(int id);
        Task<bool> AsociarExperienciaASolicitudAsync(AsociarExperienciaSolicitudDto asociarDto);
        Task DesasociarExperienciaDeSolicitudAsync(Guid solicitudId, int experienciaId);
        Task<byte[]?> GetArchivoExperienciaAsync(int id);
        Task<string?> GetNombreArchivoAsync(int id);
    }
} 