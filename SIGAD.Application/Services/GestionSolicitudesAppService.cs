using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionSolicitudesAppService
    {
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Inyectamos las dependencias
        public GestionSolicitudesAppService(ISolicitudAscensoRepository solicitudRepository, IUnitOfWork unitOfWork)
        {
            _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VerSolicitudDto>> GetAllSolicitudesAsync()
        {
            // IMPORTANTE: Este método ahora necesita obtener los datos relacionados (joins).
            // Tu repositorio debe tener un método que haga esto usando .Include() de EF Core.
            var solicitudesConDetalles = await _solicitudRepository.GetAllWithDetailsAsync();

            return solicitudesConDetalles.Select(s => new VerSolicitudDto
            {
                Id = s.Id,
                FechaCreacion = s.FechaCreacion,
                Estado = s.Estado.ToString(),

                NombreDocente = s.Docente != null ? $"{s.Docente.Nombre1} {s.Docente.Apellido1}" : "N/A",
                RangoSolicitado = s.RangoSolicitado != null ? s.RangoSolicitado.Nombre : "N/A",
                RangoActual = s.RangoActual != null ? s.RangoActual.Nombre : "Sin Rango Previo"
            }).ToList();
        }

        public async Task<Guid> CrearSolicitudAsync(CrearSolicitudDto dto, string docenteCedula)
        {
            var todasLasSolicitudes = await _solicitudRepository.GetAllAsync();
            var ultimaAprobada = todasLasSolicitudes
                .Where(s => s.DocenteCedula == docenteCedula && s.Estado == EstadoSolicitud.Aprobada)
                .OrderByDescending(s => s.FechaResolucion)
                .FirstOrDefault();

            int? rangoActualId = ultimaAprobada?.RangoSolicitadoId;


            var nuevaSolicitud = new SolicitudAscenso
            {
                Id = Guid.NewGuid(),
                DocenteCedula = docenteCedula,
                FechaCreacion = DateTime.UtcNow,
                Estado = EstadoSolicitud.Borrador,

                RangoSolicitadoId = dto.RangoSolicitadoId,
                RangoActualId = rangoActualId
            };

            await _solicitudRepository.AddAsync(nuevaSolicitud);

            await _unitOfWork.SaveChangesAsync();

            return nuevaSolicitud.Id;
        }

        public async Task AprobarSolicitudAsync(Guid solicitudId)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                throw new KeyNotFoundException($"No se encontró la solicitud con el ID: {solicitudId}");
            }

            solicitud.Estado = EstadoSolicitud.Aprobada;
            solicitud.FechaResolucion = DateTime.UtcNow;

            await _solicitudRepository.UpdateAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RechazarSolicitudAsync(Guid solicitudId)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                throw new KeyNotFoundException($"No se encontró la solicitud con el ID: {solicitudId}");
            }

            solicitud.Estado = EstadoSolicitud.Rechazada;
            solicitud.FechaResolucion = DateTime.UtcNow;

            await _solicitudRepository.UpdateAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}