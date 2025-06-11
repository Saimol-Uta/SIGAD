using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace SIGAD.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
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
                return Ok(new
                {
                    success = true,
                    message = "Login exitoso",
                    data = result
                });
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
    }
}