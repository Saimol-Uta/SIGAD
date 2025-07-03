using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace SIGAD.Application.Services
{
    public class TesisDirigidaService : ITesisDirigidaService
    {
        private readonly ITesisDirigidaRepository _repository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public TesisDirigidaService(
            ITesisDirigidaRepository repository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
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
                CertificacionRuta = t.CertificacionRuta,
                UrlCloudinary = t.UrlCloudinary,
                
                // Mapeo de solicitudes asociadas
                SolicitudId = t.TesisPorSolicitud?.FirstOrDefault()?.SolicitudId.ToString(),
                Solicitudes = t.TesisPorSolicitud?.Select(tps => new SolicitudBasicaDto
                {
                    SolicitudId = tps.SolicitudId.ToString(),
                    Estado = tps.Solicitud?.Estado.ToString() ?? "Desconocido"
                }).ToList()
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
                UrlCloudinary = dto.UrlCloudinary, // Usar la URL de Cloudinary del DTO
                ContenidoHash = dto.ContenidoHash // Usar el hash del DTO
            };

            await _repository.AddAsync(tesis);
            await _unitOfWork.SaveChangesAsync();

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
                CertificacionRuta = tesis.CertificacionRuta,
                UrlCloudinary = tesis.UrlCloudinary
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

            // Eliminar archivos de ambos almacenamientos
            if (!string.IsNullOrEmpty(tesis.CertificacionRuta) || !string.IsNullOrEmpty(tesis.UrlCloudinary))
            {
                await _fileStorageService.EliminarArchivoDualAsync(tesis.CertificacionRuta, tesis.UrlCloudinary);
            }

            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
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
            tesis.UrlCloudinary = dto.UrlCloudinary; // Asignar URL de Cloudinary
            tesis.ContenidoHash = dto.ContenidoHash; // Asignar hash del contenido

            await _repository.UpdateAsync(tesis);
            await _unitOfWork.SaveChangesAsync();
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
                CertificacionRuta = tesis.CertificacionRuta,
                UrlCloudinary = tesis.UrlCloudinary
            };
        }

    }
}
