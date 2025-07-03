using System.ComponentModel.DataAnnotations;

namespace SIGAD.Domain.Enums
{
    public enum EstadoSolicitud
    {
        /// <summary>
        /// El docente está creando la solicitud, pero aún no la ha enviado formalmente.
        /// </summary>
        [Display(Name = "Borrador")]
        Borrador = 1,

        /// <summary>
        /// La solicitud ha sido enviada y está pendiente de revisión por la Comisión.
        /// </summary>
        [Display(Name = "Enviada")]
        Enviada = 2,

        /// <summary>
        /// La Comisión está analizando activamente la solicitud.
        /// </summary>
        [Display(Name = "En Revisión")]
        EnRevision = 3,

        /// <summary>
        /// La solicitud ha sido aprobada por la Comisión y el Consejo, y el proceso ha finalizado con éxito.
        /// </summary>
        [Display(Name = "Aprobada")]
        Aprobada = 4,

        /// <summary>
        /// La solicitud fue rechazada por la Comisión. El docente ha sido notificado y puede apelar.
        /// </summary>
        [Display(Name = "Rechazada")]
        Rechazada = 5,

        /// <summary>
        /// El docente ha presentado una apelación y está pendiente de resolución por la Comisión.
        /// </summary>
        [Display(Name = "En Apelación")]
        EnApelacion = 6,

        /// <summary>
        /// La apelación del docente fue rechazada. El proceso ha finalizado sin éxito.
        /// </summary>
        [Display(Name = "Rechazada Definitivamente")]
        RechazadaDefinitiva = 7,

        /// <summary>
        /// La apelación del docente fue aceptada, resultando en la aprobación de la solicitud.
        /// </summary>
        [Display(Name = "Aprobada por Apelación")]
        AprobadaPorApelacion = 8,

        /// <summary>
        /// **NUEVO ESTADO:** La solicitud fue notificada (aprobada o rechazada) pero el docente no respondió
        /// en el plazo de 3 días. Según el Art. 6, la solicitud "queda sin efecto".
        /// </summary>
        [Display(Name = "Cerrada por Falta de Respuesta")]
        CerradaSinRespuesta = 9
    }
}