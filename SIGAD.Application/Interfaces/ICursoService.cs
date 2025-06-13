using Microsoft.AspNetCore.Http;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Interfaces
{
    public interface ICursoService
    {
        // Operaciones CRUD
        Task<IEnumerable<CursoDto>> GetAllAsync();
        Task<CursoDto?> GetByIdAsync(int id);
        Task<IEnumerable<CursoDto>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<CursoDto>> GetBySolicitudIdAsync(Guid solicitudId);
        Task<CursoDto> CreateAsync(CrearCursoDto crearCursoDto, IFormFile? certificado);
        Task<CursoDto> UpdateAsync(ActualizarCursoDto actualizarCursoDto, IFormFile? certificado = null);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Operaciones de asociación con solicitudes
        Task<bool> AddToSolicitudAsync(AsociarCursoSolicitudDto asociarDto);
        Task<bool> RemoveFromSolicitudAsync(AsociarCursoSolicitudDto asociarDto);

        // Operaciones de archivos
        Task<(byte[] FileContent, string ContentType, string FileName)> DownloadCertificadoAsync(int id);

        // Operaciones de vista simplificada
        Task<IEnumerable<VerCursoDto>> GetAllSimplifiedAsync();
        Task<IEnumerable<VerCursoDto>> GetByDocenteCedulaSimplifiedAsync(string docenteCedula);
    }
} 