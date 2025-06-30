using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace SIGAD.Application.Services
{
    public interface ITesisDirigidaService
    {
        Task<IEnumerable<TesisDirigidaDto>> ObtenerPorDocenteAsync(string cedula);
        Task<TesisDirigidaDto> CrearAsync(CreateTesisDirigidaDto dto);
        Task AsociarASolicitudAsync(Guid solicitudId, int tesisId);
        Task DesasociarDeSolicitudAsync(Guid solicitudId, int tesisId);
        Task<bool> ExistePorHashAsync(string hash);
    }

    public class TesisDirigidaService : ITesisDirigidaService
    {
        private readonly ITesisDirigidaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TesisDirigidaService(ITesisDirigidaRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TesisDirigidaDto>> ObtenerPorDocenteAsync(string cedula)
        {
            var tesis = await _repository.GetByDocenteCedulaAsync(cedula);
            return tesis.Select(t => new TesisDirigidaDto
            {
                Id = t.Id,
                DocenteCedula = t.DocenteCedula,
                NivelAcademico = t.NivelAcademico.ToString(),
                TituloTesis = t.TituloTesis,
                Estado = t.Estado.ToString(),
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin,
                Institucion = t.Institucion,
                CertificacionRuta = t.CertificacionRuta
            });
        }

        public async Task<TesisDirigidaDto> CrearAsync(CreateTesisDirigidaDto dto)
        {
            var tesis = new TesisDirigida
            {
                DocenteCedula = dto.DocenteCedula,
                NivelAcademico = Enum.Parse<NivelAcademico>(dto.NivelAcademico),
                TituloTesis = dto.TituloTesis,
                Estado = Enum.Parse<EstadoTesis>(dto.Estado),
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Institucion = dto.Institucion,
                CertificacionRuta = dto.CertificacionRuta,
                ContenidoHash = GenerarHash(dto) // Generar hash basado en el contenido
            };

            await _repository.AddAsync(tesis);
            await _unitOfWork.SaveChangesAsync(); // ¡Aquí estaba el problema!

            return new TesisDirigidaDto
            {
                Id = tesis.Id,
                DocenteCedula = tesis.DocenteCedula,
                NivelAcademico = tesis.NivelAcademico.ToString(),
                TituloTesis = tesis.TituloTesis,
                Estado = tesis.Estado.ToString(),
                FechaInicio = tesis.FechaInicio,
                FechaFin = tesis.FechaFin,
                Institucion = tesis.Institucion,
                CertificacionRuta = tesis.CertificacionRuta
            };
        }

        public async Task AsociarASolicitudAsync(Guid solicitudId, int tesisId)
        {
            await _repository.AddToSolicitudAsync(solicitudId, tesisId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DesasociarDeSolicitudAsync(Guid solicitudId, int tesisId)
        {
            await _repository.RemoveFromSolicitudAsync(solicitudId, tesisId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _repository.ExistsByHashAsync(hash);
        }

        private string GenerarHash(CreateTesisDirigidaDto dto)
        {
            var contenido = $"{dto.DocenteCedula}|{dto.TituloTesis}|{dto.NivelAcademico}|{dto.Estado}|{dto.Institucion}|{dto.FechaInicio:yyyy-MM-dd}";
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(contenido));
            return Convert.ToBase64String(hash);
        }
    }
}
