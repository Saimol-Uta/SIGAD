using System.Text.Json;

namespace SIGAD.WebAPI.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidationMiddleware> _logger;

        public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                await WriteErrorResponse(context, 400, "Error de validación", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
                await WriteErrorResponse(context, 404, "Recurso no encontrado", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation: {Message}", ex.Message);
                await WriteErrorResponse(context, 422, "Operación inválida", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access: {Message}", ex.Message);
                await WriteErrorResponse(context, 401, "Acceso no autorizado", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await WriteErrorResponse(context, 500, "Error interno", "Ha ocurrido un error inesperado");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string title, string message)
        {
            // Solo responder si no se ha enviado ya una respuesta
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                title,
                message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path.Value
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    // Extension method para registrar el middleware
    public static class ValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidationMiddleware>();
        }
    }
}
