using System.Threading.Tasks;
using SIGAD.Application.DTOs;

namespace SIGAD.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de generación de documentos de acción de personal
    /// </summary>
    public interface IAccionPersonalService
    {
        /// <summary>
        /// Genera un documento PDF de acción de personal para un docente promovido
        /// </summary>
        /// <param name="datos">Datos necesarios para generar el documento</param>
        /// <returns>Array de bytes del documento PDF generado</returns>
        Task<byte[]> GenerarAccionPersonalPdfAsync(AccionPersonalDto datos);
    }
} 