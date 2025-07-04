using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SIGAD.Application.Services
{
    public class GestionSolicitudesAppService
    {
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IDocenteRepository _docenteRepository;
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

        public async Task<Guid> EnviarSolicitudConEvidenciaAsync(EnviarSolicitudDto dto, string docenteCedula)
        {
            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
            if (docente == null) throw new KeyNotFoundException("Docente no encontrado.");

            var nuevaSolicitud = new SolicitudAscenso
            {
                Id = Guid.NewGuid(),
                DocenteCedula = docenteCedula,
                RangoSolicitadoId = dto.RangoSolicitadoId,
                RangoActualId = docente.RangoActualId,
                Estado = EstadoSolicitud.EnRevision,
                FechaCreacion = DateTime.UtcNow,
                FechaEnvio = DateTime.UtcNow
            };

            // Agregar artículos
            foreach (var doi in dto.ArticulosDOI)
            {
                nuevaSolicitud.ArticulosPorSolicitud.Add(new ArticulosPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    ArticuloDOI = doi
                });
            }

            // Agregar cursos
            foreach (var id in dto.CursosId)
            {
                nuevaSolicitud.CursosPorSolicitud.Add(new CursosPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    CursoId = id
                });
            }

            // Agregar investigaciones
            foreach (var id in dto.InvestigacionesId)
            {
                nuevaSolicitud.InvestigacionesPorSolicitud.Add(new InvestigacionesPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    InvestigacionId = id
                });
            }

            // Agregar experiencias
            foreach (var id in dto.ExperienciasId)
            {
                nuevaSolicitud.ExperienciaPorSolicitud.Add(new ExperienciaPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    ExperienciaId = id
                });
            }

            // Agregar evaluaciones
            foreach (var id in dto.EvaluacionesId)
            {
                nuevaSolicitud.EvaluacionesPorSolicitud.Add(new EvaluacionesPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    EvaluacionId = id
                });
            }

            // Agregar tesis
            foreach (var id in dto.TesisId)
            {
                nuevaSolicitud.TesisPorSolicitud.Add(new TesisPorSolicitud
                {
                    SolicitudId = nuevaSolicitud.Id,
                    TesisId = id
                });
            }

            await _solicitudRepository.AddAsync(nuevaSolicitud);
            await _unitOfWork.CompleteAsync();

            return nuevaSolicitud.Id;
        }

        public async Task<IEnumerable<VerSolicitudDto>> GetAllParaAdminAsync()
        {
            var solicitudes = await _solicitudRepository.GetAllWithDetailsAsync();
            return solicitudes.Select(s => new VerSolicitudDto
            {
                Id = s.Id,
                DocenteNombreCompleto = $"{s.Docente.Nombre1} {s.Docente.Apellido1}",
                RangoSolicitadoNombre = s.RangoSolicitado.Nombre,
                Estado = s.Estado.ToString(),
                FechaEnvio = s.FechaEnvio ?? s.FechaCreacion
            });
        }

        public async Task<SolicitudDetalleDto?> GetDetalleParaAdminAsync(Guid id)
        {
            var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(id);
            if (solicitud == null) return null;

            return new SolicitudDetalleDto
            {
                Id = solicitud.Id,
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                FechaEnvio = solicitud.FechaEnvio,
                FechaResolucion = solicitud.FechaResolucion,
                ObservacionesAdmin = solicitud.ObservacionesAdmin,
                DocenteCedula = solicitud.Docente?.Cedula ?? "",
                DocenteNombreCompleto = solicitud.Docente != null
                    ? $"{solicitud.Docente.Nombre1} {solicitud.Docente.Nombre2} {solicitud.Docente.Apellido1} {solicitud.Docente.Apellido2}".Replace("  ", " ").Trim()
                    : "N/A",
                RangoActualNombre = solicitud.RangoActual?.Nombre ?? "N/A",
                RangoSolicitadoNombre = solicitud.RangoSolicitado?.Nombre ?? "N/A",

                // Campos de aprobación UTA
                AprobadoPorComision = solicitud.AprobadoPorComision,
                AprobadoPorConsejo = solicitud.AprobadoPorConsejo,
                FechaAprobacionComision = solicitud.FechaAprobacionComision,
                FechaAprobacionConsejo = solicitud.FechaAprobacionConsejo,
                ObservacionesComision = solicitud.ObservacionesComision,
                ObservacionesConsejo = solicitud.ObservacionesConsejo,

                ArticulosPresentados = solicitud.ArticulosPorSolicitud.Select(a => new VerArticuloDto
                {
                    DOI = a.Articulo?.DOI ?? "",
                    Titulo = a.Articulo?.Titulo ?? "",
                    Revista = a.Articulo?.Revista ?? "",
                    AnioPublicacion = a.Articulo?.AnioPublicacion ?? 0,
                    DocenteCedula = a.Articulo?.DocenteCedula ?? "",
                    DocenteNombreCompleto = a.Articulo?.Docente != null
                        ? $"{a.Articulo.Docente.Nombre1} {a.Articulo.Docente.Apellido1}".Trim()
                        : "N/A",
                    ArchivoRuta = a.Articulo?.ArchivoRuta ?? ""
                }).ToList(),

                InvestigacionesPresentadas = solicitud.InvestigacionesPorSolicitud.Select(i => new VerInvestigacionDto
                {
                    Id = i.Investigacion?.Id ?? 0,
                    Titulo = i.Investigacion?.Titulo ?? "",
                    RolEnInvestigacion = i.Investigacion?.RolEnInvestigacion ?? "",
                    MesesDeInvestigacion = i.Investigacion?.MesesDeInvestigacion ?? 0,
                    FechaFinalizacion = i.Investigacion?.FechaFinalizacion ?? DateTime.MinValue,
                    NombreDocente = $"{solicitud.Docente?.Nombre1} {solicitud.Docente?.Apellido1}".Trim(),
                    InformeRuta = i.Investigacion?.InformeRuta ?? ""
                }).ToList(),

                CursosPresentados = solicitud.CursosPorSolicitud.Select(c => new VerCursoDto
                {
                    Id = c.Curso?.Id ?? 0,
                    Nombre = c.Curso?.Nombre ?? "",
                    NombreOrganizacion = c.Curso?.Organizacion?.Nombre ?? "",
                    NumeroHoras = c.Curso?.NumeroHoras ?? 0,
                    FechaFinalizacion = c.Curso?.FechaFinalizacion ?? DateTime.MinValue,
                    DocenteCedula = c.Curso?.DocenteCedula ?? "",
                    NombreDocente = c.Curso?.Docente != null
                        ? $"{c.Curso.Docente.Nombre1} {c.Curso.Docente.Apellido1}".Trim()
                        : "N/A",
                    TieneCertificado = !string.IsNullOrEmpty(c.Curso?.CertificadoRuta),
                    CertificadoRuta = c.Curso?.CertificadoRuta ?? ""
                }).ToList(),

                ExperienciasLaborales = solicitud.ExperienciaPorSolicitud.Select(e => new VerExperienciaLaboralDto
                {
                    Id = e.ExperienciaLaboral?.Id ?? 0,
                    OrganizacionNombre = e.ExperienciaLaboral?.Organizacion?.Nombre ?? "",
                    OrganizacionTipo = e.ExperienciaLaboral?.Organizacion?.TipoOrganizacion ?? "",
                    Cargo = e.ExperienciaLaboral?.Cargo ?? "",
                    FechaInicio = e.ExperienciaLaboral?.FechaInicio ?? DateTime.MinValue,
                    FechaFin = e.ExperienciaLaboral?.FechaFin,
                    CertificadoRuta = e.ExperienciaLaboral?.CertificadoRuta ?? ""
                }).ToList(),

                EvaluacionesDocente = solicitud.EvaluacionesPorSolicitud.Select(ev => new VerEvaluacionDocenteDto
                {
                    Id = ev.Evaluacion?.Id ?? 0,
                    PeriodoAcademico = ev.Evaluacion?.PeriodoAcademico ?? "",
                    FechaEvaluacion = ev.Evaluacion?.FechaEvaluacion ?? DateTime.MinValue,
                    PuntajePorcentual = ev.Evaluacion?.PuntajePorcentual ?? 0,
                    InformeRuta = ev.Evaluacion?.InformeRuta ?? ""
                }).ToList(),

                TesisDirigidas = solicitud.TesisPorSolicitud.Select(MapearTesisDirigida).ToList()
            };
        }

        public async Task<SolicitudDetalleDto?> GetDetalleParaDocenteAsync(Guid id, string docenteCedula)
        {
            var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(id);
            if (solicitud == null) return null;

            // Verificar que el docente solo pueda ver sus propias solicitudes
            if (solicitud.DocenteCedula != docenteCedula)
            {
                return null;
            }

            return new SolicitudDetalleDto
            {
                Id = solicitud.Id,
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                FechaEnvio = solicitud.FechaEnvio,
                FechaResolucion = solicitud.FechaResolucion,
                ObservacionesAdmin = solicitud.ObservacionesAdmin,
                DocenteCedula = solicitud.Docente?.Cedula ?? "",
                DocenteNombreCompleto = solicitud.Docente != null
                    ? $"{solicitud.Docente.Nombre1} {solicitud.Docente.Nombre2} {solicitud.Docente.Apellido1} {solicitud.Docente.Apellido2}".Replace("  ", " ").Trim()
                    : "N/A",
                RangoActualNombre = solicitud.RangoActual?.Nombre ?? "N/A",
                RangoSolicitadoNombre = solicitud.RangoSolicitado?.Nombre ?? "N/A",

                // Campos de aprobación UTA
                AprobadoPorComision = solicitud.AprobadoPorComision,
                AprobadoPorConsejo = solicitud.AprobadoPorConsejo,
                FechaAprobacionComision = solicitud.FechaAprobacionComision,
                FechaAprobacionConsejo = solicitud.FechaAprobacionConsejo,
                ObservacionesComision = solicitud.ObservacionesComision,
                ObservacionesConsejo = solicitud.ObservacionesConsejo,

                ArticulosPresentados = solicitud.ArticulosPorSolicitud.Select(a => new VerArticuloDto
                {
                    DOI = a.Articulo?.DOI ?? "",
                    Titulo = a.Articulo?.Titulo ?? "",
                    Revista = a.Articulo?.Revista ?? "",
                    AnioPublicacion = a.Articulo?.AnioPublicacion ?? 0,
                    DocenteCedula = a.Articulo?.DocenteCedula ?? "",
                    DocenteNombreCompleto = a.Articulo?.Docente != null
                        ? $"{a.Articulo.Docente.Nombre1} {a.Articulo.Docente.Apellido1}".Trim()
                        : "N/A",
                    ArchivoRuta = a.Articulo?.ArchivoRuta ?? ""
                }).ToList(),

                InvestigacionesPresentadas = solicitud.InvestigacionesPorSolicitud.Select(i => new VerInvestigacionDto
                {
                    Id = i.Investigacion?.Id ?? 0,
                    Titulo = i.Investigacion?.Titulo ?? "",
                    RolEnInvestigacion = i.Investigacion?.RolEnInvestigacion ?? "",
                    MesesDeInvestigacion = i.Investigacion?.MesesDeInvestigacion ?? 0,
                    FechaFinalizacion = i.Investigacion?.FechaFinalizacion ?? DateTime.MinValue,
                    NombreDocente = $"{solicitud.Docente?.Nombre1} {solicitud.Docente?.Apellido1}".Trim(),
                    InformeRuta = i.Investigacion?.InformeRuta ?? ""
                }).ToList(),

                CursosPresentados = solicitud.CursosPorSolicitud.Select(c => new VerCursoDto
                {
                    Id = c.Curso?.Id ?? 0,
                    Nombre = c.Curso?.Nombre ?? "",
                    NombreOrganizacion = c.Curso?.Organizacion?.Nombre ?? "",
                    NumeroHoras = c.Curso?.NumeroHoras ?? 0,
                    FechaFinalizacion = c.Curso?.FechaFinalizacion ?? DateTime.MinValue,
                    DocenteCedula = c.Curso?.DocenteCedula ?? "",
                    NombreDocente = $"{solicitud.Docente?.Nombre1} {solicitud.Docente?.Apellido1}".Trim(),
                    TieneCertificado = !string.IsNullOrEmpty(c.Curso?.CertificadoRuta),
                    CertificadoRuta = c.Curso?.CertificadoRuta ?? ""
                }).ToList(),

                ExperienciasLaborales = solicitud.ExperienciaPorSolicitud.Select(e => new VerExperienciaLaboralDto
                {
                    Id = e.ExperienciaLaboral?.Id ?? 0,
                    OrganizacionNombre = e.ExperienciaLaboral?.Organizacion?.Nombre ?? "",
                    OrganizacionTipo = e.ExperienciaLaboral?.Organizacion?.TipoOrganizacion ?? "",
                    Cargo = e.ExperienciaLaboral?.Cargo ?? "",
                    FechaInicio = e.ExperienciaLaboral?.FechaInicio ?? DateTime.MinValue,
                    FechaFin = e.ExperienciaLaboral?.FechaFin,
                    CertificadoRuta = e.ExperienciaLaboral?.CertificadoRuta ?? ""
                }).ToList(),

                EvaluacionesDocente = solicitud.EvaluacionesPorSolicitud.Select(ev => new VerEvaluacionDocenteDto
                {
                    Id = ev.Evaluacion?.Id ?? 0,
                    PeriodoAcademico = ev.Evaluacion?.PeriodoAcademico ?? "",
                    FechaEvaluacion = ev.Evaluacion?.FechaEvaluacion ?? DateTime.MinValue,
                    PuntajePorcentual = ev.Evaluacion?.PuntajePorcentual ?? 0,
                    InformeRuta = ev.Evaluacion?.InformeRuta ?? ""
                }).ToList(),

                TesisDirigidas = solicitud.TesisPorSolicitud.Select(MapearTesisDirigida).ToList()
            };
        }

        public async Task AprobarSolicitudAsync(Guid id, string observaciones)
        {
            await _solicitudRepository.AprobarSolicitudAsync(id, observaciones);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RechazarSolicitudAsync(Guid id, string observaciones)
        {
            await _solicitudRepository.RechazarSolicitudAsync(id, observaciones);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<SolicitudAscenso?> ObtenerBorradorActivoAsync(string docenteCedula)
        {
            var solicitudes = await _solicitudRepository.GetByDocenteAsync(docenteCedula);
            return solicitudes
                .Where(s => s.Estado == EstadoSolicitud.Borrador)
                .OrderByDescending(s => s.FechaCreacion)
                .FirstOrDefault();
        }

        public async Task<bool> TieneSolicitudActivaAsync(string docenteCedula)
        {
            var solicitudes = await _solicitudRepository.GetByDocenteAsync(docenteCedula);
            return solicitudes.Any(s => s.Estado == EstadoSolicitud.EnRevision);
        }

        public async Task<SolicitudAscenso> CrearSolicitudSimpleAsync(string docenteCedula, int rangoSolicitadoId)
        {
            if (await TieneSolicitudActivaAsync(docenteCedula))
                throw new InvalidOperationException("Ya existe una solicitud activa.");

            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
            if (docente == null)
                throw new KeyNotFoundException("Docente no encontrado.");

            var solicitud = new SolicitudAscenso
            {
                Id = Guid.NewGuid(),
                DocenteCedula = docenteCedula,
                Estado = EstadoSolicitud.EnRevision,
                FechaCreacion = DateTime.UtcNow,
                RangoActualId = docente.RangoActualId,
                RangoSolicitadoId = rangoSolicitadoId
            };

            await _solicitudRepository.AddAsync(solicitud);
            await _unitOfWork.CompleteAsync();
            return solicitud;
        }
        public async Task<IEnumerable<Rango>> ObtenerRangosDisponiblesAsync(string docenteCedula)
        {
            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
            if (docente == null)
                throw new KeyNotFoundException("Docente no encontrado.");

            // Obtener todos los rangos
            var rangos = await _unitOfWork.Rangos.GetAllAsync();
            var rangoActual = rangos.FirstOrDefault(r => r.Id == docente.RangoActualId);

            if (rangoActual == null)
                return new List<Rango>();

            // Mapeo de progresión de rangos según el reglamento UTA
            var progresionRangos = new Dictionary<string, List<string>>
            {
                ["Auxiliar 1"] = new List<string> { "Auxiliar 2" },
                ["Auxiliar 2"] = new List<string> { "Agregado 1" },
                ["Agregado 1"] = new List<string> { "Agregado 2" },
                ["Agregado 2"] = new List<string> { "Agregado 3" },
                ["Agregado 3"] = new List<string> { "Principal 1" },
                ["Principal 1"] = new List<string> { "Principal 2" },
                ["Principal 2"] = new List<string> { "Principal 3" }
            };

            // Obtener rangos disponibles para el rango actual
            if (progresionRangos.ContainsKey(rangoActual.Nombre))
            {
                var nombresDisponibles = progresionRangos[rangoActual.Nombre];
                return rangos.Where(r => nombresDisponibles.Contains(r.Nombre));
            }

            return new List<Rango>();
        }

        public async Task<Docente?> ObtenerDocentePorCedulaAsync(string cedula)
        {
            return await _docenteRepository.GetByIdWithDetailsAsync(cedula);
        }

        public async Task AprobarPorComisionAsync(Guid id, string observaciones)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(id);
            if (solicitud == null) throw new ArgumentException("Solicitud no encontrada");

            solicitud.AprobarPorComision(observaciones);

            await _solicitudRepository.UpdateAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AprobarPorConsejoAsync(Guid id, string observaciones)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(id);
            if (solicitud == null) throw new ArgumentException("Solicitud no encontrada");

            solicitud.AprobarPorConsejo(observaciones);

            await _solicitudRepository.UpdateAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task FinalizarProcesoAsync(Guid id, string observaciones)
        {
            var solicitud = await _solicitudRepository.GetByIdAsync(id);
            if (solicitud == null) throw new ArgumentException("Solicitud no encontrada");

            solicitud.FinalizarProceso(observaciones);

            await _solicitudRepository.UpdateAsync(solicitud);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<SolicitudAscenso>> GetHistorialDocenteAsync(string docenteCedula)
        {
            return await _solicitudRepository.GetHistorialByDocenteAsync(docenteCedula);
        }

        // Métodos para apelaciones
        public async Task<(bool success, string message)> PresentarApelacionAsync(Guid solicitudId, string justificacion, string docenteCedula, IFormFile? documentoAdjunto)
        {
            try
            {
                // Verificar que la solicitud existe y pertenece al docente
                var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
                if (solicitud == null)
                {
                    return (false, "Solicitud no encontrada");
                }

                if (solicitud.DocenteCedula != docenteCedula)
                {
                    return (false, "No tiene permisos para apelar esta solicitud");
                }

                // Verificar que la solicitud está rechazada
                if (solicitud.Estado != EstadoSolicitud.Rechazada)
                {
                    return (false, "Solo se pueden apelar solicitudes rechazadas");
                }

                // Crear la apelación
                string? rutaDocumento = null;
                if (documentoAdjunto != null)
                {
                    // TODO: Implementar guardado de archivo
                    rutaDocumento = $"apelaciones/{Guid.NewGuid()}_{documentoAdjunto.FileName}";
                }

                // Actualizar el estado de la solicitud a En Apelación
                solicitud.Estado = EstadoSolicitud.EnApelacion;
                solicitud.ObservacionesAdmin = $"{solicitud.ObservacionesAdmin}\n\n[APELACIÓN - {DateTime.Now:dd/MM/yyyy HH:mm}]\nJustificación: {justificacion}";
                
                await _unitOfWork.CompleteAsync();
                
                return (true, "Apelación presentada exitosamente");
            }
            catch (Exception ex)
            {
                return (false, $"Error al presentar la apelación: {ex.Message}");
            }
        }

        public async Task<List<ApelacionDto>> GetApelacionesBySolicitudAsync(Guid solicitudId)
        {
            try
            {
                var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId);
                if (solicitud == null)
                {
                    return new List<ApelacionDto>();
                }

                // Por simplicidad, extraer información de apelación de las observaciones
                var apelaciones = new List<ApelacionDto>();
                
                if (solicitud.Estado == EstadoSolicitud.EnApelacion && !string.IsNullOrEmpty(solicitud.ObservacionesAdmin))
                {
                    // Buscar apelaciones en las observaciones (implementación simplificada)
                    if (solicitud.ObservacionesAdmin.Contains("[APELACIÓN"))
                    {
                        apelaciones.Add(new ApelacionDto
                        {
                            Id = Guid.NewGuid(),
                            SolicitudId = solicitudId,
                            Justificacion = "Apelación registrada en observaciones",
                            FechaCreacion = DateTime.Now,
                            EstadoApelacion = "EN_REVISION",
                            DocenteCedula = solicitud.DocenteCedula,
                            DocenteNombre = solicitud.Docente?.Nombre1 ?? ""
                        });
                    }
                }

                return apelaciones;
            }
            catch (Exception)
            {
                return new List<ApelacionDto>();
            }
        }

        // Actualizar ambos mapeos de TesisDirigidas en GetDetalleAsync y GetDetalleParaDocenteAsync
        private VerTesisDirigidaDto MapearTesisDirigida(TesisPorSolicitud t)
        {
            return new VerTesisDirigidaDto
            {
                Id = t.TesisDirigida?.Id ?? 0,
                Titulo = t.TesisDirigida?.TituloTesis ?? "",
                FechaInicio = t.TesisDirigida?.FechaInicio ?? DateTime.MinValue,
                FechaFin = t.TesisDirigida?.FechaFin,
                Nivel = t.TesisDirigida?.NivelAcademico.ToString() ?? "",
                Estado = t.TesisDirigida?.Estado.ToString() ?? "",
                CertificacionPath = t.TesisDirigida?.CertificacionRuta ?? "",
                Institucion = t.TesisDirigida?.Institucion ?? ""
            };
        }
    }
}
