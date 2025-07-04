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
            if (docente == null || docente.Cuenta == null) return;

            string emailBody = await File.ReadAllTextAsync(_templatePath);
            string urlHistorial = "http://localhost:5250/historial-solicitudes";

            // 1. Reemplazar contenido dinámico
            emailBody = emailBody.Replace("[Nombre del Docente]", $"{docente.Nombre1} {docente.Apellido1}");
            emailBody = emailBody.Replace("[Especificar grado, si aplica]", solicitud.RangoSolicitado?.Nombre ?? "N/A");
            emailBody = emailBody.Replace("[Fecha exacta]", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            emailBody = emailBody.Replace("[UrlHistorial]", urlHistorial); // <--- CORRECCIÓN CLAVE

            // 2. Eliminar el párrafo de "NO APROBADO" completo para un resultado limpio
            emailBody = emailBody.Replace("<p style=\"line-height: 1.6; font-size: 1rem; color: rgb(108, 19, 19); font-weight: 900;\">Resultado: NO APROBADO</p>", "");

            // 3. Crear registro y enviar correo
            var mensajeDb = $"Su solicitud de ascenso ha sido APROBADA. Grado: {solicitud.RangoSolicitado?.Nombre}. Observaciones: {observaciones}";
            await CrearRegistroNotificacionAsync(solicitud.DocenteCedula, mensajeDb, urlHistorial);
            await _emailService.SendEmailAsync(docente.Cuenta.Correo, "Resultado del Proceso de Promoción Docente", emailBody);
        }

        public async Task EnviarNotificacionRechazoAsync(SolicitudAscenso solicitud, string observaciones)
        {
            var docente = solicitud.Docente;
            if (docente == null || docente.Cuenta == null) return;

            string emailBody = await File.ReadAllTextAsync(_templatePath);
            string urlHistorial = "http://localhost:5250/historial-solicitudes";

            emailBody = emailBody.Replace("[Nombre del Docente]", $"{docente.Nombre1} {docente.Apellido1}");
            emailBody = emailBody.Replace("[UrlHistorial]", urlHistorial);

            // Se actualizan las cadenas para que incluyan el estilo "color: #000000;"

            // Elimina el párrafo de "APROBADO"
            emailBody = emailBody.Replace("<p style=\"line-height: 1.6; font-size: 1rem; color: #cd982e; font-weight: 900;\">Resultado: APROBADO</p>", "");

            // Elimina el párrafo de "Grado escalafonario"
            emailBody = emailBody.Replace("<p style=\"line-height: 1.6; font-size: 1rem; color: #000000;\">Grado escalafonario al que asciende: [Especificar grado, si aplica]</p>", "");

            // Elimina el párrafo de "Fecha efectiva"
            emailBody = emailBody.Replace("<p style=\"line-height: 1.6; font-size: 1rem; color: #000000;\">Fecha efectiva del ascenso: [Fecha exacta]</p>", "");

            // Elimina el párrafo de "Felicitaciones"
            emailBody = emailBody.Replace("<p style=\"line-height: 1.6; font-size: 1rem; color: #000000;\">Felicitaciones por su dedicación y esfuerzo continuo en la mejora de la calidad académica de nuestra institución.</p>", "");

            var mensajeDb = $"Su solicitud de ascenso ha sido RECHAZADA. Motivo: {observaciones}";
            await CrearRegistroNotificacionAsync(solicitud.DocenteCedula, mensajeDb, urlHistorial);
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
    }
}
