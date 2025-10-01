using Microsoft.Extensions.Logging;
using SIGAD.Application.Contracts.Services;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;

namespace SIGAD.Application.Services
{
    /// <summary>
    /// Servicio para registro de nuevos usuarios.
    /// Principio SRP: Responsable únicamente del proceso de registro.
    /// </summary>
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserRegistrationService> _logger;

        public UserRegistrationService(
            ICuentaRepository cuentaRepository,
            IDocenteRepository docenteRepository,
            IUnitOfWork unitOfWork,
            ILogger<UserRegistrationService> logger)
        {
            _cuentaRepository = cuentaRepository;
            _docenteRepository = docenteRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                _logger.LogInformation("Iniciando registro para correo: {Correo}, cedula: {Cedula}",
                    registerRequest.Correo, registerRequest.Cedula);

                // Verificar si ya existe el correo
                var correoExists = await _cuentaRepository.ExistsByEmailAsync(registerRequest.Correo);
                _logger.LogInformation("¿Correo {Correo} ya existe? {Exists}", registerRequest.Correo, correoExists);

                if (correoExists)
                {
                    _logger.LogWarning("Registro fallido: Correo {Correo} ya existe", registerRequest.Correo);
                    return false; // Correo ya existe
                }

                // Verificar si ya existe la cédula
                var cedulaExists = await _docenteRepository.ExistsByCedulaAsync(registerRequest.Cedula);
                _logger.LogInformation("¿Cédula {Cedula} ya existe? {Exists}", registerRequest.Cedula, cedulaExists);

                if (cedulaExists)
                {
                    _logger.LogWarning("Registro fallido: Cédula {Cedula} ya existe", registerRequest.Cedula);
                    return false; // Cédula ya existe
                }

                // Crear el docente
                var docente = new Docente
                {
                    Cedula = registerRequest.Cedula,
                    Nombre1 = registerRequest.Nombre1,
                    Nombre2 = registerRequest.Nombre2,
                    Apellido1 = registerRequest.Apellido1,
                    Apellido2 = registerRequest.Apellido2
                };

                _logger.LogInformation("Creando docente: {Cedula} - {Nombre}",
                    docente.Cedula, $"{docente.Nombre1} {docente.Apellido1}");

                // Crear la cuenta con hash de contraseña
                var cuenta = new Cuenta
                {
                    Correo = registerRequest.Correo,
                    ClaveHash = HashPassword(registerRequest.Clave),
                    DocenteCedula = registerRequest.Cedula,
                    Rol = Enum.Parse<Rol>(registerRequest.Rol)
                };

                _logger.LogInformation("Creando cuenta: {Correo} con rol {Rol}",
                    cuenta.Correo, cuenta.Rol);

                // Guardar en la base de datos
                _logger.LogInformation("Agregando docente a repositorio...");
                await _docenteRepository.AddAsync(docente);

                _logger.LogInformation("Agregando cuenta a repositorio...");
                await _cuentaRepository.AddAsync(cuenta);

                _logger.LogInformation("Guardando cambios en la base de datos...");
                var savedRecords = await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("SaveChanges completado. Registros afectados: {Count}", savedRecords);

                if (savedRecords > 0)
                {
                    _logger.LogInformation("Registro exitoso para {Correo}", registerRequest.Correo);
                    return true;
                }
                else
                {
                    _logger.LogWarning("SaveChanges retornó 0 registros afectados para {Correo}", registerRequest.Correo);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro para correo: {Correo}, cedula: {Cedula}",
                    registerRequest.Correo, registerRequest.Cedula);
                return false;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                // TEMPORAL: Para testing, verificar si es texto plano
                if (hash == password)
                {
                    _logger.LogWarning("ALERTA DE SEGURIDAD: Contraseña en texto plano detectada para testing");
                    return true;
                }

                // Verificación normal con BCrypt
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _cuentaRepository.ExistsByEmailAsync(email);
        }
    }
}
