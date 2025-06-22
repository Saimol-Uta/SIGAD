public class VerTesisDirigidaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Nivel { get; set; } = ""; // pregrado, maestría, etc.
}
