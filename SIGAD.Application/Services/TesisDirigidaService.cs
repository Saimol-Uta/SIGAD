//using SIGAD.Application.DTOs;
//using SIGAD.Domain.Entities;
//using SIGAD.Domain.Interfaces;

//namespace SIGAD.Application.Services
//{
//    public class TesisDirigidaService
//    {
//        private readonly ITesisDirigidaRepository _repository;

//        public TesisDirigidaService(ITesisDirigidaRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<IEnumerable<TesisDirigidaDto>> ObtenerPorDocenteAsync(string cedula)
//        {
//            var tesis = await _repository.GetByDocenteCedulaAsync(cedula);
//            return tesis.Select(t => new TesisDirigidaDto
//            {
//                Id = t.Id,
//                DocenteCedula = t.DocenteCedula,
//                NivelAcademico = t.NivelAcademico,
//                TituloTesis = t.TituloTesis,
//                Estado = t.Estado,
//                FechaInicio = t.FechaInicio,
//                FechaFin = t.FechaFin,
//                Institucion = t.Institucion,
//                CertificacionRuta = t.CertificacionRuta
//            });
//        }

//        public async Task<TesisDirigidaDto> CrearAsync(CreateTesisDirigidaDto dto)
//        {
//            var tesis = new TesisDirigida
//            {
//                DocenteCedula = dto.DocenteCedula,
//                NivelAcademico = dto.NivelAcademico,
//                TituloTesis = dto.TituloTesis,
//                Estado = dto.Estado,
//                FechaInicio = dto.FechaInicio,
//                FechaFin = dto.FechaFin,
//                Institucion = dto.Institucion,
//                CertificacionRuta = dto.CertificacionRuta,
//                ContenidoHash = string.Empty // Puedes calcular el hash si es necesario
//            };

//            await _repository.AddAsync(tesis);
//            return new TesisDirigidaDto
//            {
//                Id = tesis.Id,
//                DocenteCedula = tesis.DocenteCedula,
//                NivelAcademico = tesis.NivelAcademico,
//                TituloTesis = tesis.TituloTesis,
//                Estado = tesis.Estado,
//                FechaInicio = tesis.FechaInicio,
//                FechaFin = tesis.FechaFin,
//                Institucion = tesis.Institucion,
//                CertificacionRuta = tesis.CertificacionRuta
//            };
//        }

//        public async Task AsociarASolicitudAsync(Guid solicitudId, int tesisId)
//        {
//            await _repository.AddToSolicitudAsync(solicitudId, tesisId);
//        }

//        public async Task DesasociarDeSolicitudAsync(Guid solicitudId, int tesisId)
//        {
//            await _repository.RemoveFromSolicitudAsync(solicitudId, tesisId);
//        }

//        public async Task<bool> ExistePorHashAsync(string hash)
//        {
//            return await _repository.ExistsByHashAsync(hash);
//        }
//    }
//}
