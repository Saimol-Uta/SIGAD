using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Services
{
    public interface IEvaluacionDocenteService
    {
        Task<IEnumerable<EvaluacionDocenteDto>> GetAllEvaluacionesAsync();
        Task<EvaluacionDocenteDto?> GetEvaluacionByIdAsync(int id);
        Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesByDocenteAsync(string docenteCedula);
        Task<IEnumerable<EvaluacionDocenteDto>> GetEvaluacionesBySolicitudAsync(Guid solicitudId);
        Task<EvaluacionDocenteDto> CreateEvaluacionAsync(CreateEvaluacionDocenteDto createDto, IFormFile? archivo);
        Task<EvaluacionDocenteDto> UpdateEvaluacionAsync(int id, UpdateEvaluacionDocenteDto updateDto, IFormFile? archivo);
        Task<bool> DeleteEvaluacionAsync(int id);
        Task<bool> AsociarEvaluacionASolicitudAsync(AsociarEvaluacionSolicitudDto asociarDto);
        Task<bool> DesasociarEvaluacionDeSolicitudAsync(Guid solicitudId, int evaluacionId);
        Task<byte[]?> GetArchivoEvaluacionAsync(int id);
        Task<string?> GetNombreArchivoAsync(int id);
    }
} 