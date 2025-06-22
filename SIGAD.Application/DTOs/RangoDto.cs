namespace SIGAD.Application.DTOs
{
    public class RangoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int ArticulosRequeridos { get; set; }
        public int AniosExperienciaRequeridos { get; set; }
        public int HorasCursoRequeridas { get; set; }
        public int MesesInvestigacionRequeridos { get; set; }
        public decimal PuntajePromedioEvaluacionesRequerido { get; set; }
        public int TesisDirigidasRequeridas { get; set; } // NUEVO
    }
}
