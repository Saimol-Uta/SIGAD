-- Script para consultar los valores actuales de la tabla Rangos en SQL Server
-- Ejecuta este script primero para ver los datos actuales

USE [SISTEMA_DOCENTES]  -- Reemplaza con el nombre de tu base de datos
GO

-- Consultar todos los rangos actuales con todos sus campos
SELECT 
    Id,
    Nombre,
    ArticulosRequeridos,
    AniosExperienciaRequeridos,
    HorasCursoRequeridas,
    MesesInvestigacionRequeridos,
    TesisDirigidasRequeridas,
    PuntajePromedioEvaluacionesRequerido,
    HorasCapacitacionPedagogicaRequeridas,
    HorasCapacitacionImpartidaRequeridas,
    PublicacionesIdiomaExtranjeroRequeridas,
    ProyectosInternacionalesRequeridos,
    RequiereArticuloEnGradoActual,
    PermiteCoordinacionProyectos
FROM Rangos
ORDER BY Id;

-- Información adicional: contar cuántos rangos existen
SELECT COUNT(*) as TotalRangos FROM Rangos;

-- Verificar si existen solicitudes asociadas a estos rangos
SELECT 
    r.Nombre as RangoNombre,
    COUNT(s.Id) as SolicitudesAsociadas
FROM Rangos r
LEFT JOIN SolicitudesAscenso s ON (s.RangoActualId = r.Id OR s.RangoSolicitadoId = r.Id)
GROUP BY r.Id, r.Nombre
ORDER BY r.Id;
