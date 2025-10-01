using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SIGAD.Application.Contracts.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SIGAD.Application.Services
{
    /// <summary>
    /// Servicio para generación y gestión de tokens JWT.
    /// Principio SRP: Responsable únicamente de la creación de tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(
            string correo,
            string rol,
            string cedula,
            string nombre1,
            string? nombre2,
            string apellido1,
            string apellido2,
            int? rangoId,
            string? rangoNombre)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
            var issuer = jwtSettings["Issuer"] ?? "SIGAD.API";
            var audience = jwtSettings["Audience"] ?? "SIGAD.Client";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Construir nombre completo
            var nombreCompleto = $"{nombre1} {nombre2} {apellido1} {apellido2}".Replace("  ", " ").Trim();

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, correo),
                new Claim(ClaimTypes.Name, correo), // Para compatibilidad
                new Claim(ClaimTypes.NameIdentifier, cedula), // Cédula como identificador principal
                new Claim(ClaimTypes.Role, rol),
                new Claim("cedula", cedula), // Mantener para compatibilidad
                new Claim("nombre1", nombre1),
                new Claim("nombre2", nombre2 ?? ""),
                new Claim("apellido1", apellido1),
                new Claim("apellido2", apellido2),
                new Claim("NombreCompleto", nombreCompleto),
                new Claim("rangoId", rangoId?.ToString() ?? ""),
                new Claim("rangoNombre", rangoNombre ?? "Sin rango asignado"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Name, nombreCompleto)
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

        public async Task<Dictionary<string, string>?> ValidateTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");
                var issuer = jwtSettings["Issuer"] ?? "SIGAD.API";
                var audience = jwtSettings["Audience"] ?? "SIGAD.Client";

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken)
                    return null;

                // Extraer claims en un diccionario
                var claims = new Dictionary<string, string>();
                foreach (var claim in principal.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }

                return await Task.FromResult(claims);
            }
            catch
            {
                return null;
            }
        }

        public string? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Buscar claim de cédula (identificador principal)
                var cedulaClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "cedula" || c.Type == ClaimTypes.NameIdentifier);
                return cedulaClaim?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
