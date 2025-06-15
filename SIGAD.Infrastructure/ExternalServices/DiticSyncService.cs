using Microsoft.Data.SqlClient;
using SIGAD.Application.DTOs.IntegracionesExternas;
using SIGAD.Application.Interfaces.Integraciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.ExternalServices
{
    public class DiticSyncService : IDiticSyncService
    {
        private readonly string _connectionString;

        public DiticSyncService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<DocenteDto>> ObtenerDocentesAsync()
        {
            var docentes = new List<DocenteDto>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                SELECT d.Cedula, d.NombreCompleto, c.Correo, c.ClaveHash, c.Rol
                FROM Docentes d
                INNER JOIN Cuentas c ON d.Cedula = c.DocenteCedula";

            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                docentes.Add(new DocenteDto
                {
                    Cedula = reader["Cedula"].ToString()!,
                    NombreCompleto = reader["NombreCompleto"].ToString()!,
                    Correo = reader["Correo"].ToString()!,
                    ClaveHash = reader["ClaveHash"].ToString()!,
                    Rol = reader["Rol"].ToString()!
                });
            }

            return docentes;
        }
    }
}
