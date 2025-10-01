using SIGAD.BlazorApp.Models;
using AppDtos = SIGAD.Application.DTOs;

namespace SIGAD.BlazorApp.Extensions
{
    /// <summary>
    /// Métodos de extensión para mapear entre DTOs de Application y Blazor.Models.
    /// Fase 3 SOLID: Patrón Adapter para convertir entre capas.
    /// Solo mapea los DTOs principales que se usan en los clientes tipados.
    /// </summary>
    public static class DtoMappingExtensions
    {
        #region SolicitudDetalleDto Mappings

        /// <summary>
        /// Convierte SolicitudDetalleDto de Application a SolicitudDto de Blazor (versión simplificada).
        /// Usado cuando solo se necesitan los campos básicos para listar solicitudes.
        /// </summary>
        public static SolicitudDto ToBlazorSolicitudDto(this AppDtos.SolicitudDetalleDto source)
        {
            return new SolicitudDto
            {
                Id = source.Id,
                DocenteNombreCompleto = source.DocenteNombreCompleto,
                RangoSolicitadoNombre = source.RangoSolicitadoNombre,
                Estado = source.Estado,
                FechaEnvio = source.FechaEnvio
            };
        }

        /// <summary>
        /// Convierte una colección de SolicitudDetalleDto de Application a lista de SolicitudDto de Blazor.
        /// </summary>
        public static List<SolicitudDto> ToBlazorSolicitudDtoList(this IEnumerable<AppDtos.SolicitudDetalleDto> source)
        {
            return source.Select(s => s.ToBlazorSolicitudDto()).ToList();
        }

        #endregion

        #region LoginResponseDto Mapping

        /// <summary>
        /// Convierte LoginResponseDto de Application a LoginResponseDto de Blazor.
        /// Incluye conversión de enum Rol y DocenteInfoDto.
        /// </summary>
        public static LoginResponseDto ToBlazorLoginResponseDto(this AppDtos.LoginResponseDto source)
        {
            return new LoginResponseDto
            {
                Token = source.Token,
                Correo = source.Correo,
                Rol = (Models.Rol)(int)source.Rol, // Conversión de enum (mismo valor numérico)
                DocenteInfo = new Models.DocenteInfoDto
                {
                    Cedula = source.DocenteInfo.Cedula,
                    Nombre1 = source.DocenteInfo.Nombre1,
                    Nombre2 = source.DocenteInfo.Nombre2,
                    Apellido1 = source.DocenteInfo.Apellido1,
                    Apellido2 = source.DocenteInfo.Apellido2
                },
                ExpiracionToken = source.ExpiracionToken
            };
        }

        #endregion

        // TODO Fase 4: Agregar mappers para DTOs anidados (Tesis, Articulos, etc.) cuando se necesiten
        // Por ahora, las operaciones de detalle siguen usando HttpClient directo porque los DTOs anidados
        // tienen estructuras muy diferentes entre Application y Blazor.Models
    }
}

