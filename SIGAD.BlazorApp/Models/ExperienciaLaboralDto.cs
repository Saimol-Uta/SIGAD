namespace SIGAD.BlazorApp.Models
{
    public class VerExperienciaLaboralDto
    {
        public int Id { get; set; }
        public string OrganizacionNombre { get; set; } = string.Empty;
        public string OrganizacionTipo { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string CertificadoRuta { get; set; } = string.Empty;
        public bool TieneCertificado => !string.IsNullOrEmpty(CertificadoRuta);

        public string PeriodoTrabajo => FechaFin.HasValue
            ? $"{FechaInicio:MM/yyyy} - {FechaFin.Value:MM/yyyy}"
            : $"{FechaInicio:MM/yyyy} - Actual";
    }
}
