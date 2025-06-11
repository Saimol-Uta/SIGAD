using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace SIGAD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IConfiguration _configuration;

        public AuthService(ICuentaRepository cuentaRepository, IConfiguration configuration)
        {
            _cuentaRepository = cuentaRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // Buscar cuenta con información del docente
                var cuenta = await _cuentaRepository.GetByCorreoWithDocenteAsync(loginRequest.Correo);
                if (cuenta == null)
                {
                    return null; // Usuario no encontrado
                }

                // Verificar contraseña
                if (!VerifyPassword(loginRequest.Clave, cuenta.ClaveHash))
                {
                    return null; // Contraseña incorrecta
                }

                // Generar token JWT
                var token = GenerateJwtToken(cuenta.Correo, cuenta.Rol.ToString(), cuenta.DocenteCedula);
                var expiracion = DateTime.UtcNow.AddHours(24); // Token válido por 24 horas

                return new LoginResponseDto
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
            }
            catch (Exception)
            {
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