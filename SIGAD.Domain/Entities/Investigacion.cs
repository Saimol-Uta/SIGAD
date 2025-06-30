using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    public class Investigacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public string RolEnInvestigacion { get; set; } = string.Empty;
        public int MesesDeInvestigacion { get; set; }
        public string InformeRuta { get; set; } = string.Empty;
        public string ContenidoHash { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;

        public TipoInvestigacion TipoProyecto { get; set; } = TipoInvestigacion.Aplicada;
        public int MesesDeParticipacion { get; set; }
        public string UnidadVerificadora { get; set; } = string.Empty;

   
        public bool EsInternacional { get; set; } = false;

        public virtual Docente Docente { get; set; } = default!;

     
        public decimal CalcularMesesEquivalentes()
        {
            return RolEnInvestigacion.ToLower() switch
            {
                var rol when rol.Contains("coordinador principal") || rol.Contains("director") => MesesDeInvestigacion * 2.0m,
                var rol when rol.Contains("coordinador subrogante") || rol.Contains("subdirector") => MesesDeInvestigacion * 1.5m,
                _ => MesesDeInvestigacion
            };
        }

        /// <summary>
        /// Verifica si cumple requisitos para rangos principales (debe ser internacional)
        /// </summary>
        public bool CumpleRequisitoRangoPrincipal()
        {
            return EsInternacional && !string.IsNullOrEmpty(UnidadVerificadora);
        }
    }
}
