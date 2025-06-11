using SIGAD.Domain.Enums;

namespace SIGAD.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public Rol Rol { get; set; }
        public DocenteInfoDto DocenteInfo { get; set; } = new();
        public DateTime ExpiracionToken { get; set; }
    }

    public class DocenteInfoDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre1 { get; set; } = string.Empty;
        public string? Nombre2 { get; set; }
        public string Apellido1 { get; set; } = string.Empty;
        public string Apellido2 { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre1} {Nombre2} {Apellido1} {Apellido2}".Replace("  ", " ").Trim();
    }
}