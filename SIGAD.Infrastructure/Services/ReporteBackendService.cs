using Microsoft.EntityFrameworkCore;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces; // Interfaz en Application
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Infrastructure.Services // Movido de Application.Services a Infrastructure.Services
{
    /// <summary>
    /// Servicio de reportes que utiliza EF Core directamente.
    /// Ubicado en Infrastructure ya que depende de implementaciones específicas de EF Core (ToListAsync, etc).
    /// </summary>
    public class ReporteBackendService
    {
        private readonly IApplicationDbContext _context;

        public ReporteBackendService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteDataDto>> ObtenerSolicitudesPorEstado()
        {
            // Obtener datos agrupados por estado
            var resultado = await _context.SolicitudesAscenso
                .GroupBy(s => s.Estado)
                .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            // Convertir a nombres legibles de estado
            return resultado.Select(r => new ReporteDataDto
            {
                Categoria = GetEstadoDisplayName(r.Estado),
                Cantidad = r.Cantidad
            });
        }

        private string GetEstadoDisplayName(EstadoSolicitud estado)
        {
            return estado switch
            {
                EstadoSolicitud.Borrador => "Borrador",
                EstadoSolicitud.Enviada => "Enviada",
                EstadoSolicitud.EnRevision => "En Revisión",
                EstadoSolicitud.Aprobada => "Aprobada",
                EstadoSolicitud.Rechazada => "Rechazada",
                EstadoSolicitud.EnApelacion => "En Apelación",
                EstadoSolicitud.RechazadaDefinitiva => "Rechazada Definitivamente",
                EstadoSolicitud.AprobadaPorApelacion => "Aprobada por Apelación",
                EstadoSolicitud.CerradaSinRespuesta => "Cerrada por Falta de Respuesta",
                _ => estado.ToString()
            };
        }

        public async Task<IEnumerable<ReporteDataDto>> ObtenerSolicitudesPorNivel()
        {
            return await _context.SolicitudesAscenso
                .Include(s => s.RangoSolicitado)
                .GroupBy(s => s.RangoSolicitado.Nombre)
                .Select(g => new ReporteDataDto { Categoria = g.Key, Cantidad = g.Count() })
                .ToListAsync();
        }

        public async Task<IEnumerable<ReporteDataDto>> ObtenerSolicitudesPorMes(int anio)
        {
            var data = await _context.SolicitudesAscenso
                .Where(s => s.FechaCreacion.Year == anio)
                .GroupBy(s => s.FechaCreacion.Month)
                .Select(g => new { MesNumero = g.Key, Cantidad = g.Count() })
                .OrderBy(x => x.MesNumero)
                .ToListAsync();

            return data.Select(item => new ReporteDataDto
            {
                Categoria = CultureInfo.CreateSpecificCulture("es-ES").DateTimeFormat.GetMonthName(item.MesNumero),
                Cantidad = item.Cantidad
            });
        }
    }
}
