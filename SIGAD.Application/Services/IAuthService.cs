using SIGAD.Application.DTOs;

namespace SIGAD.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<bool> RegisterAsync(RegisterRequestDto registerRequest);


        string GenerateJwtToken(string correo, string rol, string cedula, string nombre1, string? nombre2, string apellido1, string apellido2, int? rangoId, string? rangoNombre);
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password); 
        
    }
}