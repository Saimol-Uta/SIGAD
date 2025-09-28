using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        Task<IEnumerable<Articulo>> GetAllAsync();
        Task<Articulo?> GetByIdAsync(string doi);
        Task<IEnumerable<Articulo>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<Articulo>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(Articulo articulo);
        Task UpdateAsync(Articulo articulo);
        Task DeleteAsync(string doi);
        Task<bool> ExistsAsync(string doi);
        Task AddToSolicitudAsync(Guid solicitudId, string articuloDoi);
        Task RemoveFromSolicitudAsync(Guid solicitudId, string articuloDoi);

        Task<bool> ExistePorHashAsync(string hash);
        Task AgregarAsync(Articulo articulo);

        // Métodos específicos para el reglamento de promoción
        Task<int> GetCantidadArticulosVerificadosAsync(string docenteCedula);
        Task<IEnumerable<Articulo>> GetArticulosVerificadosAsync(string docenteCedula);
        Task<IEnumerable<Articulo>> GetArticulosPendientesVerificacionAsync(string docenteCedula);

        // Para validación de obras relevantes según reglamento
        Task<bool> CumpleRequisitoArticulosParaRangoAsync(string docenteCedula, int rangoSolicitadoId);
        Task<IEnumerable<Articulo>> GetArticulosByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin);

        // Para verificación institucional (DIDE, DINNOVA, COMITÉ EDITORIAL)
        Task VerificarArticuloAsync(string doi, string unidadVerificadora);
        Task RechazarVerificacionAsync(string doi, string observaciones);
        Task<IEnumerable<Articulo>> GetArticulosPorVerificarAsync();

        // Para indexación y relevancia
        Task<IEnumerable<Articulo>> GetArticulosIndexadosAsync(string docenteCedula);
        Task<bool> EsArticuloRelevante(string doi);
    }
}