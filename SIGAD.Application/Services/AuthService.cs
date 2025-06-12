using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;

namespace SIGAD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IDocenteRepository _docenteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ICuentaRepository cuentaRepository,
            IDocenteRepository docenteRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _cuentaRepository = cuentaRepository;
            _docenteRepository = docenteRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

                // Crear la cuenta
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

                // Verificar contraseña
                _logger.LogInformation("Verificando contraseña para {Correo}...", loginRequest.Correo);
                var passwordValid = VerifyPassword(loginRequest.Clave, cuenta.ClaveHash);
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

                // Generar token JWT
                _logger.LogInformation("Generando token JWT para {Correo}...", loginRequest.Correo);
                var token = GenerateJwtToken(cuenta.Correo, cuenta.Rol.ToString(), cuenta.DocenteCedula);
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

        public string GenerateJwtToken(string correo, string rol, string cedula)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
            var issuer = jwtSettings["Issuer"] ?? "SIGAD.API";
            var audience = jwtSettings["Audience"] ?? "SIGAD.Client";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Role, rol),
                new Claim("cedula", cedula),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
    }
}