using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.DTOs
{
    public class RangoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty; // Se inicializa con un valor predeterminado
        public int ArticulosRequeridos { get; set; }

      
    }
}