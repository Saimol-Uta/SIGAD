using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SIGAD.Application.Services
{
    public class ExperienciaLaboralService : IExperienciaLaboralService
    {
        private readonly IExperienciaLaboralRepository _experienciaRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IOrganizacionRepository _organizacionRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsPath;

        public ExperienciaLaboralService(
            IExperienciaLaboralRepository experienciaRepository,
            IDocenteRepository docenteRepository,
            IOrganizacionRepository organizacionRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _experienciaRepository = experienciaRepository;
            _docenteRepository = docenteRepository;
            _organizacionRepository = organizacionRepository;
            _solicitudRepository = solicitudRepository;
            _unitOfWork = unitOfWork;
            _uploadsPath = configuration["FileStorage:ExperienciasPath"] ?? "uploads/experiencias";
            
            // Crear directorio si no existe
            if (!Directory.Exists(_uploadsPath))
            {
                Directory.CreateDirectory(_uploadsPath);
            }
        }

        public async Task<IEnumerable<ExperienciaLaboralDto>> GetAllExperienciasAsync()
        {
            var experiencias = await _experienciaRepository.GetAllAsync();
            return experiencias.Select(MapToDto);
        }

        public async Task<ExperienciaLaboralDto?> GetExperienciaByIdAsync(int id)
        {
            var experiencia = await _experienciaRepository.GetByIdAsync(id);
            return experiencia != null ? MapToDto(experiencia) : null;
        }

        public async Task<IEnumerable<ExperienciaLaboralDto>> GetExperienciasByDocenteAsync(string docenteCedula)
        {
            var experiencias = await _experienciaRepository.GetByDocenteCedulaAsync(docenteCedula);
            return experiencias.Select(MapToDto);
        }

        public async Task<IEnumerable<ExperienciaLaboralDto>> GetExperienciasBySolicitudAsync(Guid solicitudId)
        {
            var experiencias = await _experienciaRepository.GetBySolicitudIdAsync(solicitudId);
            return experiencias.Select(MapToDto);
        }

        public async Task<ExperienciaLaboralDto> CreateExperienciaAsync(CreateExperienciaLaboralDto createDto, IFormFile? archivo)
        {
            // Validar que el docente existe
            var docente = await _docenteRepository.GetByCedulaAsync(createDto.DocenteCedula);
            if (docente == null)
            {
                throw new ArgumentException("El docente especificado no existe");
            }

            // Buscar o crear la organización
            var organizacion = await _organizacionRepository.GetByNombreAsync(createDto.OrganizacionNombre);
            if (organizacion == null)
            {
                // Crear nueva organización
                organizacion = new Organizacion
                {
                    Nombre = createDto.OrganizacionNombre,
                    TipoOrganizacion = "Empresa" // Tipo por defecto
                };
                await _organizacionRepository.AddAsync(organizacion);
                await _unitOfWork.SaveChangesAsync();
            }

            // Procesar archivo si se proporciona
            string archivoRuta = string.Empty;
            string contenidoHash = string.Empty;

            if (archivo != null && archivo.Length > 0)
            {
                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                archivoRuta = ruta;
                contenidoHash = hash;
            }

            var experiencia = new ExperienciaLaboral
            {
                OrganizacionId = organizacion.Id,
                DocenteCedula = createDto.DocenteCedula,
                Cargo = createDto.Cargo,
                FechaInicio = createDto.FechaInicio,
                FechaFin = createDto.FechaFin,
                CertificadoRuta = archivoRuta,
                ContenidoHash = contenidoHash
            };

            await _experienciaRepository.AddAsync(experiencia);
            
            // Guardar cambios para generar el ID
            await _unitOfWork.SaveChangesAsync();

            // Si se especifica una solicitud, verificar que existe y asociar la experiencia
            if (createDto.SolicitudId.HasValue)
            {
                var solicitudExists = await _solicitudRepository.ExistsAsync(createDto.SolicitudId.Value);
                if (solicitudExists)
                {
                    await _experienciaRepository.AddToSolicitudAsync(createDto.SolicitudId.Value, experiencia.Id);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException($"La solicitud con ID {createDto.SolicitudId.Value} no existe");
                }
            }

            // Obtener la experiencia creada con datos del docente y organización
            var experienciaCreada = await _experienciaRepository.GetByIdAsync(experiencia.Id);
            return MapToDto(experienciaCreada!);
        }

        public async Task<ExperienciaLaboralDto> UpdateExperienciaAsync(int id, UpdateExperienciaLaboralDto updateDto, IFormFile? archivo)
        {
            var experiencia = await _experienciaRepository.GetByIdAsync(id);
            if (experiencia == null)
            {
                throw new ArgumentException("La experiencia no existe");
            }

            // Actualizar propiedades
            experiencia.Cargo = updateDto.Cargo;
            experiencia.FechaInicio = updateDto.FechaInicio;
            experiencia.FechaFin = updateDto.FechaFin;

            // Procesar nuevo archivo si se proporciona
            if (archivo != null && archivo.Length > 0)
            {
                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(experiencia.CertificadoRuta) && File.Exists(experiencia.CertificadoRuta))
                {
                    File.Delete(experiencia.CertificadoRuta);
                }

                var (ruta, hash) = await GuardarArchivoAsync(archivo);
                experiencia.CertificadoRuta = ruta;
                experiencia.ContenidoHash = hash;
            }

            await _experienciaRepository.UpdateAsync(experiencia);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(experiencia);
        }

        public async Task<bool> DeleteExperienciaAsync(int id)
        {
            var experiencia = await _experienciaRepository.GetByIdAsync(id);
            if (experiencia == null)
            {
                return false;
            }

            // Eliminar archivo si existe
            if (!string.IsNullOrEmpty(experiencia.CertificadoRuta) && File.Exists(experiencia.CertificadoRuta))
            {
                File.Delete(experiencia.CertificadoRuta);
            }

            await _experienciaRepository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AsociarExperienciaASolicitudAsync(AsociarExperienciaSolicitudDto asociarDto)
        {
            var experienciaExists = await _experienciaRepository.ExistsAsync(asociarDto.ExperienciaId);
            if (!experienciaExists)
            {
                return false;
            }

            var solicitudExists = await _solicitudRepository.ExistsAsync(asociarDto.SolicitudId);
            if (!solicitudExists)
            {
                return false;
            }

            await _experienciaRepository.AddToSolicitudAsync(asociarDto.SolicitudId, asociarDto.ExperienciaId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task DesasociarExperienciaDeSolicitudAsync(Guid solicitudId, int experienciaId)
        {
            await _experienciaRepository.RemoveFromSolicitudAsync(solicitudId, experienciaId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<byte[]?> GetArchivoExperienciaAsync(int id)
        {
            var experiencia = await _experienciaRepository.GetByIdAsync(id);
            if (experiencia == null || string.IsNullOrEmpty(experiencia.CertificadoRuta))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(experiencia.CertificadoRuta);
        }

        public async Task<string?> GetNombreArchivoAsync(int id)
        {
            var experiencia = await _experienciaRepository.GetByIdAsync(id);
            if (experiencia == null || string.IsNullOrEmpty(experiencia.CertificadoRuta))
            {
                return null;
            }

            return Path.GetFileName(experiencia.CertificadoRuta);
        }

        private ExperienciaLaboralDto MapToDto(ExperienciaLaboral experiencia)
        {
            return new ExperienciaLaboralDto
            {
                Id = experiencia.Id,
                OrganizacionId = experiencia.OrganizacionId,
                OrganizacionNombre = experiencia.Organizacion?.Nombre ?? string.Empty,
                OrganizacionTipo = experiencia.Organizacion?.TipoOrganizacion ?? string.Empty,
                DocenteCedula = experiencia.DocenteCedula,
                Cargo = experiencia.Cargo,
                FechaInicio = experiencia.FechaInicio,
                FechaFin = experiencia.FechaFin,
                CertificadoRuta = experiencia.CertificadoRuta,
                ContenidoHash = experiencia.ContenidoHash
            };
        }

        private async Task<(string ruta, string hash)> GuardarArchivoAsync(IFormFile archivo)
        {
            // Generar nombre único para el archivo
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaCompleta = Path.Combine(_uploadsPath, nombreArchivo);

            // Guardar archivo
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Calcular hash del archivo
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(rutaCompleta))
            {
                var hash = sha256.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return (rutaCompleta, hashString);
            }
        }
    }
} 