using System.Collections.Generic;

namespace SIGAD.Application.DTOs.Docente
{
    public class HistorialDocenteDto
    {
        public List<ArticuloDto> Articulos { get; set; } = new();
        public List<CursoDto> Cursos { get; set; } = new();
        public List<EvaluacionDocenteDto> Evaluaciones { get; set; } = new();
        public List<InvestigacionDto> Investigaciones { get; set; } = new();
        public List<ExperienciaLaboralDto> Experiencias { get; set; } = new();
    }
}
