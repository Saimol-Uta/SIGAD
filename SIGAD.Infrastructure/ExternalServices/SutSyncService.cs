using Microsoft.Data.SqlClient;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces.Integraciones;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.ExternalServices
{
    public class SutSyncService : ISutSyncService
    {
        private readonly string _connectionString;

        public SutSyncService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ArticuloDto>> ObtenerArticulosAsync(string cedula)
        {
            var articulos = new List<ArticuloDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Articulos WHERE DocenteCedula = @Cedula", conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                articulos.Add(new ArticuloDto
                {
                    DOI = reader["DOI"].ToString()!,
                    Titulo = reader["Titulo"].ToString()!,
                    Revista = reader["Revista"].ToString()!,
                    AnioPublicacion = (int)reader["AnioPublicacion"],
                    ArchivoRuta = reader["ArchivoRuta"].ToString()!,
                    ContenidoHash = reader["ContenidoHash"].ToString()!,
                    DocenteCedula = reader["DocenteCedula"].ToString()!
                });
            }

            return articulos;
        }

        public async Task<IEnumerable<CursoDto>> ObtenerCursosAsync(string cedula)
        {
            var cursos = new List<CursoDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Cursos WHERE DocenteCedula = @Cedula", conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                cursos.Add(new CursoDto
                {
                    Nombre = reader["Nombre"].ToString()!,
                    Organizacion = reader["Organizacion"].ToString()!,
                    NumeroHoras = (int)reader["NumeroHoras"],
                    FechaFinalizacion = (DateTime)reader["FechaFinalizacion"],
                    CertificadoRuta = reader["CertificadoRuta"].ToString()!,
                    ContenidoHash = reader["ContenidoHash"].ToString()!,
                    DocenteCedula = reader["DocenteCedula"].ToString()!
                });
            }

            return cursos;
        }

        public async Task<IEnumerable<EvaluacionDto>> ObtenerEvaluacionesAsync(string cedula)
        {
            var evaluaciones = new List<EvaluacionDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Evaluaciones WHERE DocenteCedula = @Cedula", conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                evaluaciones.Add(new EvaluacionDto
                {
                    PeriodoAcademico = reader["PeriodoAcademico"].ToString()!,
                    FechaEvaluacion = (DateTime)reader["FechaEvaluacion"],
                    PuntajePorcentual = (decimal)reader["PuntajePorcentual"],
                    InformeRuta = reader["InformeRuta"].ToString()!,
                    ContenidoHash = reader["ContenidoHash"].ToString()!,
                    DocenteCedula = reader["DocenteCedula"].ToString()!
                });
            }

            return evaluaciones;
        }

        public async Task<IEnumerable<InvestigacionDto>> ObtenerInvestigacionesAsync(string cedula)
        {
            var investigaciones = new List<InvestigacionDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Investigaciones WHERE DocenteCedula = @Cedula", conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                investigaciones.Add(new InvestigacionDto
                {
                    Titulo = reader["Titulo"].ToString()!,
                    FechaInicio = (DateTime)reader["FechaInicio"],
                    FechaFinalizacion = (DateTime)reader["FechaFinalizacion"],
                    RolEnInvestigacion = reader["RolEnInvestigacion"].ToString()!,
                    MesesDeInvestigacion = (int)reader["MesesDeInvestigacion"],
                    InformeRuta = reader["InformeRuta"].ToString()!,
                    ContenidoHash = reader["ContenidoHash"].ToString()!,
                    DocenteCedula = reader["DocenteCedula"].ToString()!
                });
            }

            return investigaciones;
        }

        public async Task<IEnumerable<ExperienciaDto>> ObtenerExperienciasAsync(string cedula)
        {
            var experiencias = new List<ExperienciaDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand("SELECT * FROM Experiencias WHERE DocenteCedula = @Cedula", conn);
            cmd.Parameters.AddWithValue("@Cedula", cedula);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                experiencias.Add(new ExperienciaDto
                {
                    Organizacion = reader["Organizacion"].ToString()!,
                    Cargo = reader["Cargo"].ToString()!,
                    FechaInicio = (DateTime)reader["FechaInicio"],
                    FechaFin = (DateTime)reader["FechaFin"],
                    CertificadoRuta = reader["CertificadoRuta"].ToString()!,
                    ContenidoHash = reader["ContenidoHash"].ToString()!,
                    DocenteCedula = reader["DocenteCedula"].ToString()!
                });
            }

            return experiencias;
        }
    }
}
