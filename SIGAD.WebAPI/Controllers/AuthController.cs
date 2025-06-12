using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System.ComponentModel.DataAnnotations;
using SIGAD.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly SigadDbContext _context;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, SigadDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// TEMPORAL: Verifica la conexión a la base de datos
        /// </summary>
        /// <returns>Estado de la conexión y información de la base de datos</returns>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                // Verificar conexión
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "No se puede conectar a la base de datos",
                        connectionString = _context.Database.GetConnectionString()?.Replace("Password=", "Password=***")
                    });
                }

                // Verificar si existen las tablas
                var docentesCount = await _context.Docentes.CountAsync();
                var cuentasCount = await _context.Cuentas.CountAsync();

                return Ok(new
                {
                    success = true,
                    message = "Conexión exitosa a la base de datos",
                    data = new
                    {
                        canConnect = true,
                        database = _context.Database.GetDbConnection().Database,
                        server = _context.Database.GetDbConnection().DataSource,
                        docentes = docentesCount,
                        cuentas = cuentasCount,
                        timestamp = DateTime.Now
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar conexión a la base de datos");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al conectar con la base de datos",
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// TEMPORAL: Muestra todos los datos en la base (SOLO DESARROLLO)
        /// </summary>
        /// <returns>Lista de docentes y cuentas</reaturns>
        [HttpGet("debug-data")]
        public async Task<IActionResult> DebugData()
        {
            try
            {
                var docentes = await _context.Docentes.ToListAsync();
                var cuentas = await _context.Cuentas.ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Datos en la base de datos",
                    data = new
                    {
                        docentes = docentes.Select(d => new
                        {
                            d.Cedula,
                            d.Nombre1,
                            d.Nombre2,
                            d.Apellido1,
                            d.Apellido2
                        }),
                        cuentas = cuentas.Select(c => new
                        {
                            c.Correo,
                            c.DocenteCedula,
                            c.Rol,
                            ClaveHashStart = c.ClaveHash.Substring(0, Math.Min(10, c.ClaveHash.Length)) + "..."
                        }),
                        timestamp = DateTime.Now
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos de debug");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al obtener datos",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// TEMPORAL: Genera un hash para una contraseña (SOLO PARA DESARROLLO)
        /// </summary>
        /// <param name="password">Contraseña a hashear</param>
        /// <returns>Hash de la contraseña</returns>
        [HttpPost("generate-hash")]
        public IActionResult GenerateHash([FromBody] string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password is required");
            }

            var hash = _authService.HashPassword(password);

            return Ok(new
            {
                password = password,
                hash = hash,
                message = "ESTE ENDPOINT ES SOLO PARA DESARROLLO - ELIMINAR EN PRODUCCIÓN"
            });
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema
        /// </summary>
        /// <param name="registerRequest">Datos del nuevo usuario</param>
        /// <returns>Resultado del registro</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                _logger.LogInformation("Intento de registro para el correo: {Correo}", registerRequest.Correo);

                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de entrada inválidos",
                        errors = errors
                    });
                }

                // Intentar registrar
                var result = await _authService.RegisterAsync(registerRequest);
                if (!result)
                {
                    _logger.LogWarning("Registro fallido para el correo: {Correo} - Usuario ya existe", registerRequest.Correo);
                    return Conflict(new
                    {
                        success = false,
                        message = "El correo o cédula ya están registrados"
                    });
                }

                _logger.LogInformation("Registro exitoso para el correo: {Correo}", registerRequest.Correo);
                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "Usuario registrado exitosamente",
                    data = new
                    {
                        correo = registerRequest.Correo,
                        cedula = registerRequest.Cedula,
                        rol = registerRequest.Rol,
                        nombreCompleto = $"{registerRequest.Nombre1} {registerRequest.Nombre2} {registerRequest.Apellido1} {registerRequest.Apellido2}".Replace("  ", " ").Trim()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro para el correo: {Correo}", registerRequest.Correo);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Autentica un usuario y devuelve un token JWT
        /// </summary>
        /// <param name="loginRequest">Datos de login (correo y contraseña)</param>
        /// <returns>Token JWT y información del usuario autenticado</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Intento de login para el correo: {Correo}", loginRequest.Correo);

                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Datos de entrada inválidos",
                        errors = errors
                    });
                }

                // Intentar autenticar
                var result = await _authService.LoginAsync(loginRequest);
                if (result == null)
                {
                    _logger.LogWarning("Login fallido para el correo: {Correo}", loginRequest.Correo);
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Credenciales inválidas"
                    });
                }

                _logger.LogInformation("Login exitoso para el correo: {Correo}", loginRequest.Correo);
                return Ok(result); // Retornar directamente el LoginResponseDto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login para el correo: {Correo}", loginRequest.Correo);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "Error interno del servidor"
                });
            }
        }

        /// <summary>
        /// Verifica si el token JWT es válido
        /// </summary>
        /// <returns>Información del token</returns>
        [HttpGet("verify")]
        [Authorize]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyToken()
        {
            var token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Token no proporcionado"
                });
            }

            // El middleware de JWT ya validó el token si llegamos aquí
            var correo = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var rol = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var cedula = User.FindFirst("cedula")?.Value;

            return Ok(new
            {
                success = true,
                message = "Token válido",
                data = new
                {
                    correo = correo,
                    rol = rol,
                    cedula = cedula,
                    isAuthenticated = true
                }
            });
        }

        /// <summary>
        /// Endpoint para logout (en el lado del cliente se debe eliminar el token)
        /// </summary>
        /// <returns>Mensaje de logout</returns>
        [HttpPost("logout")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            return Ok(new
            {
                success = true,
                message = "Logout exitoso. Elimine el token del almacenamiento local."
            });
        }

        /// <summary>
        /// TEMPORAL: Crea usuarios de prueba con hashes correctos (SOLO DESARROLLO)
        /// </summary>
        /// <returns>Resultado de la creación</returns>
        [HttpPost("create-test-users")]
        public async Task<IActionResult> CreateTestUsers()
        {
            try
            {
                _logger.LogInformation("Iniciando creación de usuarios de prueba...");

                // Limpiar usuarios existentes de prueba
                var existingCuentas = await _context.Cuentas
                    .Where(c => c.Correo.Contains("@sigad.edu.co"))
                    .ToListAsync();

                var existingDocentes = await _context.Docentes
                    .Where(d => new[] { "1234567890", "0987654321", "1122334455" }.Contains(d.Cedula))
                    .ToListAsync();

                if (existingCuentas.Any())
                {
                    _context.Cuentas.RemoveRange(existingCuentas);
                    _logger.LogInformation("Eliminando {Count} cuentas existentes", existingCuentas.Count);
                }

                if (existingDocentes.Any())
                {
                    _context.Docentes.RemoveRange(existingDocentes);
                    _logger.LogInformation("Eliminando {Count} docentes existentes", existingDocentes.Count);
                }

                await _context.SaveChangesAsync();

                // Generar hash con el sistema actual
                var password = "123456";
                var hash = _authService.HashPassword(password);
                _logger.LogInformation("Hash generado para '123456': {Hash}", hash.Substring(0, 20) + "...");

                // Crear usuarios de prueba
                var testUsers = new[]
                {
                    new { Cedula = "1234567890", Nombre1 = "Juan", Nombre2 = "Carlos", Apellido1 = "Pérez", Apellido2 = "González", Correo = "admin@sigad.edu.co", Rol = "ADMINISTRADOR" },
                    new { Cedula = "0987654321", Nombre1 = "María", Nombre2 = "Elena", Apellido1 = "Rodríguez", Apellido2 = "López", Correo = "docente1@sigad.edu.co", Rol = "DOCENTE" },
                    new { Cedula = "1122334455", Nombre1 = "Pedro", Nombre2 = (string?)null, Apellido1 = "Martínez", Apellido2 = "Hernández", Correo = "docente2@sigad.edu.co", Rol = "DOCENTE" }
                };

                foreach (var user in testUsers)
                {
                    _logger.LogInformation("Creando usuario: {Correo}", user.Correo);

                    var registerRequest = new RegisterRequestDto
                    {
                        Correo = user.Correo,
                        Clave = password,
                        Cedula = user.Cedula,
                        Nombre1 = user.Nombre1,
                        Nombre2 = user.Nombre2,
                        Apellido1 = user.Apellido1,
                        Apellido2 = user.Apellido2,
                        Rol = user.Rol
                    };

                    var result = await _authService.RegisterAsync(registerRequest);
                    if (!result)
                    {
                        _logger.LogError("Error al crear usuario {Correo}", user.Correo);
                        return StatusCode(500, new
                        {
                            success = false,
                            message = $"Error al crear usuario {user.Correo}"
                        });
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = "Usuarios de prueba creados exitosamente",
                    data = new
                    {
                        password = password,
                        hashUsed = hash.Substring(0, 20) + "...",
                        users = testUsers.Select(u => new
                        {
                            correo = u.Correo,
                            rol = u.Rol,
                            cedula = u.Cedula,
                            nombre = $"{u.Nombre1} {u.Nombre2} {u.Apellido1} {u.Apellido2}".Replace("  ", " ").Trim()
                        })
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuarios de prueba");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear usuarios de prueba",
                    error = ex.Message
                });
            }
        }
    }
}