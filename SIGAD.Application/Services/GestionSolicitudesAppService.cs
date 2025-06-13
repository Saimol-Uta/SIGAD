using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
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
        private readonly IDocenteRepository _docenteRepository; // Necesario para obtener el rango actual
        private readonly IUnitOfWork _unitOfWork;

        public GestionSolicitudesAppService(
            ISolicitudAscensoRepository solicitudRepository,
            IDocenteRepository docenteRepository,
            IUnitOfWork unitOfWork)
        {
            _solicitudRepository = solicitudRepository;
            _docenteRepository = docenteRepository;
            _unitOfWork = unitOfWork;
        }

        // Método para crear y enviar la solicitud con su evidencia
        public async Task<Guid> EnviarSolicitudConEvidenciaAsync(EnviarSolicitudDto dto, string docenteCedula)
        {
            // Aquí se implementaría la lógica de negocio, como la regla de los 4 años.
            // Por ahora, implementamos la creación directa.

            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
            if (docente == null) throw new KeyNotFoundException("Docente no encontrado.");

            // Creamos la entidad del dominio
            var nuevaSolicitud = new SolicitudAscenso(
                docenteCedula,
                dto.RangoSolicitadoId,
                docente.RangoActualId // Obtenemos el rango actual del docente
            );

            // Vincular la evidencia seleccionada a la solicitud
            dto.ArticulosDOI.ForEach(doi => nuevaSolicitud.ArticulosPorSolicitud.Add(new ArticulosPorSolicitud { ArticuloDOI = doi }));
            dto.CursosId.ForEach(id => nuevaSolicitud.CursosPorSolicitud.Add(new CursosPorSolicitud { CursoId = id }));
            dto.InvestigacionesId.ForEach(id => nuevaSolicitud.InvestigacionesPorSolicitud.Add(new InvestigacionesPorSolicitud { InvestigacionId = id }));
            dto.ExperienciasId.ForEach(id => nuevaSolicitud.ExperienciaPorSolicitud.Add(new ExperienciaPorSolicitud { ExperienciaId = id }));
            dto.EvaluacionesId.ForEach(id => nuevaSolicitud.EvaluacionesPorSolicitud.Add(new EvaluacionesPorSolicitud { EvaluacionId = id }));

            // Guardar en la base de datos
            await _solicitudRepository.AddAsync(nuevaSolicitud);
            await _unitOfWork.SaveChangesAsync();

            return nuevaSolicitud.Id;
        }

        // Método para obtener la lista para el panel de admin
        public async Task<IEnumerable<VerSolicitudDto>> GetAllParaAdminAsync()
        {
            var solicitudes = await _solicitudRepository.GetAllWithDetailsAsync();
            return solicitudes.Select(s => new VerSolicitudDto
            {
                Id = s.Id,
                DocenteNombreCompleto = $"{s.Docente.Nombre1} {s.Docente.Apellido1}",
                RangoSolicitadoNombre = s.RangoSolicitado.Nombre,
                Estado = s.Estado.ToString(),
                FechaEnvio = s.FechaEnvio ?? s.FechaCreacion // Usar FechaCreacion si FechaEnvio es nula
            });
        }

        // Método para obtener el detalle completo de una solicitud
        public async Task<SolicitudDetalleDto?> GetDetalleParaAdminAsync(Guid id)
        {
            var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(id);
            if (solicitud == null) return null;

            // Mapeo de la entidad completa al DTO de detalle
            return new SolicitudDetalleDto
            {
                Id = solicitud.Id,
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                FechaEnvio = solicitud.FechaEnvio,
                FechaResolucion = solicitud.FechaResolucion,
                ObservacionesAdmin = solicitud.ObservacionesAdmin,
                DocenteCedula = solicitud.Docente.Cedula,
                DocenteNombreCompleto = $"{solicitud.Docente.Nombre1} {solicitud.Docente.Nombre2} {solicitud.Docente.Apellido1} {solicitud.Docente.Apellido2}".Replace("  ", " ").Trim(),
                RangoActualNombre = solicitud.RangoActual?.Nombre ?? "N/A",
                RangoSolicitadoNombre = solicitud.RangoSolicitado.Nombre,
                ArticulosPresentados = solicitud.ArticulosPorSolicitud.Select(a => new VerArticuloDto { DOI = a.Articulo.DOI, Titulo = a.Articulo.Titulo, Revista = a.Articulo.Revista, AnioPublicacion = a.Articulo.AnioPublicacion }).ToList(),
                InvestigacionesPresentadas = solicitud.InvestigacionesPorSolicitud.Select(i => new VerInvestigacionDto { Id = i.Investigacion.Id, Titulo = i.Investigacion.Titulo, MesesDeInvestigacion = i.Investigacion.MesesDeInvestigacion }).ToList(),
                // ... aquí irían los mapeos para Cursos, Experiencias, etc. ...
            };
        }

        // Métodos para cambiar el estado de la solicitud
        public async Task AprobarSolicitudAsync(Guid id, string observaciones)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(id);
            if (solicitud != null)
            {
                solicitud.Aprobar(observaciones); // Asumiendo un método en la entidad SolicitudAscenso
                await _unitOfWork.SaveChangesAsync();
            }
        }
        public async Task RechazarSolicitudAsync(Guid id, string observaciones)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(id);
            if (solicitud != null)
            {
                solicitud.Rechazar(observaciones); // Asumiendo un método en la entidad SolicitudAscenso
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}