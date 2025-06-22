using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Entities;

namespace SIGAD.Domain.Interfaces
{
    public interface ICursoRepository
    {
        // Operaciones CRUD básicas
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<IEnumerable<Curso>> GetByDocenteCedulaAsync(string docenteCedula);
        Task<IEnumerable<Curso>> GetBySolicitudIdAsync(Guid solicitudId);
        Task AddAsync(Curso curso);
        Task UpdateAsync(Curso curso);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Operaciones de asociación con solicitudes
        Task AddToSolicitudAsync(Guid solicitudId, int cursoId);
        Task RemoveFromSolicitudAsync(Guid solicitudId, int cursoId);

        Task<bool> ExistePorHashAsync(string hash);
        Task AgregarAsync(Curso curso);

        // Métodos específicos para el reglamento de promoción
        Task<int> GetTotalHorasCapacitacionAsync(string docenteCedula, int ultimosAnios = 3);
        Task<int> GetHorasActualizacionPedagogicaAsync(string docenteCedula, int ultimosAnios = 3);
        Task<int> GetHorasActualizacionCientificaAsync(string docenteCedula, int ultimosAnios = 3);

        // Para validación según rangos del reglamento
        Task<bool> CumpleRequisitoHorasParaRangoAsync(string docenteCedula, int rangoSolicitadoId);
        Task<IEnumerable<Curso>> GetCursosByPeriodoAsync(string docenteCedula, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Curso>> GetCursosByTipoAsync(string docenteCedula, string tipoCurso);

        // Para certificaciones internacionales y universidades acreditadas
        Task<IEnumerable<Curso>> GetCursosInstitucionesAcreditadasAsync(string docenteCedula);
        Task<bool> EsInstitucionAcreditadaAsync(string institucion);

        // Para equivalencias del Art. 3 del reglamento
        Task<int> GetHorasEquivalenciasFacilitacionAsync(string docenteCedula);
        Task RegistrarEquivalenciaFacilitacionAsync(string docenteCedula, string tipoFacilitacion, int horasEquivalentes);
    }
}