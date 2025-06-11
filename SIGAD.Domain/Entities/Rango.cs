// SIGAD.Domain/Entities/Rango.cs
namespace SIGAD.Domain.Entities
{
    public class Rango
    {
        public int Id { get; set; } // Mapea a: Id INT PRIMARY KEY
        public string Nombre { get; set; }
        public int ArticulosRequeridos { get; set; }
        public int AniosExperienciaRequeridos { get; set; }
        public int HorasCursoRequeridas { get; set; }
        public int MesesInvestigacionRequeridos { get; set; }
        public decimal PuntajePromedioEvaluacionesRequerido { get; set; } // Mapea a: DECIMAL(5,2)

        // Propiedades de navegación
        // Un Rango puede ser el RangoActual en muchas solicitudes
        public virtual ICollection<SolicitudAscenso> SolicitudesComoRangoActual { get; set; } = new List<SolicitudAscenso>();
        // Un Rango puede ser el RangoSolicitado en muchas solicitudes
        public virtual ICollection<SolicitudAscenso> SolicitudesComoRangoSolicitado { get; set; } = new List<SolicitudAscenso>();
    }
}