using SIGAD.Domain.Enums;
namespace SIGAD.Domain.Entities
{
    public class Apelacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SolicitudId { get; set; }
        public SolicitudAscenso Solicitud { get; set; } = null!;

        public string Motivo { get; set; } = string.Empty;

        public string ArchivoRuta { get; set; } = string.Empty;
        public string ArchivoNombre { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public EstadoApelacion Estado { get; set; } = EstadoApelacion.Pendiente;
    }
}
