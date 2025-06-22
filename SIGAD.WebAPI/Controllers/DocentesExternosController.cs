//using Microsoft.AspNetCore.Mvc;
//using SIGAD.Application.Interfaces.Integraciones;
//using SIGAD.Domain.Entities;
//using SIGAD.Domain.Enums;
//using SIGAD.Domain.Interfaces;

//namespace SIGAD.WebAPI.Controllers
//{
//    [ApiController]
//    [Route("api/docentes/externos")]
//    public class DocentesExternosController : ControllerBase
//    {
//        private readonly IDiticSyncService _ditic;
//        private readonly IUnitOfWork _unitOfWork;

//        public DocentesExternosController(IDiticSyncService ditic, IUnitOfWork unitOfWork)
//        {
//            _ditic = ditic;
//            _unitOfWork = unitOfWork;
//        }

//        [HttpPost("importar")]
//        public async Task<IActionResult> ImportarDocentes()
//        {
//            var externos = await _ditic.ObtenerDocentesAsync();
//            int insertados = 0;

//            foreach (var dto in externos)
//            {
//                // Verificar si ya existe una cuenta con el correo
//                bool existeCorreo = await _unitOfWork.Cuentas.ExistePorCorreoAsync(dto.Correo);
//                if (existeCorreo)
//                    continue;

//                // Verificar si ya existe una cuenta con la cédula
//                bool existeCedulaCuenta = await _unitOfWork.Cuentas.ExistePorCedulaAsync(dto.Cedula);
//                if (existeCedulaCuenta)
//                    continue;

//                // Verificar si ya existe el docente
//                var docenteExistente = await _unitOfWork.Docentes.ObtenerPorCedulaAsync(dto.Cedula);
//                if (docenteExistente == null)
//                {
//                    var partes = dto.NombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);

//                    var nuevoDocente = new Docente
//                    {
//                        Cedula = dto.Cedula,
//                        Nombre1 = partes.ElementAtOrDefault(0),
//                        Nombre2 = partes.ElementAtOrDefault(1),
//                        Apellido1 = partes.ElementAtOrDefault(2),
//                        Apellido2 = partes.ElementAtOrDefault(3)
//                    };

//                    await _unitOfWork.Docentes.AgregarAsync(nuevoDocente);
//                }

//                var cuenta = new Cuenta
//                {
//                    Correo = dto.Correo,
//                    ClaveHash = dto.ClaveHash,
//                    Rol = Enum.Parse<Rol>(dto.Rol, ignoreCase: true),
//                    DocenteCedula = dto.Cedula
//                };

//                await _unitOfWork.Cuentas.AgregarAsync(cuenta);
//                insertados++;
//            }

//            await _unitOfWork.CompleteAsync();

//            return Ok(new { mensaje = $"Se importaron {insertados} docentes nuevos desde DITIC." });
//        }
//    }
//}
