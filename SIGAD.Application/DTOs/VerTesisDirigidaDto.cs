public class VerTesisDirigidaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Nivel { get; set; } = ""; // pregrado, maestría, etc.
    public string Estado { get; set; } = ""; // En proceso, finalizada, abandonada
    public string CertificacionPath { get; set; } = "";
    public string Institucion { get; set; } = ""; // Campo original de la entidad
}
