using SIGAD.Domain.Entities;

public interface ISolicitudAscensoRepository
{
    Task<SolicitudAscenso?> GetByIdAsync(Guid id);
    Task<IEnumerable<SolicitudAscenso>> GetAllWithDetailsAsync();
    Task<SolicitudAscenso?> GetByIdWithDetailsAsync(Guid id);
    Task AddAsync(SolicitudAscenso solicitud);
    Task UpdateAsync(SolicitudAscenso solicitud);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<SolicitudAscenso>> GetAllAsync();
}