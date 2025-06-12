namespace SIGAD.BlazorApp.Models
{
    public class AscensoEstadoDto
    {
        public ActualizarRangoDto Requisitos { get; set; }

        public string EstadoArticulos { get; set; } = "Sin Revisar";
        public string EstadoExperiencia { get; set; } = "Sin Revisar";
        public string EstadoCursos { get; set; } = "Sin Revisar";
        public string EstadoInvestigacion { get; set; } = "Sin Revisar";
        public string EstadoEvaluaciones { get; set; } = "Sin Revisar";
    }

}
