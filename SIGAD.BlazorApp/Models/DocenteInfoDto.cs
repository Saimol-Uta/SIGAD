namespace SIGAD.BlazorApp.Models
{
  
        public class DocenteInfoDTO
        {
            public string Cedula { get; set; } = string.Empty;
            public string Nombre1 { get; set; } = string.Empty;
            public string? Nombre2 { get; set; }
            public string Apellido1 { get; set; } = string.Empty;
            public string Apellido2 { get; set; } = string.Empty;
            public string NombreCompleto => $"{Nombre1} {Nombre2} {Apellido1} {Apellido2}".Replace("  ", " ").Trim();
        }
    }
