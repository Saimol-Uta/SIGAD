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
        public string Nombre { get; set; } = string.Empty;
        public int ArticulosRequeridos { get; set; }
        public int AniosExperienciaRequeridos { get; set; }
        public int HorasCursoRequeridas { get; set; }
        public int MesesInvestigacionRequeridos { get; set; }
        public int TesisDirigidasRequeridas { get; set; }
        public decimal PuntajePromedioEvaluacionesRequerido { get; set; } // Mapea a: DECIMAL(5,2)

        // CAMPOS ADICIONALES ESPECÍFICOS DEL REGLAMENTO UTA
        public int HorasCapacitacionPedagogicaRequeridas { get; set; } = 0; // 25% de HorasCursoRequeridas
        public int HorasCapacitacionImpartidaRequeridas { get; set; } = 0; // Para rangos principales (Art. tablas)
        public int PublicacionesIdiomaExtranjeroRequeridas { get; set; } = 0; // Para rangos principales
        public int ProyectosInternacionalesRequeridos { get; set; } = 0; // Para rangos principales
        public bool RequiereArticuloEnGradoActual { get; set; } = false; // "durante el ejercicio de sus actividades en el grado"
        public bool PermiteCoordinacionProyectos { get; set; } = false; // Para multiplcar tiempo por 1.5x o 2x

        // Propiedades de navegación
        // Un Rango puede ser el RangoActual en muchas solicitudes
        public virtual ICollection<SolicitudAscenso> SolicitudesComoRangoActual { get; set; } = new List<SolicitudAscenso>();
        // Un Rango puede ser el RangoSolicitado en muchas solicitudes
        public virtual ICollection<SolicitudAscenso> SolicitudesComoRangoSolicitado { get; set; } = new List<SolicitudAscenso>();

        public void ActualizarRequisitos(string nombre, int articulos, int anios, int horas, int meses, decimal puntaje, int tesis)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rango es requerido.", nameof(nombre));

            Nombre = nombre;
            ArticulosRequeridos = articulos;
            AniosExperienciaRequeridos = anios;
            HorasCursoRequeridas = horas;
            MesesInvestigacionRequeridos = meses;
            PuntajePromedioEvaluacionesRequerido = puntaje;
            TesisDirigidasRequeridas = tesis; // NUEVO
        }
    }
}