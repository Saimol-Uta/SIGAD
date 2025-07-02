using SIGAD.Application.DTOs;
using SIGAD.Application.Services;
using SIGAD.Domain.Entities;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;

public class ApelacionService : IApelacionService
{
    private readonly IApelacionRepository _apelacionRepository;
    private readonly ISolicitudAscensoRepository _solicitudRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApelacionService(
        IApelacionRepository apelacionRepository,
        ISolicitudAscensoRepository solicitudRepository,
        IUnitOfWork unitOfWork)
    {
        _apelacionRepository = apelacionRepository;
        _solicitudRepository = solicitudRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CrearApelacionAsync(CrearApelacionDto dto, string SolicitudId)
    {
        var solicitud = await _solicitudRepository.GetByDocenteAsync(dto.SolicitudId.ToString());
        if (solicitud == null)
            throw new Exception("La solicitud no fue encontrada.");

        if (string.IsNullOrWhiteSpace(dto.DocumentoUrl))
            throw new Exception("Debe ingresar un enlace al documento de respaldo.");

        var apelacion = new Apelacion
        {
            SolicitudId = dto.SolicitudId,
            Motivo = dto.Motivo,
            ArchivoRuta = dto.DocumentoUrl,  // Guardamos el link
            ArchivoNombre = "Documento en línea",
            Fecha = DateTime.UtcNow,
            Estado = EstadoApelacion.Pendiente
        };

        await _apelacionRepository.AddAsync(apelacion);
        await _unitOfWork.SaveChangesAsync();
    }
}
