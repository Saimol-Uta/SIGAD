using SIGAD.Application.DTOs;

namespace SIGAD.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        string GenerateJwtToken(string correo, string rol, string cedula);
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password);
    }
}