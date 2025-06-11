using SIGAD.Application.DTOs;
using SIGAD.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public class GestionSolicitudesAppService
    {
        private readonly ISolicitudAscensoRepository _solicitudRepository;
        // private readonly IUnitOfWork _unitOfWork;

        // Inyectamos las dependencias que necesitará
        public GestionSolicitudesAppService(ISolicitudAscensoRepository solicitudRepository /*, IUnitOfWork unitOfWork*/)
        {
            _solicitudRepository = solicitudRepository;
            // _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VerSolicitudDto>> GetAllSolicitudesAsync()
        {
            // TAREA para un compañero: Implementar la lógica real para obtener y mapear las solicitudes.
            Console.WriteLine("Lógica para obtener todas las solicitudes no implementada.");
            await Task.Delay(10); // Simula trabajo asíncrono
            return new List<VerSolicitudDto>(); // Devuelve una lista vacía por ahora
        }

        public async Task<Guid> CrearSolicitudAsync(CrearSolicitudDto dto, string docenteCedula)
        {
            // TAREA para un compañero: Implementar lógica para crear la entidad SolicitudAscenso,
            // añadirla con el repositorio y guardar los cambios con UnitOfWork.
            Console.WriteLine("Lógica para crear solicitud no implementada.");
            await Task.Delay(10);
            return Guid.NewGuid(); // Devuelve un Guid de prueba
        }

        public async Task AprobarSolicitudAsync(Guid solicitudId)
        {
            // TAREA para un compañero: Implementar lógica para obtener la solicitud,
            // cambiar su estado a "Aprobada", y guardar los cambios.
            Console.WriteLine($"Lógica para aprobar solicitud {solicitudId} no implementada.");
            await Task.Delay(10);
        }

        public async Task RechazarSolicitudAsync(Guid solicitudId)
        {
            // TAREA para un compañero: Implementar lógica para obtener la solicitud,
            // cambiar su estado a "Rechazada", y guardar los cambios.
            Console.WriteLine($"Lógica para rechazar solicitud {solicitudId} no implementada.");
            await Task.Delay(10);
        }
    }
}
