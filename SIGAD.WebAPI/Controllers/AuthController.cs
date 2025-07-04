using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Services;
using SIGAD.Domain.Entities;
using SIGAD.Infrastructure.Persistence;
using System.ComponentModel.DataAnnotations;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class AuthController : ControllerBase
    {
        [HttpPost("verificar-codigo")]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodigo([FromBody] VerificarCodigoDto dto)
        {
            var valido = await _authService.VerificarCodigoAsync(dto.Email, dto.Codigo);

            if (!valido)
            {
                return BadRequest(new { Message = "El código ingresado es incorrecto o ha expirado." });
            }

            return Ok(new { Message = "Código verificado correctamente." });
        }

        [HttpPost("solicitar-recuperacion")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous] // Un usuario sin sesión debe poder usar esto
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] SolicitarRecuperacionDto dto)
        {
            await _authService.SolicitarRecuperacionAsync(dto.Email);

            // Por seguridad, siempre devolvemos una respuesta genérica exitosa
            return Ok(new { Message = "Si su correo electrónico está registrado en nuestro sistema, recibirá un correo con las instrucciones para restablecer su contraseña." });
        }

        [HttpPost("restablecer-contrasena")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> RestablecerContrasena([FromBody] RestablecerContrasenaDto dto)
        {
            var success = await _authService.RestablecerContrasenaAsync(dto.Email, dto.Codigo, dto.NuevaContrasena, dto.ConfirmarContrasena);

            if (!success)
            {
                return BadRequest(new { Message = "El código de recuperación es inválido, ha expirado o el correo es incorrecto." });
            }

            return Ok(new { Message = "Su contraseña ha sido restablecida exitosamente." });
        }

        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly SigadDbContext _context;
        private readonly INotificacionService _notificacionService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, SigadDbContext context, INotificacionService notificacionService)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
            _notificacionService = notificacionService; // <-- Y AQUÍ
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
        /// PRUEBA EL ENVÍO DE CORREO USANDO LA PLANTILLA HTML.
        /// </summary>
        [HttpPost("test-html-email")]
        [AllowAnonymous]
        public async Task<IActionResult> TestHtmlEmail([FromBody] TestHtmlEmailDto dto)
        {
            try
            {
                _logger.LogInformation("Iniciando envío de correo de prueba con plantilla a: {ToEmail}", dto.ToEmail);

                // 1. Creamos un objeto "falso" de SolicitudAscenso con los datos necesarios
                //    para que el NotificacionService pueda trabajar. No viene de la BD.
                var dummySolicitud = new SolicitudAscenso
                {
                    Docente = new Docente
                    {
                        Nombre1 = dto.DocenteNombre,
                        Apellido1 = "", // No es necesario para la plantilla
                        Cuenta = new Cuenta { Correo = dto.ToEmail }
                    },
                    RangoSolicitado = new Rango { Nombre = dto.RangoNombre },
                    DocenteCedula = "1234567890" // Valor de relleno
                };

                // 2. Llamamos al método correspondiente del servicio de notificación
                if (dto.EsAprobacion)
                {
                    await _notificacionService.EnviarNotificacionAprobacionAsync(dummySolicitud, dto.Observaciones);
                }
                else
                {
                    await _notificacionService.EnviarNotificacionRechazoAsync(dummySolicitud, dto.Observaciones);
                }

                return Ok(new
                {
                    success = true,
                    message = $"Intento de envío de correo con plantilla a {dto.ToEmail} completado."
                });
            }
            catch (Exception ex)
            {
                // Si algo falla, este bloque nos dirá exactamente qué es.
                _logger.LogError(ex, "FALLO el envío del correo de prueba con plantilla a {ToEmail}", dto.ToEmail);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al procesar la notificación.",
                    errorType = ex.GetType().Name, // Nos dice si es FileNotFoundException, SmtpException, etc.
                    errorMessage = ex.Message,
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
        }        /// <summary>
                 /// Registra un usuario SOLO con cédula, correo y clave (flujo simplificado)
                 /// </summary>
                 /// <param name="model">Datos mínimos para registro</param>
                 /// <returns>Resultado del registro</returns>
        [HttpPost("register-simple")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSimple([FromBody] RegisterSimpleDto model)
        {
            // Validación básica
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
                return BadRequest(new { success = false, message = "Datos de entrada inválidos", errors });
            }

            // Verificar si ya existe la cuenta
            var cuentaExiste = await _context.Cuentas.AnyAsync(c => c.Correo == model.Correo || c.DocenteCedula == model.Cedula);
            if (cuentaExiste)
            {
                return Conflict(new { success = false, message = "El correo o cédula ya están registrados" });
            }

            // Crear cuenta
            var cuenta = new SIGAD.Domain.Entities.Cuenta
            {
                Correo = model.Correo,
                ClaveHash = _authService.HashPassword(model.Clave),
                DocenteCedula = model.Cedula,
                // Puedes asignar un rol por defecto si lo necesitas
                Rol = Domain.Enums.Rol.DOCENTE
            };
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return StatusCode(StatusCodes.Status201Created, new
            {
                success = true,
                message = "Usuario registrado exitosamente",
                data = new
                {
                    correo = model.Correo,
                    cedula = model.Cedula
                }
            });
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
        /// TEMPORAL: Crea los rangos académicos oficiales según el Reglamento UTA Resolución 0677-CU-P-2023 (SOLO DESARROLLO)
        /// </summary>
        /// <returns>Resultado de la creación de rangos oficiales según el reglamento UTA</returns>
        [HttpPost("create-test-rangos")]
        public async Task<IActionResult> CreateTestRangos()
        {
            try
            {
                _logger.LogInformation("Iniciando creación de rangos académicos según Reglamento UTA Resolución 0677-CU-P-2023...");

                // Limpiar rangos existentes si existen
                var existingRangos = await _context.Rangos.ToListAsync();
                if (existingRangos.Any())
                {
                    _context.Rangos.RemoveRange(existingRangos);
                    _logger.LogInformation("Eliminando {Count} rangos existentes", existingRangos.Count);
                    await _context.SaveChangesAsync();
                }                // Crear rangos según el Reglamento para la Promoción del Personal Académico Titular de la UTA
                // Resolución 0677-CU-P-2023
                var rangosReglamento = new[]
                {
                    // TITULAR AUXILIAR 1 - Rango inicial (sin requisitos previos para promoción)
                    new {
                        Nombre = "Titular Auxiliar 1",
                        ArticulosRequeridos = 0,
                        AniosExperienciaRequeridos = 0,
                        HorasCursoRequeridas = 0,
                        MesesInvestigacionRequeridos = 0,
                        TesisDirigidasRequeridas = 0,
                        PuntajePromedioEvaluacionesRequerido = 0.0m
                    },
                    
                    // TITULAR AUXILIAR 2 - Anexo 1, Página 7
                    new {
                        Nombre = "Titular Auxiliar 2",
                        ArticulosRequeridos = 1,  // 1 obra de relevancia o artículo indexado
                        AniosExperienciaRequeridos = 4,  // 4 años como titular auxiliar 1
                        HorasCursoRequeridas = 96,  // 96 horas de capacitación (25% pedagógica = 24h)
                        MesesInvestigacionRequeridos = 0,  // No especifica proyectos de investigación
                        TesisDirigidasRequeridas = 0,  // No requiere dirección de tesis
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR AGREGADO 1 - Anexo 1, Página 8  
                    new {
                        Nombre = "Titular Agregado 1",
                        ArticulosRequeridos = 2,  // 2 obras de relevancia o artículos indexados
                        AniosExperienciaRequeridos = 4,  // 4 años como titular auxiliar 2
                        HorasCursoRequeridas = 96,  // 96 horas de capacitación (25% pedagógica = 24h)
                        MesesInvestigacionRequeridos = 12,  // 12 meses en proyectos de investigación/vinculación
                        TesisDirigidasRequeridas = 0,  // No requiere dirección de tesis
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR AGREGADO 2 - Anexo 1, Página 9
                    new {
                        Nombre = "Titular Agregado 2",
                        ArticulosRequeridos = 3,  // 3 obras de relevancia o artículos indexados
                        AniosExperienciaRequeridos = 4,  // 4 años como titular agregado 1
                        HorasCursoRequeridas = 128,  // 128 horas de capacitación (25% pedagógica = 32h)
                        MesesInvestigacionRequeridos = 24,  // 24 meses en proyectos de investigación/vinculación
                        TesisDirigidasRequeridas = 0,  // No requiere dirección de tesis
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR AGREGADO 3 - Anexo 1, Página 10
                    new {
                        Nombre = "Titular Agregado 3",
                        ArticulosRequeridos = 5,  // 5 obras de relevancia o artículos indexados
                        AniosExperienciaRequeridos = 4,  // 4 años como titular agregado 2
                        HorasCursoRequeridas = 160,  // 160 horas de capacitación (25% pedagógica = 40h)
                        MesesInvestigacionRequeridos = 24,  // 24 meses en proyectos de investigación/vinculación
                        TesisDirigidasRequeridas = 0,  // No requiere dirección de tesis
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR PRINCIPAL 1 - Anexo 1, Página 11
                    new {
                        Nombre = "Titular Principal 1",
                        ArticulosRequeridos = 8,  // 8 obras de relevancia o artículos indexados (1 en idioma extranjero)
                        AniosExperienciaRequeridos = 3,  // 3 años como titular principal 1
                        HorasCursoRequeridas = 224,  // 224 horas de capacitación (25% pedagógica = 56h) + 40h impartidas
                        MesesInvestigacionRequeridos = 24,  // 24 meses dirigiendo proyectos de investigación
                        TesisDirigidasRequeridas = 2,  // 2 tesis de doctorado dirigidas/codirigidas
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR PRINCIPAL 2 - Anexo 1, Página 12
                    new {
                        Nombre = "Titular Principal 2",
                        ArticulosRequeridos = 12,  // 12 obras de relevancia o artículos indexados (2 en idioma extranjero)
                        AniosExperienciaRequeridos = 3,  // 3 años como titular principal 2
                        HorasCursoRequeridas = 256,  // 256 horas de capacitación (25% pedagógica = 64h) + 80h impartidas
                        MesesInvestigacionRequeridos = 36,  // 36 meses dirigiendo proyectos de investigación
                        TesisDirigidasRequeridas = 3,  // 3 tesis de doctorado dirigidas/codirigidas
                        PuntajePromedioEvaluacionesRequerido = 75.0m  // 75% en evaluación integral
                    },
                    
                    // TITULAR PRINCIPAL 3 - Rango máximo (sin promoción posterior)
                    new {
                        Nombre = "Titular Principal 3",
                        ArticulosRequeridos = 15,  // Estimado para el rango máximo
                        AniosExperienciaRequeridos = 25,  // Estimado para el rango máximo
                        HorasCursoRequeridas = 300,  // Estimado para el rango máximo
                        MesesInvestigacionRequeridos = 48,  // Estimado para el rango máximo
                        TesisDirigidasRequeridas = 5,  // Estimado para el rango máximo
                        PuntajePromedioEvaluacionesRequerido = 80.0m  // Estimado para el rango máximo
                    }
                };

                var rangosCreados = new List<object>();

                foreach (var rango in rangosReglamento)
                {
                    _logger.LogInformation("Creando rango: {Nombre}", rango.Nombre); var newRango = new SIGAD.Domain.Entities.Rango
                    {
                        // No establecemos Id - dejar que Entity Framework lo genere automáticamente
                        Nombre = rango.Nombre,
                        ArticulosRequeridos = rango.ArticulosRequeridos,
                        AniosExperienciaRequeridos = rango.AniosExperienciaRequeridos,
                        HorasCursoRequeridas = rango.HorasCursoRequeridas,
                        MesesInvestigacionRequeridos = rango.MesesInvestigacionRequeridos,
                        TesisDirigidasRequeridas = rango.TesisDirigidasRequeridas,
                        PuntajePromedioEvaluacionesRequerido = rango.PuntajePromedioEvaluacionesRequerido
                    };

                    _context.Rangos.Add(newRango);
                }

                var savedRecords = await _context.SaveChangesAsync();

                // Obtener los rangos creados con sus IDs generados
                var rangosEnDb = await _context.Rangos.OrderBy(r => r.Id).ToListAsync(); rangosCreados = rangosEnDb.Select(r => new
                {
                    id = r.Id,
                    nombre = r.Nombre,
                    articulosRequeridos = r.ArticulosRequeridos,
                    aniosExperiencia = r.AniosExperienciaRequeridos,
                    horasCurso = r.HorasCursoRequeridas,
                    mesesInvestigacion = r.MesesInvestigacionRequeridos,
                    tesisDirigidas = r.TesisDirigidasRequeridas,
                    puntajePromedio = r.PuntajePromedioEvaluacionesRequerido
                }).ToList<object>(); _logger.LogInformation("Rangos académicos UTA creados exitosamente según Resolución 0677-CU-P-2023. Registros guardados: {Count}", savedRecords);

                return Ok(new
                {
                    success = true,
                    message = "Rangos académicos UTA creados exitosamente según Resolución 0677-CU-P-2023",
                    data = new
                    {
                        rangosCreados = savedRecords,
                        rangos = rangosCreados,
                        reglamento = "Reglamento para la Promoción del Personal Académico Titular de la UTA",
                        resolucion = "0677-CU-P-2023",
                        fechaAprobacion = "15 de junio de 2023",
                        fechaCreacion = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rangos académicos UTA");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error al crear rangos académicos UTA",
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
                    new { Cedula = "1122334455", Nombre1 = "Pedro", Nombre2 = "Andes", Apellido1 = "Martínez", Apellido2 = "Hernández", Correo = "docente2@sigad.edu.co", Rol = "DOCENTE" }
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
        /// Verifica si una solicitud cumple con los requisitos del rango antes de enviarla
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud a verificar</param>
        /// <returns>Estado de los requisitos</returns>
        [HttpGet("verificar-requisitos/{solicitudId}")]
        [Authorize(Roles = "DOCENTE")]
        public async Task<IActionResult> VerificarRequisitosSolicitud(Guid solicitudId)
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

                // Verificar requisitos
                var cumpleRequisitos = await VerificarRequisitosRangoAsync(solicitudId, solicitud.RangoSolicitado);

                return Ok(new
                {
                    success = true,
                    cumpleRequisitos = cumpleRequisitos.cumple,
                    requisitosFaltantes = cumpleRequisitos.requisitosFaltantes,
                    valoresActuales = cumpleRequisitos.valoresActuales,
                    valoresRequeridos = cumpleRequisitos.valoresRequeridos,
                    rangoSolicitado = solicitud.RangoSolicitado.Nombre
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar requisitos de solicitud");
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

                // Verificar que la solicitud cumple con los requisitos del rango
                var cumpleRequisitos = await VerificarRequisitosRangoAsync(solicitudId, solicitud.RangoSolicitado);
                if (!cumpleRequisitos.cumple)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No cumple con los requisitos mínimos para este rango",
                        requirementsNotMet = cumpleRequisitos.requisitosFaltantes,
                        currentValues = cumpleRequisitos.valoresActuales,
                        requiredValues = cumpleRequisitos.valoresRequeridos
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
        /// Verifica que la solicitud cumpla con todos los requisitos específicos del rango solicitado
        /// </summary>
        /// <param name="solicitudId">ID de la solicitud</param>
        /// <param name="rango">Rango solicitado con sus requisitos</param>
        /// <returns>Estado de verificación de requisitos</returns>
        private async Task<(bool cumple, List<string> requisitosFaltantes, object valoresActuales, object valoresRequeridos)> VerificarRequisitosRangoAsync(Guid solicitudId, SIGAD.Domain.Entities.Rango rango)
        {
            var requisitosFaltantes = new List<string>();

            // 1. VERIFICAR ARTÍCULOS
            var articulosCount = await _context.ArticulosPorSolicitud
                .CountAsync(aps => aps.SolicitudId == solicitudId);

            // 2. VERIFICAR AÑOS DE EXPERIENCIA LABORAL (suma total)
            var experienciasLaborales = await _context.ExperienciasPorSolicitud
                .Where(eps => eps.SolicitudId == solicitudId)
                .Include(eps => eps.ExperienciaLaboral)
                .Select(eps => eps.ExperienciaLaboral)
                .ToListAsync();

            var totalAniosExperiencia = CalcularTotalAniosExperiencia(experienciasLaborales);

            // 3. VERIFICAR HORAS DE CURSOS SEGÚN EL RANGO SOLICITADO
            var cursos = await _context.CursosPorSolicitud
                .Where(cps => cps.SolicitudId == solicitudId)
                .Include(cps => cps.Curso)
                .Select(cps => cps.Curso)
                .ToListAsync();

            int totalHorasCursos = 0;
            
            // Para rangos 4 y superiores (Agregado 3, Principal 1, 2, 3): usar horas impartidas
            if (rango.Id >= 5) // Asumiendo que rango 5+ son los avanzados
            {
                totalHorasCursos = cursos
                    .Where(c => c.ImpartidoPorDocente && c.HorasImpartidas.HasValue)
                    .Sum(c => c.HorasImpartidas ?? 0);
            }
            else
            {
                // Para rangos 1, 2, 3 (Auxiliar 1, 2 y Agregado 1, 2): usar horas de capacitación recibida
                totalHorasCursos = cursos.Sum(c => c.NumeroHoras);
            }

            // 4. VERIFICAR MESES DE INVESTIGACIÓN SEGÚN REGLAMENTO UTA (con multiplicadores por rol y proyectos internacionales)
            var investigaciones = await _context.InvestigacionesPorSolicitud
                .Where(ips => ips.SolicitudId == solicitudId)
                .Include(ips => ips.Investigacion)
                .Select(ips => ips.Investigacion)
                .ToListAsync();

            var resultadoInvestigacion = CalcularMesesInvestigacionConReglamento(investigaciones, rango);
            var totalMesesInvestigacion = resultadoInvestigacion.mesesTotales;
            var cumpleRequisitosInternacionales = resultadoInvestigacion.cumpleInternacionales;

            // 5. VERIFICAR EVALUACIONES DOCENTES (al menos 75% promedio mínimo)
            var evaluaciones = await _context.EvaluacionesPorSolicitud
                .Where(evps => evps.SolicitudId == solicitudId)
                .Include(evps => evps.Evaluacion)
                .Select(evps => evps.Evaluacion)
                .ToListAsync();

            var promedioEvaluaciones = evaluaciones.Any() ? evaluaciones.Average(e => e?.PuntajePorcentual ?? 0) : 0;
            var todasEvaluacionesCumplen = evaluaciones.All(e => (e?.PuntajePorcentual ?? 0) >= rango.PuntajePromedioEvaluacionesRequerido);

            // 6. VERIFICAR TESIS DIRIGIDAS
            var tesisCount = await _context.TesisPorSolicitud
                .CountAsync(tps => tps.SolicitudId == solicitudId);

            // VALIDAR CADA REQUISITO
            if (articulosCount < rango.ArticulosRequeridos)
            {
                requisitosFaltantes.Add($"Artículos: Tiene {articulosCount}, requiere {rango.ArticulosRequeridos}");
            }

            // Usar tolerancia para comparación de decimales (0.1 años = aproximadamente 1 mes)
            if (totalAniosExperiencia < (rango.AniosExperienciaRequeridos - 0.1m))
            {
                requisitosFaltantes.Add($"Años de experiencia: Tiene {totalAniosExperiencia:F1}, requiere {rango.AniosExperienciaRequeridos}");
            }

            if (totalHorasCursos < rango.HorasCursoRequeridas)
            {
                string tipoHoras = rango.Id >= 5 ? "impartidas" : "de capacitación";
                requisitosFaltantes.Add($"Horas {tipoHoras} en cursos: Tiene {totalHorasCursos}, requiere {rango.HorasCursoRequeridas}");
            }

            if (totalMesesInvestigacion < rango.MesesInvestigacionRequeridos)
            {
                requisitosFaltantes.Add($"Meses de investigación: Tiene {totalMesesInvestigacion:F1}, requiere {rango.MesesInvestigacionRequeridos}");
            }

            // Validar proyectos internacionales para rangos Principal
            if (!cumpleRequisitosInternacionales)
            {
                if (rango.Id == 6) // Principal 1
                {
                    requisitosFaltantes.Add("Principal 1: Debe tener al menos 1 proyecto internacional dirigido/codirigido");
                }
                else if (rango.Id == 7) // Principal 2
                {
                    requisitosFaltantes.Add("Principal 2: Debe tener al menos 1 proyecto internacional dirigido/codirigido (24 meses mínimo)");
                }
                else if (rango.Id == 8) // Principal 3
                {
                    requisitosFaltantes.Add("Principal 3: Debe tener al menos 2 proyectos internacionales dirigidos/codirigidos (36 meses mínimo)");
                }
            }

            // VALIDAR EVALUACIONES DOCENTES - NUEVA LÓGICA ESPECÍFICA
            var validacionEvaluaciones = ValidarEvaluacionesParaSolicitud(evaluaciones);
            
            if (!validacionEvaluaciones.esValida)
            {
                requisitosFaltantes.AddRange(validacionEvaluaciones.errores);
            }

            // VALIDAR TESIS DIRIGIDAS
            if (tesisCount < rango.TesisDirigidasRequeridas)
            {
                requisitosFaltantes.Add($"Tesis dirigidas: Tiene {tesisCount}, requiere {rango.TesisDirigidasRequeridas}");
            }

            // Valores actuales y requeridos para mostrar al usuario
            var valoresActuales = new
            {
                articulos = articulosCount,
                aniosExperiencia = Math.Round(totalAniosExperiencia, 1),
                horasCursos = totalHorasCursos,
                mesesInvestigacion = Math.Round(totalMesesInvestigacion, 1),
                proyectosInternacionales = resultadoInvestigacion.proyectosInternacionales,
                cumpleRequisitosInternacionales = cumpleRequisitosInternacionales,
                promedioEvaluaciones = Math.Round(promedioEvaluaciones, 1),
                totalEvaluaciones = evaluaciones.Count,
                evaluacionesCumplen = evaluaciones.Count(e => (e?.PuntajePorcentual ?? 0) >= rango.PuntajePromedioEvaluacionesRequerido),
                tesisDirigidas = tesisCount
            };

            var valoresRequeridos = new
            {
                articulos = rango.ArticulosRequeridos,
                aniosExperiencia = rango.AniosExperienciaRequeridos,
                horasCursos = rango.HorasCursoRequeridas,
                mesesInvestigacion = rango.MesesInvestigacionRequeridos,
                proyectosInternacionalesRequeridos = GetProyectosInternacionalesRequeridos(rango.Id),
                promedioEvaluaciones = rango.PuntajePromedioEvaluacionesRequerido,
                rangoNombre = rango.Nombre,
                tesisDirigidas = rango.TesisDirigidasRequeridas,
                notaEvaluaciones = "Pueden incluir las evaluaciones que consideren apropiadas, con promedio mínimo requerido",
                notaInvestigacion = "Coordinador Principal = 2x tiempo, Coordinador Subrogante = 1.5x tiempo"
            };

            return (requisitosFaltantes.Count == 0, requisitosFaltantes, valoresActuales, valoresRequeridos);
        }

        /// <summary>
        /// Calcula el total de años de experiencia laboral
        /// </summary>
        private decimal CalcularTotalAniosExperiencia(List<SIGAD.Domain.Entities.ExperienciaLaboral> experiencias)
        {
            decimal totalAnios = 0;

            foreach (var exp in experiencias)
            {
                var fechaFin = exp.FechaFin ?? DateTime.Now; // Si no tiene fecha fin, usar fecha actual
                var diferencia = fechaFin - exp.FechaInicio;
                var anios = (decimal)diferencia.TotalDays / 365.25m; // Considerar años bisiestos
                totalAnios += anios;
            }

            return totalAnios;
        }

        /// <summary>
        /// Calcula el total de meses de investigación
        /// </summary>
        private decimal CalcularTotalMesesInvestigacion(List<SIGAD.Domain.Entities.Investigacion> investigaciones)
        {
            decimal totalMeses = 0;

            foreach (var inv in investigaciones)
            {
                // Usar MesesDeInvestigacion que ya está calculado en la entidad
                totalMeses += inv.MesesDeInvestigacion;
            }

            return totalMeses;
        }

        /// <summary>
        /// Calcula meses de investigación aplicando multiplicadores según el reglamento UTA y valida proyectos internacionales
        /// </summary>
        /// <param name="investigaciones">Lista de investigaciones</param>
        /// <param name="rango">Rango solicitado</param>
        /// <returns>Tupla con meses totales, cantidad de proyectos internacionales y si cumple requisitos</returns>
        private (decimal mesesTotales, int proyectosInternacionales, bool cumpleInternacionales) CalcularMesesInvestigacionConReglamento(
            List<SIGAD.Domain.Entities.Investigacion> investigaciones, 
            SIGAD.Domain.Entities.Rango rango)
        {
            decimal totalMeses = 0;
            int proyectosInternacionales = 0;
            decimal mesesInternacionales = 0;

            foreach (var inv in investigaciones)
            {
                decimal mesesConMultiplicador = inv.MesesDeParticipacion;

                // Aplicar multiplicadores según el rol en investigación
                switch (inv.RolEnInvestigacion?.ToUpper())
                {
                    case "COORDINADOR PRINCIPAL":
                    case "DIRECTOR":
                    case "INVESTIGADOR PRINCIPAL":
                        mesesConMultiplicador *= 2.0m; // Doble tiempo
                        break;
                    case "COORDINADOR SUBROGANTE":
                    case "CODIRECTOR":
                    case "INVESTIGADOR SUBROGANTE":
                        mesesConMultiplicador *= 1.5m; // 1.5x tiempo
                        break;
                    default:
                        // Investigador regular: 1x tiempo (sin multiplicador)
                        break;
                }

                totalMeses += mesesConMultiplicador;

                // Contar proyectos internacionales
                if (inv.EsInternacional)
                {
                    proyectosInternacionales++;
                    mesesInternacionales += mesesConMultiplicador;
                }
            }

            // Validar requisitos internacionales según el rango
            bool cumpleInternacionales = true;
            
            switch (rango.Id)
            {
                case 6: // Principal 1 → Principal 2
                    // Requiere al menos 1 proyecto internacional
                    cumpleInternacionales = proyectosInternacionales >= 1;
                    break;
                case 7: // Principal 2 → Principal 3  
                    // Requiere al menos 1 proyecto internacional con 24+ meses
                    cumpleInternacionales = proyectosInternacionales >= 1 && mesesInternacionales >= 24;
                    break;
                case 8: // Principal 3
                    // Requiere al menos 2 proyectos internacionales con 36+ meses total
                    cumpleInternacionales = proyectosInternacionales >= 2 && mesesInternacionales >= 36;
                    break;
                default:
                    // Para otros rangos no se requieren proyectos internacionales
                    cumpleInternacionales = true;
                    break;
            }

            return (totalMeses, proyectosInternacionales, cumpleInternacionales);
        }

        /// <summary>
        /// Obtiene la cantidad de proyectos internacionales requeridos según el rango
        /// </summary>
        /// <param name="rangoId">ID del rango</param>
        /// <returns>Cantidad de proyectos internacionales requeridos</returns>
        private int GetProyectosInternacionalesRequeridos(int rangoId)
        {
            return rangoId switch
            {
                6 => 1, // Principal 1
                7 => 1, // Principal 2 (con al menos 24 meses)
                8 => 2, // Principal 3 (con al menos 36 meses total)
                _ => 0  // Otros rangos no requieren proyectos internacionales
            };
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

        /// <summary>
        /// Verifica si existe una cédula en la base de datos de docentes
        /// </summary>
        /// <param name="cedula">Cédula a verificar</param>
        /// <returns>True si existe, False si no</returns>
        [HttpGet("cedula-existe/{cedula}")]
        [AllowAnonymous]
        public async Task<IActionResult> CedulaExiste(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula) || cedula.Length != 10)
            {
                return BadRequest(new { success = false, message = "La cédula debe tener exactamente 10 dígitos" });
            }
            var existe = await _context.Docentes.AnyAsync(d => d.Cedula == cedula);
            return Ok(existe);
        }

        /// <summary>
        /// (Opcional) Crea un usuario temporal si la cédula no existe (descomentar para habilitar)
        /// </summary>
        /// <param name="model">Datos mínimos para usuario temporal</param>
        /// <returns>Resultado del registro temporal</returns>
        /*
        [HttpPost("registrar-usuario-temporal")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuarioTemporal([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
                return BadRequest(new { success = false, message = "Datos de entrada inválidos", errors });
            }
            // Aquí puedes crear un usuario temporal en la tabla que corresponda
            // Ejemplo:
            var usuarioTemporal = new SIGAD.Domain.Entities.Docente
            {
                Cedula = model.Cedula,
                Nombre1 = "TEMPORAL",
                Apellido1 = "TEMPORAL",
                Correo = model.Correo
            };
            _context.Docentes.Add(usuarioTemporal);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Usuario temporal creado" });
        }
        */

        /// <summary>
        /// Valida que las evaluaciones docentes cumplan con los requisitos para una solicitud de ascenso:
        /// - Exactamente 4 evaluaciones
        /// - Todas en exactamente 2 años consecutivos (no puede ser un solo año)
        /// - No permitir años futuros (solo años ≤ año actual)
        /// - Promedio mínimo de 75%
        /// </summary>
        /// <param name="evaluaciones">Lista de evaluaciones asociadas a la solicitud</param>
        /// <returns>Tupla con el resultado de validación y lista de errores</returns>
        private (bool esValida, List<string> errores) ValidarEvaluacionesParaSolicitud(List<SIGAD.Domain.Entities.EvaluacionDocente?> evaluaciones)
        {
            var errores = new List<string>();
            int anoActual = DateTime.Now.Year; // 2025

            // Filtrar evaluaciones no nulas
            var evaluacionesValidas = evaluaciones.Where(e => e != null).Cast<SIGAD.Domain.Entities.EvaluacionDocente>().ToList();

            // 1. Verificar que tenga exactamente 4 evaluaciones
            if (evaluacionesValidas.Count != 4)
            {
                errores.Add($"Evaluaciones: Se requieren exactamente 4 evaluaciones, pero tiene {evaluacionesValidas.Count}");
                return (false, errores);
            }

            // 2. Obtener los años únicos de las evaluaciones
            var anosEvaluaciones = evaluacionesValidas.Select(e => e.FechaEvaluacion.Year).Distinct().OrderBy(y => y).ToList();

            // 3. Verificar que no haya años futuros
            var anosFuturos = anosEvaluaciones.Where(ano => ano > anoActual).ToList();
            if (anosFuturos.Any())
            {
                errores.Add($"Evaluaciones: No se permiten evaluaciones de años futuros. Años futuros encontrados: {string.Join(", ", anosFuturos)}. Año actual: {anoActual}");
                return (false, errores);
            }

            // 4. Validar que sean EXACTAMENTE 2 años consecutivos (no puede ser un solo año)
            if (anosEvaluaciones.Count == 1)
            {
                errores.Add($"Evaluaciones: Todas las evaluaciones son del año {anosEvaluaciones[0]}. Se requieren evaluaciones de exactamente 2 años consecutivos, no un solo año.");
                return (false, errores);
            }
            else if (anosEvaluaciones.Count == 2)
            {
                int anoMenor = anosEvaluaciones[0];
                int anoMayor = anosEvaluaciones[1];
                
                if (anoMayor - anoMenor != 1)
                {
                    errores.Add($"Evaluaciones: Los años {anoMenor} y {anoMayor} no son consecutivos. Se requieren evaluaciones de exactamente 2 años consecutivos.");
                    return (false, errores);
                }
                
                // Si llegamos aquí, tenemos exactamente 2 años consecutivos ✅
            }
            else
            {
                errores.Add($"Evaluaciones: Las evaluaciones abarcan {anosEvaluaciones.Count} años diferentes ({string.Join(", ", anosEvaluaciones)}). Se requieren evaluaciones de exactamente 2 años consecutivos.");
                return (false, errores);
            }

            // 5. Verificar que el promedio sea al menos 75%
            decimal promedioEvaluaciones = evaluacionesValidas.Average(e => e.PuntajePorcentual);
            
            if (promedioEvaluaciones < 75.0m)
            {
                errores.Add($"Evaluaciones: El promedio es {promedioEvaluaciones:F1}%, pero se requiere un mínimo de 75%");
            }

            // Información adicional para debugging (solo si todo está bien)
            if (errores.Count == 0)
            {
                string rangoDetectado = $"{anosEvaluaciones[0]}-{anosEvaluaciones[1]}";
                _logger.LogInformation($"[VALIDACIÓN EVALUACIONES] ✅ Válidas: 4 evaluaciones, periodo: {rangoDetectado}, promedio: {promedioEvaluaciones:F1}%");
            }
            else
            {
                _logger.LogWarning($"[VALIDACIÓN EVALUACIONES] ❌ Errores: {string.Join(" | ", errores)}");
            }

            return (errores.Count == 0, errores);
        }
    }
    public class TestHtmlEmailDto
    {
        [Required]
        [EmailAddress]
        public string ToEmail { get; set; } = "tu.correo@ejemplo.com";

        [Required]
        public string DocenteNombre { get; set; } = "Juan Pérez";

        [Required]
        public string RangoNombre { get; set; } = "Titular Agregado 1";

        public string Observaciones { get; set; } = "Prueba de envío con plantilla.";

        // Para probar ambos casos (aprobado o rechazado)
        public bool EsAprobacion { get; set; } = true;
    }
}