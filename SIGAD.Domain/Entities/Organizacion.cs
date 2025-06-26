using System.Collections.Generic;

namespace SIGAD.Domain.Entities
{
    public class Organizacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoOrganizacion { get; set; } = "EXTERNA";

        public virtual ICollection<ExperienciaLaboral> ExperienciasLaborales { get; set; } = new List<ExperienciaLaboral>();
        public virtual ICollection<Curso> Cursos { get; set; } = new List<Curso>();
    }
}
