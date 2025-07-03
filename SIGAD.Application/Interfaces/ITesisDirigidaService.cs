using SIGAD.Application.DTOs;

namespace SIGAD.Application.Interfaces
{
    public interface ITesisDirigidaService
    {
        Task<IEnumerable<TesisDirigidaDto>> ObtenerPorDocenteAsync(string cedula);
        Task<TesisDirigidaDto> CrearAsync(CreateTesisDirigidaDto dto);
        Task AsociarASolicitudAsync(Guid solicitudId, int tesisId);
        Task DesasociarDeSolicitudAsync(Guid solicitudId, int tesisId);
        Task<bool> ExistePorHashAsync(string hash);

        // Métodos nuevos:
        Task<bool> EliminarAsync(int id);
        Task<bool> EditarAsync(int id, CreateTesisDirigidaDto dto);
        Task<string?> ObtenerRutaPdfAsync(int id);
        Task<TesisDirigidaDto?> ObtenerPorIdAsync(int id);
    }
}