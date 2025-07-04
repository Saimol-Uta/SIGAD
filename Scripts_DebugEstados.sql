-- Script para verificar el estado de las solicitudes
SELECT 
    sa.Id,
    d.Nombre1 + ' ' + d.Apellido1 AS DocenteNombre,
    sa.Estado,
    sa.TipoResolucion,
    sa.FechaCreacion,
    COUNT(a.Id) AS NumeroApelaciones
FROM SolicitudesAscenso sa
INNER JOIN Docentes d ON sa.DocenteCedula = d.Cedula
LEFT JOIN Apelaciones a ON sa.Id = a.SolicitudAscensoId
GROUP BY sa.Id, d.Nombre1, d.Apellido1, sa.Estado, sa.TipoResolucion, sa.FechaCreacion
ORDER BY sa.FechaCreacion DESC;

-- Verificar específicamente solicitudes en estado EnApelacion
SELECT 
    sa.Id,
    d.Nombre1 + ' ' + d.Apellido1 AS DocenteNombre,
    sa.Estado,
    sa.TipoResolucion,
    sa.FechaCreacion
FROM SolicitudesAscenso sa
INNER JOIN Docentes d ON sa.DocenteCedula = d.Cedula
WHERE sa.Estado = 6; -- EnApelacion

-- Verificar apelaciones existentes
SELECT 
    a.Id,
    a.SolicitudAscensoId,
    a.Estado AS EstadoApelacion,
    a.FechaPresentacion,
    a.FechaLimiteRespuesta,
    d.Nombre1 + ' ' + d.Apellido1 AS DocenteNombre
FROM Apelaciones a
INNER JOIN SolicitudesAscenso sa ON a.SolicitudAscensoId = sa.Id
INNER JOIN Docentes d ON sa.DocenteCedula = d.Cedula
ORDER BY a.FechaPresentacion DESC;
