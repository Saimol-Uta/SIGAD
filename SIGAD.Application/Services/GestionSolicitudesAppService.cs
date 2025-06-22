//using SIGAD.Application.DTOs;
//using SIGAD.Domain.Entities;
//using SIGAD.Domain.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace SIGAD.Application.Services
//{
//    public class GestionSolicitudesAppService
//    {
//        private readonly ISolicitudAscensoRepository _solicitudRepository;
//        private readonly IDocenteRepository _docenteRepository;
//        private readonly IUnitOfWork _unitOfWork;

//        public GestionSolicitudesAppService(
//            ISolicitudAscensoRepository solicitudRepository,
//            IDocenteRepository docenteRepository,
//            IUnitOfWork unitOfWork)
//        {
//            _solicitudRepository = solicitudRepository;
//            _docenteRepository = docenteRepository;
//            _unitOfWork = unitOfWork;
//        }

//        public async Task<Guid> EnviarSolicitudConEvidenciaAsync(EnviarSolicitudDto dto, string docenteCedula)
//        {
//            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
//            if (docente == null) throw new KeyNotFoundException("Docente no encontrado.");

//            var nuevaSolicitud = new SolicitudAscenso(
//                docenteCedula,
//                dto.RangoSolicitadoId,
//                docente.RangoActualId
//            );

//            dto.ArticulosDOI.ForEach(doi => nuevaSolicitud.ArticulosPorSolicitud.Add(new ArticulosPorSolicitud { ArticuloDOI = doi }));
//            dto.CursosId.ForEach(id => nuevaSolicitud.CursosPorSolicitud.Add(new CursosPorSolicitud { CursoId = id }));
//            dto.InvestigacionesId.ForEach(id => nuevaSolicitud.InvestigacionesPorSolicitud.Add(new InvestigacionesPorSolicitud { InvestigacionId = id }));
//            dto.ExperienciasId.ForEach(id => nuevaSolicitud.ExperienciaPorSolicitud.Add(new ExperienciaPorSolicitud { ExperienciaId = id }));
//            dto.EvaluacionesId.ForEach(id => nuevaSolicitud.EvaluacionesPorSolicitud.Add(new EvaluacionesPorSolicitud { EvaluacionId = id }));
//            dto.TesisId.ForEach(id => nuevaSolicitud.TesisPorSolicitud.Add(new TesisPorSolicitud { TesisDirigidaId = id }));

//            await _solicitudRepository.AddAsync(nuevaSolicitud);
//            await _unitOfWork.SaveChangesAsync();

//            return nuevaSolicitud.Id;
//        }

//        public async Task<IEnumerable<VerSolicitudDto>> GetAllParaAdminAsync()
//        {
//            var solicitudes = await _solicitudRepository.GetAllWithDetailsAsync();
//            return solicitudes.Select(s => new VerSolicitudDto
//            {
//                Id = s.Id,
//                DocenteNombreCompleto = $"{s.Docente.Nombre1} {s.Docente.Apellido1}",
//                RangoSolicitadoNombre = s.RangoSolicitado.Nombre,
//                Estado = s.Estado.ToString(),
//                FechaEnvio = s.FechaEnvio ?? s.FechaCreacion
//            });
//        }

//        public async Task<SolicitudDetalleDto?> GetDetalleParaAdminAsync(Guid id)
//        {
//            var solicitud = await _solicitudRepository.GetByIdWithDetailsAsync(id);
//            if (solicitud == null) return null;

//            return new SolicitudDetalleDto
//            {
//                Id = solicitud.Id,
//                Estado = solicitud.Estado.ToString(),
//                FechaCreacion = solicitud.FechaCreacion,
//                FechaEnvio = solicitud.FechaEnvio,
//                FechaResolucion = solicitud.FechaResolucion,
//                ObservacionesAdmin = solicitud.ObservacionesAdmin,
//                DocenteCedula = solicitud.Docente?.Cedula ?? "",
//                DocenteNombreCompleto = solicitud.Docente != null
//                    ? $"{solicitud.Docente.Nombre1} {solicitud.Docente.Nombre2} {solicitud.Docente.Apellido1} {solicitud.Docente.Apellido2}".Replace("  ", " ").Trim()
//                    : "N/A",
//                RangoActualNombre = solicitud.RangoActual?.Nombre ?? "N/A",
//                RangoSolicitadoNombre = solicitud.RangoSolicitado?.Nombre ?? "N/A",

//                ArticulosPresentados = solicitud.ArticulosPorSolicitud.Select(a => new VerArticuloDto
//                {
//                    DOI = a.Articulo?.DOI ?? "",
//                    Titulo = a.Articulo?.Titulo ?? "",
//                    Revista = a.Articulo?.Revista ?? "",
//                    AnioPublicacion = a.Articulo?.AnioPublicacion ?? 0,
//                    DocenteCedula = a.Articulo?.DocenteCedula ?? "",
//                    DocenteNombreCompleto = a.Articulo?.Docente != null
//                        ? $"{a.Articulo.Docente.Nombre1} {a.Articulo.Docente.Apellido1}".Trim()
//                        : "N/A"
//                }).ToList(),

//                InvestigacionesPresentadas = solicitud.InvestigacionesPorSolicitud.Select(i => new VerInvestigacionDto
//                {
//                    Id = i.Investigacion?.Id ?? 0,
//                    Titulo = i.Investigacion?.Titulo ?? "",
//                    RolEnInvestigacion = i.Investigacion?.RolEnInvestigacion ?? "",
//                    MesesDeInvestigacion = i.Investigacion?.MesesDeInvestigacion ?? 0,
//                    FechaFinalizacion = i.Investigacion?.FechaFinalizacion ?? DateTime.MinValue,
//                    NombreDocente = $"{solicitud.Docente?.Nombre1} {solicitud.Docente?.Apellido1}".Trim()
//                }).ToList(),

//                CursosPresentados = solicitud.CursosPorSolicitud.Select(c => new VerCursoDto
//                {
//                    Id = c.Curso?.Id ?? 0,
//                    Nombre = c.Curso?.Nombre ?? "",
//                    NombreOrganizacion = c.Curso?.Organizacion?.Nombre ?? "",
//                    NumeroHoras = c.Curso?.NumeroHoras ?? 0,
//                    FechaFinalizacion = c.Curso?.FechaFinalizacion ?? DateTime.MinValue,
//                    DocenteCedula = c.Curso?.DocenteCedula ?? "",
//                    NombreDocente = c.Curso?.Docente != null
//                        ? $"{c.Curso.Docente.Nombre1} {c.Curso.Docente.Apellido1}".Trim()
//                        : "N/A",
//                    TieneCertificado = !string.IsNullOrEmpty(c.Curso?.CertificadoRuta)
//                }).ToList(),

//                ExperienciasLaborales = solicitud.ExperienciaPorSolicitud.Select(e => new VerExperienciaLaboralDto
//                {
//                    Id = e.ExperienciaLaboral?.Id ?? 0,
//                    OrganizacionNombre = e.ExperienciaLaboral?.Organizacion?.Nombre ?? "",
//                    OrganizacionTipo = e.ExperienciaLaboral?.Organizacion?.TipoOrganizacion ?? "",
//                    Cargo = e.ExperienciaLaboral?.Cargo ?? "",
//                    FechaInicio = e.ExperienciaLaboral?.FechaInicio ?? DateTime.MinValue,
//                    FechaFin = e.ExperienciaLaboral?.FechaFin,
//                    CertificadoRuta = e.ExperienciaLaboral?.CertificadoRuta ?? ""
//                }).ToList(),

//                EvaluacionesDocente = solicitud.EvaluacionesPorSolicitud.Select(ev => new VerEvaluacionDocenteDto
//                {
//                    Id = ev.Evaluacion?.Id ?? 0,
//                    PeriodoAcademico = ev.Evaluacion?.PeriodoAcademico ?? "",
//                    FechaEvaluacion = ev.Evaluacion?.FechaEvaluacion ?? DateTime.MinValue,
//                    PuntajePorcentual = ev.Evaluacion?.PuntajePorcentual ?? 0,
//                    InformeRuta = ev.Evaluacion?.InformeRuta ?? ""
//                }).ToList(),

//                TesisDirigidas = solicitud.TesisPorSolicitud.Select(t => new VerTesisDirigidaDto
//                {
//                    Id = t.TesisDirigida?.Id ?? 0,
//                    Titulo = t.TesisDirigida?.TituloTesis ?? "",
//                    FechaInicio = t.TesisDirigida?.FechaInicio ?? DateTime.MinValue,
//                    FechaFin = t.TesisDirigida?.FechaFin,
//                    Nivel = t.TesisDirigida?.NivelAcademico ?? ""
//                }).ToList()


//            };
//        }

//        public async Task AprobarSolicitudAsync(Guid id, string observaciones)
//        {
//            var solicitud = await _solicitudRepository.GetByIdAsync(id);
//            if (solicitud != null)
//            {
//                solicitud.Aprobar(observaciones);
//                await _unitOfWork.SaveChangesAsync();
//            }
//        }

//        public async Task RechazarSolicitudAsync(Guid id, string observaciones)
//        {
//            var solicitud = await _solicitudRepository.GetByIdAsync(id);
//            if (solicitud != null)
//            {
//                solicitud.Rechazar(observaciones);
//                await _unitOfWork.SaveChangesAsync();
//            }
//        }
//        public async Task<SolicitudAscenso?> ObtenerBorradorActivoAsync(string docenteCedula)
//        {
//            var solicitudes = await _solicitudRepository.GetAllAsync();
//            return solicitudes
//                .Where(s => s.DocenteCedula == docenteCedula && s.Estado == Domain.Enums.EstadoSolicitud.Borrador)
//                .OrderByDescending(s => s.FechaCreacion)
//                .FirstOrDefault();
//        }

//        // Verifica si el docente tiene una solicitud activa
//        public async Task<bool> TieneSolicitudActivaAsync(string docenteCedula)
//        {
//            var solicitudes = await _solicitudRepository.GetAllAsync();
//            return solicitudes.Any(s =>
//                s.DocenteCedula == docenteCedula &&
//                s.Estado == Domain.Enums.EstadoSolicitud.EnRevision);
//        }

//        // Crea una solicitud simple si no hay una activa
//        public async Task<SolicitudAscenso> CrearSolicitudSimpleAsync(string docenteCedula, int rangoSolicitadoId)
//        {
//            if (await TieneSolicitudActivaAsync(docenteCedula))
//                throw new InvalidOperationException("Ya existe una solicitud activa.");

//            var docente = await _docenteRepository.GetByIdWithDetailsAsync(docenteCedula);
//            if (docente == null)
//                throw new KeyNotFoundException("Docente no encontrado.");

//            var solicitud = new SolicitudAscenso 
//            {
//                Id = Guid.NewGuid(),
//                DocenteCedula = docenteCedula,
//                Estado = Domain.Enums.EstadoSolicitud.EnRevision,
//                FechaCreacion = DateTime.UtcNow,
//                RangoActualId = docente.RangoActualId,
//                RangoSolicitadoId = rangoSolicitadoId
//            };

//            await _solicitudRepository.AddAsync(solicitud);
//            await _unitOfWork.SaveChangesAsync();
//            return solicitud;
//        }


//    }
//}
