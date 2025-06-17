using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGAD.Application.DTOs.Validacion;
using System.Threading.Tasks;

namespace SIGAD.Application.Services
{
    public interface IValidacionRequisitosService
    {
        Task<ProgresoRequisitosDto> VerificarProgresoAsync(string docenteCedula, int rangoId);
    }
}