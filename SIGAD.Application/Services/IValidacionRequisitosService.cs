using SIGAD.Application.DTOs.Validacion;
using System.Threading.Tasks;

public interface IValidacionRequisitosService
{
    Task<ProgresoRequisitosDto> VerificarProgresoAsync(string docenteCedula, int rangoId);
}
