using Microsoft.EntityFrameworkCore;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Entities; // Asegúrate que el using a tus entidades es correcto
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Application.Interfaces;

namespace SIGAD.Application.Services
{
    public class ReporteBackendService
    {
        private readonly IApplicationDbContext _context;

        public ReporteBackendService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReporteDataDto>> ObtenerSolicitudesPorEstado()
        {
            return await _context.SolicitudesAscenso
                .GroupBy(s => s.Estado.ToString()) // <-- CAMBIO CLAVE AQUÍ
                .Select(g => new ReporteDataDto { Categoria = g.Key, Cantidad = g.Count() })
                .ToListAsync();
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
