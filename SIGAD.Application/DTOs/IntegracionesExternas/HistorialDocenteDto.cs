using System.Collections.Generic;

namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class HistorialDocenteDto
    {
        public List<ArticuloExternoDto> Articulos { get; set; } = new();
        public List<CursoDto> Cursos { get; set; } = new();
        public List<EvaluacionDto> Evaluaciones { get; set; } = new();
        public List<InvestigacionDto> Investigaciones { get; set; } = new();
        public List<ExperienciaDto> Experiencias { get; set; } = new();
    }
}
