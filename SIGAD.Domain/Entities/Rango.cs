// SIGAD.Domain/Entities/Rango.cs
namespace SIGAD.Domain.Entities
{
    public class Rango
    {
        public Guid Id { get; private set; } // Identificador único del rango
        public string Nombre { get; private set; } // Nombre del rango, ej: "Profesor Titular A"
        public string Descripcion { get; private set; } // Descripción opcional del rango

        // Constructor privado para EF Core y para la creación controlada desde un método factoría o servicio
        private Rango() { }

        // Constructor público para crear nuevas instancias de Rango
        public Rango(Guid id, string nombre, string descripcion)
        {
            // Aquí podrías añadir validaciones si fueran necesarias antes de asignar
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del rango no puede estar vacío.", nameof(nombre));
            }

            Id = id == Guid.Empty ? Guid.NewGuid() : id; // Asigna un nuevo Guid si no se provee uno
            Nombre = nombre;
            Descripcion = descripcion;
        }

        // Métodos para modificar el estado de Rango (si fueran necesarios más adelante)
        // Ejemplo:
        // public void ActualizarDescripcion(string nuevaDescripcion)
        // {
        //     Descripcion = nuevaDescripcion;
        // }
    }
}