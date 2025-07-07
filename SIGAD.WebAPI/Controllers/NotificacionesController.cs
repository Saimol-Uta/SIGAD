using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGAD.Application.Interfaces;
using System.Security.Claims;

namespace SIGAD.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ¡Importante! Solo usuarios autenticados pueden ver sus notificaciones
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionesController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        // DTO para la respuesta
        public class UnreadCountResponse
        {
            public int UnreadCount { get; set; }
        }

        // GET: api/notificaciones/unread-count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            // Obtenemos la cédula del usuario desde el token JWT
            var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(cedula))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            var count = await _notificacionService.GetUnreadCountByCedulaAsync(cedula);

            return Ok(new UnreadCountResponse { UnreadCount = count });
        }

        // GET: api/notificaciones
        [HttpGet]
        public async Task<IActionResult> GetNotificaciones()
        {
            var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(cedula)) return Unauthorized();

            var notificaciones = await _notificacionService.GetNotificacionesByCedulaAsync(cedula);
            return Ok(notificaciones);
        }

        // POST: api/notificaciones/{id}/mark-as-read
        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var cedula = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(cedula)) return Unauthorized();

            var success = await _notificacionService.MarkAsReadAsync(id, cedula);
            if (!success)
            {
                return NotFound(); // O BadRequest, si el usuario intentó marcar una notificación ajena
            }

            return NoContent(); // 204 No Content es una respuesta estándar para una acción exitosa sin retorno de contenido.
        }
    }
}