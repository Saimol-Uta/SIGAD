using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionSolicitudesAppService
    {
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IApelacionRepository _apelacionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GestionSolicitudesAppService(
            ISolicitudAscensoRepository solicitudRepository,
            IDocenteRepository docenteRepository,
            IApelacionRepository apelacionRepository,
            IUnitOfWork unitOfWork)
        {
            _solicitudRepository = solicitudRepository;
            _docenteRepository = docenteRepository;
            _apelacionRepository = apelacionRepository;
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
        public async Task<(bool success, string message)> PresentarApelacionAsync(Guid solicitudId, string justificacion, string docenteCedula, IFormFileCollection? documentosAdjuntos)
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

                // Verificar si ya existe una apelación pendiente
                var tieneApelacionPendiente = await _apelacionRepository.TieneApelacionPendienteAsync(solicitudId);
                if (tieneApelacionPendiente)
                {
                    return (false, "Ya existe una apelación pendiente para esta solicitud");
                }

                // Manejar archivos adjuntos si existen
                string? rutasDocumentos = null;
                if (documentosAdjuntos != null && documentosAdjuntos.Count > 0)
                {
                    var rutas = new List<string>();
                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "apelaciones");
                    
                    // Crear el directorio si no existe
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    foreach (var archivo in documentosAdjuntos)
                    {
                        if (archivo.Length > 0)
                        {
                            // Generar nombre único para el archivo
                            var extension = Path.GetExtension(archivo.FileName);
                            var nombreUnico = $"{Guid.NewGuid()}{extension}";
                            var rutaCompleta = Path.Combine(uploadsPath, nombreUnico);
                            
                            // Guardar el archivo físicamente
                            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                            {
                                await archivo.CopyToAsync(stream);
                            }
                            
                            // Agregar la ruta relativa a la lista
                            rutas.Add($"uploads/apelaciones/{nombreUnico}");
                        }
                    }
                    
                    rutasDocumentos = string.Join(";", rutas);
                }

                // Crear la nueva apelación
                var nuevaApelacion = new Apelacion(solicitudId, justificacion, docenteCedula)
                {
                    DocumentosRespaldo = rutasDocumentos
                };

                // Agregar la apelación al repositorio
                await _apelacionRepository.AddAsync(nuevaApelacion);

                // Actualizar el estado de la solicitud a En Apelación
                solicitud.Estado = EstadoSolicitud.EnApelacion;
                await _solicitudRepository.UpdateAsync(solicitud);
                
                // Guardar los cambios
                await _unitOfWork.CompleteAsync();
                
                return (true, "Apelación presentada exitosamente");
            }
            catch (Exception ex)
            {
                // Log detallado del error
                Console.WriteLine($"Error en PresentarApelacionAsync: {ex}");
                return (false, $"Error al presentar la apelación: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }

        public async Task<List<ApelacionDto>> GetApelacionesBySolicitudAsync(Guid solicitudId)
        {
            try
            {
                var apelaciones = await _apelacionRepository.GetApelacionesPorSolicitudAsync(solicitudId);
                
                return apelaciones.Select(a => new ApelacionDto
                {
                    Id = Guid.NewGuid(), // Convertir int a Guid para compatibilidad
                    SolicitudId = a.SolicitudAscensoId,
                    Justificacion = a.Motivo,
                    FechaCreacion = a.FechaPresentacion,
                    FechaResolucion = a.FechaResolucion,
                    EstadoApelacion = a.Estado.ToString(),
                    ObservacionesResolucion = a.ObservacionesComision,
                    DocumentoRuta = a.DocumentosRespaldo,
                    DocenteCedula = a.CreadoPor,
                    DocenteNombre = a.SolicitudAscenso?.Docente != null 
                        ? $"{a.SolicitudAscenso.Docente.Nombre1} {a.SolicitudAscenso.Docente.Apellido1}".Trim()
                        : ""
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                Console.WriteLine($"Error al obtener apelaciones: {ex.Message}");
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

        // Métodos para administración de apelaciones
        public async Task<List<SolicitudConApelacionDto>> GetSolicitudesConApelacionesAsync()
        {
            try
            {
                Console.WriteLine("=== GetSolicitudesConApelacionesAsync - Iniciando ===");
                var solicitudes = await _solicitudRepository.GetAllWithDetailsAsync();
                var cantidadSolicitudes = solicitudes?.ToList().Count ?? 0;
                Console.WriteLine($"Encontradas {cantidadSolicitudes} solicitudes totales");

                var resultado = new List<SolicitudConApelacionDto>();

                foreach (var solicitud in solicitudes)
                {
                    // Refrescar el estado de la solicitud desde la base de datos para evitar datos cacheados
                    var solicitudActualizada = await _solicitudRepository.GetByIdWithDetailsAsync(solicitud.Id);
                    if (solicitudActualizada == null) continue;

                    var apelaciones = await _apelacionRepository.GetApelacionesPorSolicitudAsync(solicitud.Id);
                    if (apelaciones == null || !apelaciones.Any())
                        continue; // Solo mostrar solicitudes con al menos una apelación

                    // Tomar la última apelación (por fecha de creación o resolución)
                    var ultimaApelacion = apelaciones.OrderByDescending(a => a.FechaPresentacion).FirstOrDefault();
                    if (ultimaApelacion == null)
                        continue;

                    // Determinar el estado de la apelación
                    string estadoApelacion = ultimaApelacion.Estado.ToString();
                    bool tieneApelacionPendiente = ultimaApelacion.Estado == Domain.Enums.EstadoApelacion.Pendiente;
                    bool apelacionResuelta = ultimaApelacion.Estado == Domain.Enums.EstadoApelacion.Aceptada || ultimaApelacion.Estado == Domain.Enums.EstadoApelacion.Rechazada;

                    var dto = new SolicitudConApelacionDto
                    {
                        Id = solicitudActualizada.Id,
                        DocenteNombreCompleto = $"{solicitudActualizada.Docente?.Nombre1} {solicitudActualizada.Docente?.Apellido1}".Trim(),
                        RangoSolicitadoNombre = solicitudActualizada.RangoSolicitado?.Nombre ?? "",
                        FechaCreacion = solicitudActualizada.FechaCreacion,
                        Estado = solicitudActualizada.Estado.ToString(),
                        TieneApelacion = tieneApelacionPendiente || apelacionResuelta,
                        EstadoApelacion = estadoApelacion // <-- Nuevo campo opcional para mostrar el estado real
                    };

                    // Corrección: siempre asignar fechas y días restantes, incluso si la apelación está resuelta
                    dto.FechaApelacion = ultimaApelacion.FechaPresentacion;
                    dto.FechaLimiteApelacion = ultimaApelacion.FechaLimiteRespuesta;
                    dto.ApelacionVencida = tieneApelacionPendiente ? DateTime.Now > ultimaApelacion.FechaLimiteRespuesta : false;
                    dto.DiasRestantesApelacion = tieneApelacionPendiente
                        ? Math.Max(0, (ultimaApelacion.FechaLimiteRespuesta - DateTime.Now).Days)
                        : 0;

                    resultado.Add(dto);
                }
                return resultado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener solicitudes con apelaciones: {ex}");
                return new List<SolicitudConApelacionDto>();
            }
        }

        public async Task<ApelacionDetalleDto?> GetApelacionDetalleAsync(Guid solicitudId)
        {
            try
            {
                var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(solicitudId);
                if (solicitud == null) return null;

                var apelaciones = await _apelacionRepository.GetApelacionesPorSolicitudAsync(solicitudId);
                var apelacionActiva = apelaciones?.FirstOrDefault(a => a.Estado == Domain.Enums.EstadoApelacion.Pendiente);
                
                if (apelacionActiva == null) return null;

                return new ApelacionDetalleDto
                {
                    Id = apelacionActiva.Id, // <-- CORRECTO: el ID de la apelación (int)
                    SolicitudId = solicitudId,
                    DocenteNombre = $"{solicitud.Docente?.Nombre1} {solicitud.Docente?.Apellido1}".Trim(),
                    DocenteEmail = "", // No hay email en la entidad Docente, dejar vacío o buscar en otro lado
                    Justificacion = apelacionActiva.Motivo,
                    DocumentosAdjuntos = string.IsNullOrEmpty(apelacionActiva.DocumentosRespaldo) ? new List<string>() : new List<string> { apelacionActiva.DocumentosRespaldo },
                    FechaCreacion = apelacionActiva.FechaPresentacion,
                    Estado = apelacionActiva.Estado.ToString(),
                    RangoSolicitado = solicitud.RangoSolicitado?.Nombre ?? "",
                    FechaSolicitud = solicitud.FechaCreacion,
                    EstadoSolicitud = solicitud.Estado.ToString(),
                    ObservacionesRechazo = solicitud.ObservacionesAdmin
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalle de apelación: {ex}");
                return null;
            }
        }

        public async Task<(bool success, string message)> ResolverApelacionAsync(int apelacionId, ResolverApelacionDto dto, string adminCedula)
        {
            try
            {
                // Obtener la apelación
                var apelacion = await _apelacionRepository.GetByIdAsync(apelacionId);
                if (apelacion == null)
                {
                    return (false, "Apelación no encontrada");
                }

                // Validar que esté pendiente
                if (apelacion.Estado != Domain.Enums.EstadoApelacion.Pendiente)
                {
                    return (false, "Esta apelación ya fue resuelta");
                }

                // Validar que no esté vencida
                if (DateTime.Now > apelacion.FechaLimiteRespuesta)
                {
                    return (false, "Esta apelación está vencida");
                }

                // Obtener la solicitud asociada (ya trackeada por el contexto)
                var solicitud = apelacion.SolicitudAscenso;
                if (solicitud == null)
                {
                    // Si por alguna razón no está incluida, cargarla manualmente
                    solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(apelacion.SolicitudAscensoId);
                    if (solicitud == null)
                        return (false, "Solicitud asociada no encontrada");
                }

                // Actualizar la apelación
                apelacion.Estado = dto.Aceptada ? Domain.Enums.EstadoApelacion.Aceptada : Domain.Enums.EstadoApelacion.Rechazada;
                apelacion.FechaResolucion = DateTime.UtcNow;
                apelacion.Aceptada = dto.Aceptada;
                apelacion.ObservacionesComision = dto.ObservacionesComision;
                apelacion.ModificadoPor = adminCedula;
                apelacion.FechaModificacion = DateTime.UtcNow;

                // Actualizar el estado de la solicitud
                if (dto.Aceptada)
                {
                    solicitud.Estado = EstadoSolicitud.AprobadaPorApelacion;
                    // ASCENSO AUTOMÁTICO
                    await AscenderDocenteAutomaticamenteAsync(solicitud);
                }
                else
                {
                    solicitud.Estado = EstadoSolicitud.RechazadaDefinitiva;
                }

                // Guardar cambios (solo una vez)
                await _unitOfWork.CompleteAsync();

                // Enviar notificación
                await EnviarNotificacionResolucionAsync(solicitud, apelacion, dto.Aceptada);

                string mensaje = dto.Aceptada 
                    ? "Apelación aceptada. El docente ha sido ascendido automáticamente."
                    : "Apelación rechazada. La decisión es definitiva.";

                return (true, mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al resolver apelación: {ex}");
                return (false, $"Error al resolver la apelación: {ex.Message}");
            }
        }

        private async Task AscenderDocenteAutomaticamenteAsync(SolicitudAscenso solicitud)
        {
            try
            {
                var docente = await _docenteRepository.GetByCedulaAsync(solicitud.DocenteCedula);
                if (docente != null)
                {
                    // Actualizar el rango del docente
                    docente.RangoActualId = solicitud.RangoSolicitadoId;
                    await _docenteRepository.UpdateAsync(docente);
                    // Guardar el cambio en la base de datos
                    await _unitOfWork.CompleteAsync();
                    Console.WriteLine($"Docente {docente.Cedula} ascendido automáticamente al rango {solicitud.RangoSolicitadoId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ascenso automático: {ex}");
                // No fallar el proceso principal por esto
            }
        }

        private Task EnviarNotificacionResolucionAsync(SolicitudAscenso solicitud, Apelacion apelacion, bool aceptada)
        {
            try
            {
                var docente = solicitud.Docente;
                if (docente == null) return Task.CompletedTask;

                string asunto = aceptada ? "✅ Su apelación ha sido ACEPTADA" : "❌ Resolución de su apelación";
                
                string mensaje = aceptada
                    ? $@"
                        Estimado {docente.Nombre1} {docente.Apellido1},

                        Nos complace informarle que su apelación para la solicitud de ascenso a '{solicitud.RangoSolicitado?.Nombre}' ha sido ACEPTADA por la Comisión Académica.

                        Su solicitud ha sido aprobada y su ascenso ha sido procesado automáticamente.

                        Observaciones de la Comisión:
                        {apelacion.ObservacionesComision ?? "Sin observaciones adicionales"}

                        Fecha de resolución: {DateTime.Now:dd/MM/yyyy HH:mm}
                        Nuevo rango: {solicitud.RangoSolicitado?.Nombre}

                        ¡Felicitaciones por este logro!

                        Sistema SIGAD
                        Universidad Técnica de Ambato"
                    : $@"
                        Estimado {docente.Nombre1} {docente.Apellido1},

                        Lamentamos informarle que su apelación para la solicitud de ascenso a '{solicitud.RangoSolicitado?.Nombre}' ha sido RECHAZADA por la Comisión Académica.

                        Esta decisión es DEFINITIVA según el Art. 6 del reglamento universitario.

                        Observaciones de la Comisión:
                        {apelacion.ObservacionesComision ?? "Sin observaciones adicionales"}

                        Fecha de resolución: {DateTime.Now:dd/MM/yyyy HH:mm}

                        Sistema SIGAD
                        Universidad Técnica de Ambato";

                // TODO: Enviar email real usando un servicio de email
                Console.WriteLine($"EMAIL ENVIADO: {asunto} a {docente.Cedula}@uta.edu.ec");
                Console.WriteLine($"Contenido: {mensaje}");
                
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar notificación: {ex}");
                return Task.CompletedTask;
            }
        }
    }
}
