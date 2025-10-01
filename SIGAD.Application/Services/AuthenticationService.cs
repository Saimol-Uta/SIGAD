using Microsoft.Extensions.Logging;
using SIGAD.Application.Contracts.Services;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;

namespace SIGAD.Application.Services
{
    /// <summary>
    /// Servicio para autenticación de usuarios.
    /// Principio SRP: Responsable únicamente del proceso de login.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        private readonly IRangoRepository _rangoRepository;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IUserRegistrationService _userRegistrationService;

        public AuthenticationService(
            ICuentaRepository cuentaRepository,
            ISolicitudAscensoRepository solicitudRepository,
            IRangoRepository rangoRepository,
            ILogger<AuthenticationService> logger,
            ITokenService tokenService,
            IUserRegistrationService userRegistrationService)
        {
            _cuentaRepository = cuentaRepository;
            _solicitudRepository = solicitudRepository;
            _rangoRepository = rangoRepository;
            _logger = logger;
            _tokenService = tokenService;
            _userRegistrationService = userRegistrationService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Iniciando login para correo: {Correo}", loginRequest.Correo);

                // Buscar cuenta con información del docente
                var cuenta = await _cuentaRepository.GetByEmailAsync(loginRequest.Correo);
                if (cuenta == null)
                {
                    _logger.LogWarning("Login fallido: No se encontró cuenta para el correo {Correo}", loginRequest.Correo);
                    return null; // Usuario no encontrado
                }

                _logger.LogInformation("Cuenta encontrada para {Correo}. Rol: {Rol}, DocenteCedula: {Cedula}",
                    cuenta.Correo, cuenta.Rol, cuenta.DocenteCedula);

                // Log del hash almacenado (solo los primeros caracteres por seguridad)
                _logger.LogInformation("Hash almacenado (primeros 20 chars): {HashStart}",
                    cuenta.ClaveHash.Substring(0, Math.Min(20, cuenta.ClaveHash.Length)));

                // Verificar contraseña (delegado a UserRegistrationService)
                _logger.LogInformation("Verificando contraseña para {Correo}...", loginRequest.Correo);
                var passwordValid = _userRegistrationService.VerifyPassword(loginRequest.Clave, cuenta.ClaveHash);
                _logger.LogInformation("Resultado verificación contraseña para {Correo}: {IsValid}",
                    loginRequest.Correo, passwordValid);

                if (!passwordValid)
                {
                    _logger.LogWarning("Login fallido: Contraseña incorrecta para {Correo}", loginRequest.Correo);
                    return null; // Contraseña incorrecta
                }

                // Verificar que el docente existe y está vinculado
                if (cuenta.Docente == null)
                {
                    _logger.LogError("Login fallido: Cuenta {Correo} no tiene docente vinculado", loginRequest.Correo);
                    return null;
                }

                _logger.LogInformation("Docente vinculado encontrado: {Nombre} {Apellido} (Cedula: {Cedula})",
                    cuenta.Docente.Nombre1, cuenta.Docente.Apellido1, cuenta.Docente.Cedula);

                // Obtener rango actual del docente
                var rangoActual = await GetRangoActualDocenteAsync(cuenta.DocenteCedula);

                // Generar token JWT (delegado a TokenService)
                _logger.LogInformation("Generando token JWT para {Correo}...", loginRequest.Correo);
                var token = _tokenService.GenerateJwtToken(
                    cuenta.Correo,
                    cuenta.Rol.ToString(),
                    cuenta.DocenteCedula,
                    cuenta.Docente.Nombre1,
                    cuenta.Docente.Nombre2,
                    cuenta.Docente.Apellido1,
                    cuenta.Docente.Apellido2,
                    rangoActual?.Id,
                    rangoActual?.Nombre
                );
                var expiracion = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

                var response = new LoginResponseDto
                {
                    Token = token,
                    Correo = cuenta.Correo,
                    Rol = cuenta.Rol,
                    ExpiracionToken = expiracion,
                    DocenteInfo = new DocenteInfoDto
                    {
                        Cedula = cuenta.Docente.Cedula,
                        Nombre1 = cuenta.Docente.Nombre1,
                        Nombre2 = cuenta.Docente.Nombre2,
                        Apellido1 = cuenta.Docente.Apellido1,
                        Apellido2 = cuenta.Docente.Apellido2
                    }
                };

                _logger.LogInformation("Login exitoso para {Correo}. Token generado.", loginRequest.Correo);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para correo: {Correo}", loginRequest.Correo);
                return null;
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var cuenta = await _cuentaRepository.GetByEmailAsync(email);
            if (cuenta == null)
                return false;

            return _userRegistrationService.VerifyPassword(password, cuenta.ClaveHash);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return _userRegistrationService.VerifyPassword(password, hash);
        }

        /// <summary>
        /// Método privado para obtener el rango actual de un docente basado en solicitudes aprobadas.
        /// </summary>
        private async Task<Rango?> GetRangoActualDocenteAsync(string docenteCedula)
        {
            try
            {
                _logger.LogInformation("Obteniendo rango actual para docente {Cedula}", docenteCedula);

                // Buscar la última solicitud aprobada del docente
                var todasLasSolicitudes = await _solicitudRepository.GetAllAsync();
                var ultimaSolicitudAprobada = todasLasSolicitudes
                    .Where(s => s.DocenteCedula == docenteCedula && s.Estado == EstadoSolicitud.Aprobada)
                    .OrderByDescending(s => s.FechaResolucion)
                    .FirstOrDefault();

                if (ultimaSolicitudAprobada != null)
                {
                    // Si tiene solicitudes aprobadas, el rango actual es el último rango solicitado aprobado
                    var rango = await _rangoRepository.GetByIdAsync(ultimaSolicitudAprobada.RangoSolicitadoId);
                    _logger.LogInformation("Rango actual encontrado para {Cedula}: {RangoNombre} (ID: {RangoId})",
                        docenteCedula, rango?.Nombre, rango?.Id);
                    return rango;
                }
                else
                {
                    // Si no tiene solicitudes aprobadas, asumir rango nivel 1 por defecto (sin crear solicitud)
                    _logger.LogInformation("Docente {Cedula} sin rango actual - Asumiendo rango nivel 1 por defecto", docenteCedula);
                    var todosLosRangos = await _rangoRepository.GetAllAsync();
                    var rangoNivel1 = todosLosRangos.OrderBy(r => r.Id).FirstOrDefault();

                    if (rangoNivel1 != null)
                    {
                        _logger.LogInformation("Rango nivel 1 asumido para {Cedula}: {RangoNombre} (ID: {RangoId})",
                            docenteCedula, rangoNivel1.Nombre, rangoNivel1.Id);
                    }

                    return rangoNivel1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rango actual para docente {Cedula}", docenteCedula);
                return null;
            }
        }
    }
}
