using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Domain.Enums;

namespace SIGAD.Domain.Entities
{
    public class AccionesDePersonal
    {
        public int Id { get; set; }
        public string DocenteCedula { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public TipoCargoAdministrativo TipoCargo { get; set; } = TipoCargoAdministrativo.DirectorCarrera;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string DocumentoRuta { get; set; } = string.Empty; // Para base interna
        public string CertificadoRuta { get; set; } = string.Empty; // Para bases externas
        public string ContenidoHash { get; set; } = string.Empty;

        // Métodos de negocio
        public int GetMesesDuracion()
        {
            var fechaFin = FechaFin ?? DateTime.Now;
            return (int)((fechaFin - FechaInicio).TotalDays / 30.44); // Promedio días por mes
        }

        public bool EsCargoDeAutoridad()
        {
            return TipoCargo == TipoCargoAdministrativo.Rector ||
                   TipoCargo == TipoCargoAdministrativo.Vicerrector ||
                   TipoCargo == TipoCargoAdministrativo.AutoridadSNES;
        }

        public int GetHorasEquivalencia()
        {
            return TipoCargo switch
            {
                TipoCargoAdministrativo.MiembroComisionExterno => 16,
                TipoCargoAdministrativo.ParEvaluadorExterno => 16,
                TipoCargoAdministrativo.FacilitadorCES => 24, // Puede ser 24-32
                TipoCargoAdministrativo.EvaluadorCACES => 32,
                _ => 0
            };
        }// Propiedad de navegación hacia el Docente
        public virtual Docente Docente { get; set; } = null!;

        // Relación con solicitudes
        public virtual ICollection<AccionesDePersonalPorSolicitud> AccionesDePersonalPorSolicitud { get; set; } = new List<AccionesDePersonalPorSolicitud>();
    }
}
