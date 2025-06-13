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
            var nombre1 = User.FindFirst("nombre1")?.Value;
            var nombre2 = User.FindFirst("nombre2")?.Value;
            var apellido1 = User.FindFirst("apellido1")?.Value;
            var apellido2 = User.FindFirst("apellido2")?.Value;
            var nombreCompleto = User.FindFirst("nombreCompleto")?.Value;
            var rangoId = User.FindFirst("rangoId")?.Value;
            var rangoNombre = User.FindFirst("rangoNombre")?.Value;

            return Ok(new
            {
                success = true,
                message = "Token válido",
                data = new
                {
                    correo = correo,
                    rol = rol,
                    cedula = cedula,
                    nombre1 = nombre1,
                    nombre2 = nombre2,
                    apellido1 = apellido1,
                    apellido2 = apellido2,
                    nombreCompleto = nombreCompleto,
                    rangoId = rangoId,
                    rangoNombre = rangoNombre,
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
        /// TEMPORAL: Crea rangos de prueba (SOLO DESARROLLO)
        /// </summary>
        /// <returns>Resultado de la creación</returns>
        [HttpPost("create-test-rangos")]
        public async Task<IActionResult> CreateTestRangos()
        {
            try
            {
                _logger.LogInformation("Iniciando creación de rangos de prueba...");

                // Limpiar rangos existentes si existen
                var existingRangos = await _context.Rangos.ToListAsync();
                if (existingRangos.Any())
                {
                    _context.Rangos.RemoveRange(existingRangos);
                    _logger.LogInformation("Eliminando {Count} rangos existentes", existingRangos.Count);
                    await _context.SaveChangesAsync();
                }

                // Crear rangos de prueba (sin IDs específicos - dejar que la DB los genere)
                var testRangos = new[]
                {
                    new { Nombre = "Instructor", ArticulosRequeridos = 0, AniosExperienciaRequeridos = 0, HorasCursoRequeridas = 0, MesesInvestigacionRequeridos = 0, PuntajePromedioEvaluacionesRequerido = 0.0m },
                    new { Nombre = "Profesor Asistente", ArticulosRequeridos = 2, AniosExperienciaRequeridos = 2, HorasCursoRequeridas = 40, MesesInvestigacionRequeridos = 12, PuntajePromedioEvaluacionesRequerido = 70.0m },
                    new { Nombre = "Profesor Asociado", ArticulosRequeridos = 5, AniosExperienciaRequeridos = 5, HorasCursoRequeridas = 80, MesesInvestigacionRequeridos = 24, PuntajePromedioEvaluacionesRequerido = 75.0m },
                    new { Nombre = "Profesor Titular", ArticulosRequeridos = 10, AniosExperienciaRequeridos = 10, HorasCursoRequeridas = 120, MesesInvestigacionRequeridos = 36, PuntajePromedioEvaluacionesRequerido = 80.0m }
                };

                var rangosCreados = new List<object>();

                foreach (var rango in testRangos)
                {
                    _logger.LogInformation("Creando rango: {Nombre}", rango.Nombre);

                    var newRango = new SIGAD.Domain.Entities.Rango
                    {
                        // No establecemos Id - dejar que Entity Framework lo genere automáticamente
                        Nombre = rango.Nombre,
                        ArticulosRequeridos = rango.ArticulosRequeridos,
                        AniosExperienciaRequeridos = rango.AniosExperienciaRequeridos,
                        HorasCursoRequeridas = rango.HorasCursoRequeridas,
                        MesesInvestigacionRequeridos = rango.MesesInvestigacionRequeridos,
                        PuntajePromedioEvaluacionesRequerido = rango.PuntajePromedioEvaluacionesRequerido
                    };

                    _context.Rangos.Add(newRango);
                }

                var savedRecords = await _context.SaveChangesAsync();

                // Obtener los rangos creados con sus IDs generados
                var rangosEnDb = await _context.Rangos.OrderBy(r => r.Id).ToListAsync();
                rangosCreados = rangosEnDb.Select(r => new
                {
                    id = r.Id,
                    nombre = r.Nombre,
                    articulosRequeridos = r.ArticulosRequeridos,
                    aniosExperiencia = r.AniosExperienciaRequeridos,
                    horasCurso = r.HorasCursoRequeridas,
                    mesesInvestigacion = r.MesesInvestigacionRequeridos,
                    puntajePromedio = r.PuntajePromedioEvaluacionesRequerido
                }).ToList<object>();
                _logger.LogInformation("Rangos creados exitosamente. Registros guardados: {Count}", savedRecords);

                return Ok(new
                {
                    success = true,
                    message = "Rangos de prueba creados exitosamente",
                    data = new
                    {
                        rangosCreados = savedRecords,
                        rangos = rangosCreados
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rangos de prueba");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear rangos de prueba",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// TEMPORAL: Crea una solicitud de ascenso aprobada para un docente (SOLO DESARROLLO)
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <param name="rangoId">ID del rango a asignar</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost("create-test-ascenso/{cedula}/{rangoId}")]
        public async Task<IActionResult> CreateTestAscenso(string cedula, int rangoId)
        {
            try
            {
                _logger.LogInformation("Creando solicitud de ascenso de prueba para docente {Cedula} al rango {RangoId}", cedula, rangoId);

                // Verificar que el docente existe
                var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.Cedula == cedula);
                if (docente == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Docente con cédula {cedula} no encontrado"
                    });
                }

                // Verificar que el rango existe
                var rango = await _context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoId);
                if (rango == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Rango con ID {rangoId} no encontrado"
                    });
                }

                // Crear solicitud de ascenso aprobada
                var solicitud = new SIGAD.Domain.Entities.SolicitudAscenso(
     cedula,
     null, // Primer rango
     rangoId,
     DateTime.UtcNow.AddDays(-30), // Hace 30 días
     DateTime.UtcNow.AddDays(-25),   // Hace 25 días
     DateTime.UtcNow.AddDays(-1), // Hace 1 día
     SIGAD.Domain.Enums.EstadoSolicitud.Aprobada,
     "Solicitud de prueba - Aprobada automáticamente"
 );

                _context.SolicitudesAscenso.Add(solicitud);
                var savedRecords = await _context.SaveChangesAsync();

                _logger.LogInformation("Solicitud de ascenso creada exitosamente. ID: {SolicitudId}", solicitud.Id);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud de ascenso de prueba creada exitosamente",
                    data = new
                    {
                        solicitudId = solicitud.Id,
                        docenteCedula = cedula,
                        rangoAsignado = rango.Nombre,
                        rangoId = rangoId,
                        fechaAprobacion = solicitud.FechaResolucion
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud de ascenso de prueba");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear solicitud de ascenso de prueba",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// TEMPORAL: Crea una solicitud de ascenso con lógica de rango automático (SOLO DESARROLLO)
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <param name="rangoSolicitadoId">ID del rango solicitado</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost("create-solicitud-con-logica/{cedula}/{rangoSolicitadoId}")]
        public async Task<IActionResult> CreateSolicitudConLogica(string cedula, int rangoSolicitadoId)
        {
            try
            {
                _logger.LogInformation("Creando solicitud con lógica automática para docente {Cedula} al rango {RangoId}", cedula, rangoSolicitadoId);

                // Verificar que el docente existe
                var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.Cedula == cedula);
                if (docente == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Docente con cédula {cedula} no encontrado"
                    });
                }

                // Verificar que el rango solicitado existe
                var rangoSolicitado = await _context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoSolicitadoId);
                if (rangoSolicitado == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Rango solicitado con ID {rangoSolicitadoId} no encontrado"
                    });
                }

                // AQUÍ ESTÁ LA LÓGICA QUE PEDISTE:
                // Buscar el rango actual del docente
                var solicitudesAprobadas = await _context.SolicitudesAscenso
                    .Where(s => s.DocenteCedula == cedula && s.Estado == SIGAD.Domain.Enums.EstadoSolicitud.Aprobada)
                    .OrderByDescending(s => s.FechaResolucion)
                    .FirstOrDefaultAsync();

                int? rangoActualId = null;
                string rangoActualNombre = "Sin rango previo";

                if (solicitudesAprobadas != null)
                {
                    // Tiene rango actual de solicitudes previas
                    rangoActualId = solicitudesAprobadas.RangoSolicitadoId;
                    var rangoActual = await _context.Rangos.FirstOrDefaultAsync(r => r.Id == rangoActualId);
                    rangoActualNombre = rangoActual?.Nombre ?? "Rango desconocido";
                }
                else
                {
                    // NO TIENE RANGO ACTUAL (null) - ASIGNAR AUTOMÁTICAMENTE RANGO NIVEL 1
                    var rangoNivel1 = await _context.Rangos.OrderBy(r => r.Id).FirstOrDefaultAsync();
                    if (rangoNivel1 != null)
                    {
                        rangoActualId = rangoNivel1.Id;
                        rangoActualNombre = rangoNivel1.Nombre + " (asignado automáticamente)";
                        _logger.LogInformation("Docente {Cedula} sin rango previo - Asignando automáticamente rango nivel 1: {RangoNombre}",
                            cedula, rangoNivel1.Nombre);
                    }
                }

                // Crear la solicitud con el rango actual (ya sea real o asignado automáticamente)
                var solicitud = new SIGAD.Domain.Entities.SolicitudAscenso(
                    cedula,
                    rangoActualId ?? 1, // Use 1 as default if null
                    rangoSolicitadoId
                );

                _context.SolicitudesAscenso.Add(solicitud);
                var savedRecords = await _context.SaveChangesAsync();

                _logger.LogInformation("Solicitud creada exitosamente. ID: {SolicitudId}", solicitud.Id);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud de ascenso creada exitosamente con lógica automática",
                    data = new
                    {
                        solicitudId = solicitud.Id,
                        docenteCedula = cedula,
                        rangoActual = new { id = rangoActualId, nombre = rangoActualNombre },
                        rangoSolicitado = new { id = rangoSolicitadoId, nombre = rangoSolicitado.Nombre },
                        estado = solicitud.Estado.ToString(),
                        observaciones = solicitud.ObservacionesAdmin
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud con lógica automática");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear solicitud con lógica automática",
                    error = ex.Message
                });
            }
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

        /// <summary>
        /// Verifica si el docente autenticado tiene una solicitud de ascenso activa
        /// Una solicitud está activa si está en estado: Borrador, Enviada o EnRevision
        /// </summary>
        /// <returns>Estado de la solicitud activa</returns>
        [HttpGet("verificar-solicitud-activa")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> VerificarSolicitudActiva()
        {
            try
            {
                // Obtener cédula del token
                var cedulaClaim = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(cedulaClaim))
                {
                    return BadRequest(new { success = false, message = "No se pudo obtener la información del usuario" });
                }

                // Estados considerados como "activos" - el docente no puede crear otra solicitud
                var estadosActivos = new[]
                {
                    SIGAD.Domain.Enums.EstadoSolicitud.Borrador,
                    SIGAD.Domain.Enums.EstadoSolicitud.Enviada,
                    SIGAD.Domain.Enums.EstadoSolicitud.EnRevision
                };

                // Verificar que no tiene una solicitud activa
                var solicitudActiva = await _context.SolicitudesAscenso
                    .Include(s => s.RangoSolicitado)
                    .FirstOrDefaultAsync(s => s.DocenteCedula == cedulaClaim && estadosActivos.Contains(s.Estado));

                if (solicitudActiva != null)
                {
                    return Ok(new
                    {
                        success = true,
                        tieneSolicitudActiva = true,
                        solicitudId = solicitudActiva.Id,
                        estado = solicitudActiva.Estado.ToString(),
                        estadoDescripcion = GetEstadoDescripcion(solicitudActiva.Estado),
                        fechaCreacion = solicitudActiva.FechaCreacion,
                        fechaEnvio = solicitudActiva.FechaEnvio,
                        rangoSolicitado = solicitudActiva.RangoSolicitado.Nombre,
                        mensaje = GetMensajeEstadoSolicitud(solicitudActiva.Estado)
                    });
                }

                return Ok(new
                {
                    success = true,
                    tieneSolicitudActiva = false,
                    solicitudId = (string?)null,
                    mensaje = "No tiene solicitudes activas. Puede iniciar una nueva solicitud de ascenso."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar solicitud activa");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Crea una nueva solicitud de ascenso para el docente autenticado
        /// </summary>
        /// <param name="request">Datos de la solicitud</param>
        /// <returns>Resultado de la creación</returns>
        [HttpPost("crear-solicitud")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> CrearSolicitudAscenso([FromBody] CrearSolicitudRequestDto request)
        {
            try
            {
                // Obtener cédula del token
                var cedulaClaim = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(cedulaClaim))
                {
                    return BadRequest(new { success = false, message = "No se pudo obtener la información del usuario" });
                }

                // Verificar que el docente existe
                var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.Cedula == cedulaClaim);
                if (docente == null)
                {
                    return NotFound(new { success = false, message = "Docente no encontrado" });
                }

                // Verificar que el rango solicitado existe
                var rangoSolicitado = await _context.Rangos.FirstOrDefaultAsync(r => r.Id == request.RangoSolicitadoId);
                if (rangoSolicitado == null)
                {
                    return NotFound(new { success = false, message = "Rango solicitado no encontrado" });
                }

                // Estados considerados como "activos" - el docente no puede crear otra solicitud
                var estadosActivos = new[]
                {
                    SIGAD.Domain.Enums.EstadoSolicitud.Borrador,
                    SIGAD.Domain.Enums.EstadoSolicitud.Enviada,
                    SIGAD.Domain.Enums.EstadoSolicitud.EnRevision
                };

                // Verificar que no tiene una solicitud activa
                var solicitudActiva = await _context.SolicitudesAscenso
                    .Include(s => s.RangoSolicitado)
                    .FirstOrDefaultAsync(s => s.DocenteCedula == cedulaClaim && estadosActivos.Contains(s.Estado));

                if (solicitudActiva != null)
                {
                    var mensajeDetallado = GetMensajeEstadoSolicitud(solicitudActiva.Estado);
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = $"No puede crear una nueva solicitud. {mensajeDetallado}", 
                        solicitudId = solicitudActiva.Id,
                        estado = solicitudActiva.Estado.ToString(),
                        rangoSolicitado = solicitudActiva.RangoSolicitado.Nombre
                    });
                }

                // Determinar rango actual
                var rangoActual = await GetRangoActualInfoAsync(cedulaClaim);

                // Crear nueva solicitud
                var nuevaSolicitud = new SIGAD.Domain.Entities.SolicitudAscenso(
             cedulaClaim,
             rangoActual.rangoId ?? 1,
             request.RangoSolicitadoId,
             rangoActual.rangoId != null ? null : "Rango actual asignado automáticamente (nivel 1) - Docente sin ascensos previos"
            );

                _context.SolicitudesAscenso.Add(nuevaSolicitud);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Solicitud de ascenso creada. ID: {SolicitudId}, Docente: {Cedula}", nuevaSolicitud.Id, cedulaClaim);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud de ascenso creada exitosamente",
                    data = new
                    {
                        solicitudId = nuevaSolicitud.Id,
                        docenteCedula = cedulaClaim,
                        rangoActual = new { id = rangoActual.rangoId, nombre = rangoActual.rangoNombre },
                        rangoSolicitado = new { id = request.RangoSolicitadoId, nombre = rangoSolicitado.Nombre },
                        estado = nuevaSolicitud.Estado.ToString(),
                        fechaCreacion = nuevaSolicitud.FechaCreacion
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud de ascenso");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Envía la solicitud de ascenso cambiando su estado de Borrador a Enviada
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a enviar</param>
        /// <returns>Resultado del envío</returns>
        [HttpPost("enviar-solicitud/{solicitudId}")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> EnviarSolicitudAscenso(Guid solicitudId)
        {
            try
            {
                // Obtener cédula del token
                var cedulaClaim = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(cedulaClaim))
                {
                    return BadRequest(new { success = false, message = "No se pudo obtener la información del usuario" });
                }

                // Buscar la solicitud
                var solicitud = await _context.SolicitudesAscenso
                    .Include(s => s.RangoSolicitado)
                    .FirstOrDefaultAsync(s => s.Id == solicitudId && s.DocenteCedula == cedulaClaim);

                if (solicitud == null)
                {
                    return NotFound(new { success = false, message = "Solicitud no encontrada" });
                }

                // Verificar que la solicitud está en estado Borrador
                if (solicitud.Estado != SIGAD.Domain.Enums.EstadoSolicitud.Borrador)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"La solicitud no puede ser enviada. Estado actual: {solicitud.Estado}"
                    });
                }

                // Verificar que la solicitud tiene documentos asociados
                var tieneDocumentos = await VerificarDocumentosAsync(solicitudId);
                if (!tieneDocumentos.tieneDocumentos)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "La solicitud debe tener al menos un documento de cada tipo requerido antes de ser enviada",
                        documentosFaltantes = tieneDocumentos.documentosFaltantes
                    });
                }

                // Cambiar estado a Enviada y establecer fecha de envío
                solicitud.Estado = SIGAD.Domain.Enums.EstadoSolicitud.Enviada;
                solicitud.FechaEnvio = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Solicitud enviada exitosamente. ID: {SolicitudId}, Docente: {Cedula}", solicitudId, cedulaClaim);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud enviada exitosamente",
                    data = new
                    {
                        solicitudId = solicitud.Id,
                        estado = solicitud.Estado.ToString(),
                        fechaEnvio = solicitud.FechaEnvio,
                        rangoSolicitado = solicitud.RangoSolicitado.Nombre
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud de ascenso");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Cancela una solicitud en estado Borrador
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a cancelar</param>
        /// <returns>Resultado de la cancelación</returns>
        [HttpDelete("cancelar-solicitud/{solicitudId}")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> CancelarSolicitudAscenso(Guid solicitudId)
        {
            try
            {
                // Obtener cédula del token
                var cedulaClaim = User.FindFirst("cedula")?.Value;
                if (string.IsNullOrEmpty(cedulaClaim))
                {
                    return BadRequest(new { success = false, message = "No se pudo obtener la información del usuario" });
                }

                // Buscar la solicitud
                var solicitud = await _context.SolicitudesAscenso
                    .FirstOrDefaultAsync(s => s.Id == solicitudId && s.DocenteCedula == cedulaClaim);

                if (solicitud == null)
                {
                    return NotFound(new { success = false, message = "Solicitud no encontrada" });
                }

                // Solo permitir cancelar solicitudes en estado Borrador
                if (solicitud.Estado != SIGAD.Domain.Enums.EstadoSolicitud.Borrador)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Solo se pueden cancelar solicitudes en borrador. Estado actual: {solicitud.Estado}"
                    });
                }

                // Eliminar la solicitud y sus asociaciones
                _context.SolicitudesAscenso.Remove(solicitud);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Solicitud cancelada exitosamente. ID: {SolicitudId}, Docente: {Cedula}", solicitudId, cedulaClaim);

                return Ok(new
                {
                    success = true,
                    message = "Solicitud cancelada exitosamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar solicitud de ascenso");
                return StatusCode(500, new { success = false, message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Verifica que la solicitud tenga documentos requeridos antes de enviarla
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <returns>Estado de verificación de documentos</returns>
        private async Task<(bool tieneDocumentos, List<string> documentosFaltantes)> VerificarDocumentosAsync(Guid solicitudId)
        {
            var documentosFaltantes = new List<string>();

            // Verificar artículos
            var tieneArticulos = await _context.ArticulosPorSolicitud
                .AnyAsync(aps => aps.SolicitudId == solicitudId);
            if (!tieneArticulos)
            {
                documentosFaltantes.Add("Artículos");
            }

            // Verificar cursos
            var tieneCursos = await _context.CursosPorSolicitud
                .AnyAsync(cps => cps.SolicitudId == solicitudId);
            if (!tieneCursos)
            {
                documentosFaltantes.Add("Cursos");
            }

            // Verificar investigaciones
            var tieneInvestigaciones = await _context.InvestigacionesPorSolicitud
                .AnyAsync(ips => ips.SolicitudId == solicitudId);
            if (!tieneInvestigaciones)
            {
                documentosFaltantes.Add("Investigaciones");
            }

            // Verificar experiencias laborales
            var tieneExperiencias = await _context.ExperienciasPorSolicitud
                .AnyAsync(eps => eps.SolicitudId == solicitudId);
            if (!tieneExperiencias)
            {
                documentosFaltantes.Add("Experiencias Laborales");
            }

            // Verificar evaluaciones
            var tieneEvaluaciones = await _context.EvaluacionesPorSolicitud
                .AnyAsync(evps => evps.SolicitudId == solicitudId);
            if (!tieneEvaluaciones)
            {
                documentosFaltantes.Add("Evaluaciones");
            }

            return (documentosFaltantes.Count == 0, documentosFaltantes);
        }

        /// <summary>
        /// Obtiene una descripción amigable del estado de la solicitud
        /// </summary>
        /// <param name="estado">Estado de la solicitud</param>
        /// <returns>Descripción del estado</returns>
        private string GetEstadoDescripcion(SIGAD.Domain.Enums.EstadoSolicitud estado)
        {
            return estado switch
            {
                SIGAD.Domain.Enums.EstadoSolicitud.Borrador => "En preparación",
                SIGAD.Domain.Enums.EstadoSolicitud.Enviada => "Enviada para revisión",
                SIGAD.Domain.Enums.EstadoSolicitud.EnRevision => "En proceso de evaluación",
                SIGAD.Domain.Enums.EstadoSolicitud.Aprobada => "Aprobada",
                SIGAD.Domain.Enums.EstadoSolicitud.Rechazada => "Rechazada",
                _ => "Estado desconocido"
            };
        }

        /// <summary>
        /// Obtiene un mensaje específico según el estado de la solicitud activa
        /// </summary>
        /// <param name="estado">Estado de la solicitud</param>
        /// <returns>Mensaje para mostrar al usuario</returns>
        private string GetMensajeEstadoSolicitud(SIGAD.Domain.Enums.EstadoSolicitud estado)
        {
            return estado switch
            {
                SIGAD.Domain.Enums.EstadoSolicitud.Borrador => "Tiene una solicitud en preparación. Complete los documentos requeridos para enviarla.",
                SIGAD.Domain.Enums.EstadoSolicitud.Enviada => "Su solicitud ha sido enviada y está pendiente de revisión. No puede crear otra solicitud hasta obtener una respuesta.",
                SIGAD.Domain.Enums.EstadoSolicitud.EnRevision => "Su solicitud está siendo evaluada por los administradores. No puede crear otra solicitud hasta obtener una respuesta.",
                _ => "Tiene una solicitud activa en proceso."
            };
        }

        /// <summary>
        /// Obtiene información del rango actual del docente
        /// </summary>
        /// <param name="docenteCedula">Cédula del docente</param>
        /// <returns>Información del rango actual</returns>
        private async Task<(int? rangoId, string rangoNombre)> GetRangoActualInfoAsync(string docenteCedula)
        {
            try
            {
                _logger.LogInformation("Obteniendo rango actual para docente {Cedula}", docenteCedula);

                // Buscar la última solicitud aprobada del docente
                var ultimaSolicitudAprobada = await _context.SolicitudesAscenso
                    .Where(s => s.DocenteCedula == docenteCedula && s.Estado == SIGAD.Domain.Enums.EstadoSolicitud.Aprobada)
                    .OrderByDescending(s => s.FechaResolucion)
                    .FirstOrDefaultAsync();

                if (ultimaSolicitudAprobada != null)
                {
                    // Si tiene solicitudes aprobadas, el rango actual es el último rango solicitado aprobado
                    var rango = await _context.Rangos.FirstOrDefaultAsync(r => r.Id == ultimaSolicitudAprobada.RangoSolicitadoId);
                    if (rango != null)
                    {
                        _logger.LogInformation("Rango actual encontrado para {Cedula}: {RangoNombre} (ID: {RangoId})",
                            docenteCedula, rango.Nombre, rango.Id);
                        return (rango.Id, rango.Nombre);
                    }
                }

                // Si no tiene solicitudes aprobadas, asumir rango nivel 1 por defecto (sin crear solicitud)
                _logger.LogInformation("Docente {Cedula} sin rango actual - Asumiendo rango nivel 1 por defecto", docenteCedula);
                var rangoNivel1 = await _context.Rangos.OrderBy(r => r.Id).FirstOrDefaultAsync();

                if (rangoNivel1 != null)
                {
                    _logger.LogInformation("Rango nivel 1 asumido para {Cedula}: {RangoNombre} (ID: {RangoId})",
                        docenteCedula, rangoNivel1.Nombre, rangoNivel1.Id);
                    return (rangoNivel1.Id, rangoNivel1.Nombre);
                }

                return (null, "Sin rango asignado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rango actual para docente {Cedula}", docenteCedula);
                return (null, "Error al obtener rango");
            }
        }
    }
}