using SIGAD.Domain.Entities;
using SIGAD.Domain.Interfaces;
using SIGAD.Application.Interfaces;
using System;
using System.IO; // Necesario para leer archivos
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly string _templatePath;

        public NotificacionService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            // Construye la ruta al directorio de plantillas
            _templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates", "ResultadoSolicitud.html");
        }

        public async Task EnviarNotificacionAprobacionAsync(SolicitudAscenso solicitud, string observaciones)
        {
            var docente = solicitud.Docente;
            if (docente == null || docente.Cuenta == null) return; // No se puede notificar

            // 1. Cargar y personalizar la plantilla HTML
            string emailBody = await File.ReadAllTextAsync(_templatePath);

            emailBody = emailBody.Replace("[Nombre del Docente]", $"{docente.Nombre1} {docente.Apellido1}");
            emailBody = emailBody.Replace("Resultado: NO APROBADO", ""); // Ocultar la parte de no aprobado
            emailBody = emailBody.Replace("[Especificar grado, si aplica]", solicitud.RangoSolicitado?.Nombre ?? "N/A");
            emailBody = emailBody.Replace("[Fecha exacta]", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            // Ocultar los botones de apelación que no aplican
            emailBody = OcultarElementoHtml(emailBody, "<!-- Mostrar solo si resultado es NO APROBADO -->");

            // 2. Crear el registro de la notificación en la base de datos
            var mensajeDb = $"Su solicitud de ascenso ha sido APROBADA. Grado: {solicitud.RangoSolicitado?.Nombre}. Observaciones: {observaciones}";
            var url = $"/docente/solicitudes/{solicitud.Id}";
            await CrearRegistroNotificacionAsync(solicitud.DocenteCedula, mensajeDb, url);

            // 3. Enviar el correo
            await _emailService.SendEmailAsync(docente.Cuenta.Correo, "Resultado del Proceso de Promoción Docente", emailBody);
        }

        public async Task EnviarNotificacionRechazoAsync(SolicitudAscenso solicitud, string observaciones)
        {
            var docente = solicitud.Docente;
            if (docente == null || docente.Cuenta == null) return; // No se puede notificar

            // 1. Cargar y personalizar la plantilla HTML
            string emailBody = await File.ReadAllTextAsync(_templatePath);

            emailBody = emailBody.Replace("[Nombre del Docente]", $"{docente.Nombre1} {docente.Apellido1}");
            emailBody = emailBody.Replace("Resultado: APROBADO", ""); // Ocultar la parte de aprobado
            emailBody = emailBody.Replace("Grado escalafonario al que asciende: [Especificar grado, si aplica]", "");
            emailBody = emailBody.Replace("Fecha efectiva del ascenso: [Fecha exacta]", "");

            // 2. Crear el registro de la notificación en la base de datos
            var mensajeDb = $"Su solicitud de ascenso ha sido RECHAZADA. Motivo: {observaciones}";
            var url = $"/docente/solicitudes/{solicitud.Id}";
            await CrearRegistroNotificacionAsync(solicitud.DocenteCedula, mensajeDb, url);

            // 3. Enviar el correo
            await _emailService.SendEmailAsync(docente.Cuenta.Correo, "Resultado del Proceso de Promoción Docente", emailBody);
        }

        private async Task CrearRegistroNotificacionAsync(string cedula, string mensaje, string? url)
        {
            var notificacion = new Notificacion
            {
                DocenteCedula = cedula,
                Mensaje = mensaje,
                UrlRedireccion = url,
                EsLeida = false,
                FechaCreacion = DateTime.UtcNow
            };
            await _unitOfWork.Notificaciones.AddAsync(notificacion);
            await _unitOfWork.SaveChangesAsync();
        }

        // Helper para "ocultar" un bloque de HTML comentándolo
        private string OcultarElementoHtml(string html, string startComment)
        {
            int startIndex = html.IndexOf(startComment);
            if (startIndex == -1) return html;

            int endIndex = html.IndexOf("</div>", startIndex) + "</div>".Length;
            return html.Remove(startIndex, endIndex - startIndex);
        }
    }
}
