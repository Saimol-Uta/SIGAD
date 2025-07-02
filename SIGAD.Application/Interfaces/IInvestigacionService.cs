using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Interfaces
{
    public interface IInvestigacionService
    {
        // Operaciones CRUD esenciales
        Task<IEnumerable<InvestigacionDto>> GetAllAsync();
        Task<InvestigacionDto?> GetByIdAsync(int id);
        Task<IEnumerable<InvestigacionDto>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<InvestigacionDto>> GetBySolicitudIdAsync(Guid solicitudId);
        Task<InvestigacionDto> CreateAsync(CrearInvestigacionDto crearInvestigacionDto, IFormFile informe);
        Task<bool> DeleteAsync(int id);
        Task<InvestigacionDto?> UpdateAsync(int id, ActualizarInvestigacionDto actualizarInvestigacionDto, IFormFile? archivo);

        // Vistas simplificadas
        Task<IEnumerable<VerInvestigacionDto>> GetVerInvestigacionesAsync();

        // Operaciones de archivos
        Task<(byte[] FileContent, string ContentType, string FileName)> DownloadInformeAsync(int id);
    }
} 