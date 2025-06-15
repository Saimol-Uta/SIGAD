namespace SIGAD.Application.DTOs.IntegracionesExternas
{
    public class DocenteDto
    {
        public string Cedula { get; set; } = default!;
        public string NombreCompleto { get; set; } = default!;
        public string Correo { get; set; } = default!;
        public string ClaveHash { get; set; } = default!;
        public string Rol { get; set; } = default!;
    }
}

