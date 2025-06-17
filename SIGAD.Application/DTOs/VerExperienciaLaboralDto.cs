using System.ComponentModel.DataAnnotations;

namespace SIGAD.Application.DTOs
{
    public class VerExperienciaLaboralDto
    {
        public int Id { get; set; }

        [Display(Name = "Organización")]
        public string OrganizacionNombre { get; set; } = string.Empty;

        [Display(Name = "Tipo de Organización")]
        public string OrganizacionTipo { get; set; } = string.Empty;

        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Certificado")]
        public string CertificadoRuta { get; set; } = string.Empty;

        public bool TieneCertificado => !string.IsNullOrEmpty(CertificadoRuta);

        public string PeriodoTrabajo => FechaFin.HasValue
            ? $"{FechaInicio:MM/yyyy} - {FechaFin.Value:MM/yyyy}"
            : $"{FechaInicio:MM/yyyy} - Actual";
    }
}
