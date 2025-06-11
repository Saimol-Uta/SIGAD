// SIGAD.Domain/Entities/Rango.cs
namespace SIGAD.Domain.Entities
{
    public class Rango
    {
        // Constructor que toma Id y Nombre
        public Rango(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        // Constructor por defecto necesario para serialización/deserialización
        public Rango() { }
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

        public void ActualizarRequisitos(string nombre, int articulos, int anios, int horas, int meses, decimal puntaje)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rango es requerido.", nameof(nombre));

            Nombre = nombre;
            ArticulosRequeridos = articulos;
            AniosExperienciaRequeridos = anios;
            HorasCursoRequeridas = horas;
            MesesInvestigacionRequeridos = meses;
            PuntajePromedioEvaluacionesRequerido = puntaje;
        }
    }
}