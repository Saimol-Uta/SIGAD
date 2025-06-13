using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Services
{
    public interface IArticuloService
    {
        Task<IEnumerable<ArticuloDto>> GetAllArticulosAsync();
        Task<ArticuloDto?> GetArticuloByIdAsync(string doi);
        Task<IEnumerable<ArticuloDto>> GetArticulosByDocenteAsync(string docenteCedula);
        Task<IEnumerable<ArticuloDto>> GetArticulosBySolicitudAsync(Guid solicitudId);
        Task<ArticuloDto> CreateArticuloAsync(CrearArticuloDto createDto, IFormFile? archivo);
        Task<ArticuloDto> UpdateArticuloAsync(string doi, ActualizarArticuloDto updateDto, IFormFile? archivo);
        Task<bool> DeleteArticuloAsync(string doi);
        Task<bool> AsociarArticuloASolicitudAsync(AsociarArticuloSolicitudDto asociarDto);
        Task<bool> DesasociarArticuloDeSolicitudAsync(Guid solicitudId, string articuloDoi);
        Task<byte[]?> GetArchivoArticuloAsync(string doi);
        Task<string?> GetNombreArchivoAsync(string doi);
    }
} 