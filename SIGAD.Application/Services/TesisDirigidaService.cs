using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Enums; // Asegúrate de tener este using

namespace SIGAD.Application.Services
{
    public class TesisDirigidaService : ITesisDirigidaService
    {
        private readonly ITesisDirigidaRepository _repository;

        public TesisDirigidaService(ITesisDirigidaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TesisDirigidaDto>> ObtenerPorDocenteAsync(string cedula)
        {
            var tesis = await _repository.GetByDocenteCedulaAsync(cedula);
            return tesis.Select(t => new TesisDirigidaDto
            {
                Id = t.Id,
                DocenteCedula = t.DocenteCedula,
                NivelAcademico = t.NivelAcademico.ToString(), // Enum a string
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
            // Conversión segura de string a enum
            EstadoTesis estadoTesis = EstadoTesis.EnProceso;
            Enum.TryParse<EstadoTesis>(dto.Estado, true, out estadoTesis);

            var tesis = new TesisDirigida
            {
                DocenteCedula = dto.DocenteCedula,
                NivelAcademico = NivelAcademicoHelper.ParseNivelAcademico(dto.NivelAcademico),
                TituloTesis = dto.TituloTesis,
                Estado = estadoTesis,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Institucion = dto.Institucion,
                CertificacionRuta = dto.CertificacionRuta,
                ContenidoHash = string.Empty // Puedes calcular el hash si es necesario
            };

            await _repository.AddAsync(tesis);
            return new TesisDirigidaDto
            {
                Id = tesis.Id,
                DocenteCedula = tesis.DocenteCedula,
                NivelAcademico = tesis.NivelAcademico.ToString(),
                TituloTesis = tesis.TituloTesis,
                Estado = tesis.Estado.ToString(), // Convierte enum a string para el DTO
                FechaInicio = tesis.FechaInicio,
                FechaFin = tesis.FechaFin,
                Institucion = tesis.Institucion,
                CertificacionRuta = tesis.CertificacionRuta
            };
        }

        public async Task AsociarASolicitudAsync(Guid solicitudId, int tesisId)
        {
            await _repository.AddToSolicitudAsync(solicitudId, tesisId);
        }

        public async Task DesasociarDeSolicitudAsync(Guid solicitudId, int tesisId)
        {
            await _repository.RemoveFromSolicitudAsync(solicitudId, tesisId);
        }

        public async Task<bool> ExistePorHashAsync(string hash)
        {
            return await _repository.ExistsByHashAsync(hash);
        }
        public async Task<bool> EliminarAsync(int id)
        {
            var tesis = await _repository.GetByIdAsync(id);
            if (tesis == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }
        public async Task<bool> EditarAsync(int id, CreateTesisDirigidaDto dto)
        {
            var tesis = await _repository.GetByIdAsync(id);
            if (tesis == null)
                return false;

            tesis.TituloTesis = dto.TituloTesis;
            tesis.NivelAcademico = NivelAcademicoHelper.ParseNivelAcademico(dto.NivelAcademico);
            EstadoTesis estadoTesis = EstadoTesis.EnProceso;
            Enum.TryParse<EstadoTesis>(dto.Estado, true, out estadoTesis);
            tesis.Estado = estadoTesis;
            tesis.FechaInicio = dto.FechaInicio;
            tesis.FechaFin = dto.FechaFin;
            tesis.Institucion = dto.Institucion;
            tesis.CertificacionRuta = dto.CertificacionRuta;

            await _repository.UpdateAsync(tesis);
            return true;
        }
        public async Task<string?> ObtenerRutaPdfAsync(int id)
        {
            var tesis = await _repository.GetByIdAsync(id);
            return tesis?.CertificacionRuta;
        }
        public async Task<TesisDirigidaDto?> ObtenerPorIdAsync(int id)
        {
            var tesis = await _repository.GetByIdAsync(id);
            if (tesis == null)
                return null;

            // Mapea la entidad a DTO (ajusta según tu mapeo real)
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

    }
}
