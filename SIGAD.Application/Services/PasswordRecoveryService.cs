using SIGAD.Application.Contracts.Services;
using SIGAD.Domain.Interfaces;

namespace SIGAD.Application.Services
{
    /// <summary>
    /// Servicio para recuperación y restablecimiento de contraseñas.
    /// Principio SRP: Responsable únicamente del proceso de recuperación de contraseñas.
    /// </summary>
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly ICuentaRepository _cuentaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IUserRegistrationService _userRegistrationService;

        public PasswordRecoveryService(
            ICuentaRepository cuentaRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IUserRegistrationService userRegistrationService)
        {
            _cuentaRepository = cuentaRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userRegistrationService = userRegistrationService;
        }

        public async Task<bool> SolicitarRecuperacionAsync(string email)
        {
            var cuenta = await _cuentaRepository.GetByEmailAsync(email);

            if (cuenta == null)
            {
                // Por seguridad, retornar true incluso si el email no existe
                return true;
            }

            // Generar código de 6 dígitos
            var codigo = new Random().Next(100000, 999999).ToString();

            // Guardar código y fecha de expiración
            cuenta.CodigoRecuperacion = codigo;
            cuenta.CodigoExpiracion = DateTime.UtcNow.AddMinutes(15);

            await _unitOfWork.SaveChangesAsync();

            // Enviar email con el código
            var asunto = "Código de Recuperación de Contraseña - SIGAD";
            var cuerpo = $"Hola, has solicitado restablecer tu contraseña. Tu código de recuperación es: {codigo}. Este código expirará en 15 minutos.";
            await _emailService.SendEmailAsync(cuenta.Correo, asunto, cuerpo);

            return true;
        }

        public async Task<bool> VerificarCodigoAsync(string email, string codigo)
        {
            return await _cuentaRepository.VerificarCodigoRecuperacionAsync(email, codigo);
        }

        public async Task<bool> RestablecerContrasenaAsync(string email, string codigo, string nuevaContrasena, string confirmarContrasena)
        {
            // Validar que las contraseñas coincidan
            if (nuevaContrasena != confirmarContrasena)
            {
                return false;
            }

            // Buscar cuenta y validar código
            var cuenta = await _cuentaRepository.GetByEmailAsync(email);

            if (cuenta == null || cuenta.CodigoRecuperacion != codigo || cuenta.CodigoExpiracion < DateTime.UtcNow)
            {
                return false;
            }

            // Hash de la nueva contraseña (delegado a UserRegistrationService)
            cuenta.ClaveHash = _userRegistrationService.HashPassword(nuevaContrasena);

            // Limpiar código de recuperación
            cuenta.CodigoRecuperacion = null;
            cuenta.CodigoExpiracion = null;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
