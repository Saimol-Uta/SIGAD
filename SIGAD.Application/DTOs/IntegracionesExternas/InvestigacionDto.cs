namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class InvestigacionDto
    {
        public string Titulo { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string RolEnInvestigacion { get; set; } = default!;
        public int MesesDeInvestigacion { get; set; }
        public string InformeRuta { get; set; } = default!;
        public string ContenidoHash { get; set; } = default!;
        public string DocenteCedula { get; set; } = default!;

        public string TipoProyecto { get; set; } = string.Empty;
        public int MesesDeParticipacion { get; set; }
        public string UnidadVerificadora { get; set; } = string.Empty;

        public bool EsInternacional { get; set; }


    }
}
