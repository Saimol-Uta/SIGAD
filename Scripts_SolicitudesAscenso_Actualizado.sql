-- =============================================================================
-- SCRIPT ACTUALIZADO PARA INSERTAR SOLICITUDES DE ASCENSO
-- =============================================================================
-- Basado en la estructura real de la tabla SolicitudesAscenso
-- =============================================================================

USE SISTEMA_DOCENTES;
GO

-- Verificar docentes existentes
PRINT '=== DOCENTES EXISTENTES ==='
SELECT Cedula, Nombre1, Apellido1, Apellido2 FROM Docentes;

-- Verificar rangos existentes  
PRINT '=== RANGOS EXISTENTES ==='
SELECT Id, Nombre FROM Rangos ORDER BY Id;

-- Verificar solicitudes existentes (si las hay)
PRINT '=== SOLICITUDES EXISTENTES ==='
SELECT COUNT(*) as TotalSolicitudes FROM SolicitudesAscenso;

-- =============================================================================
-- INSERTAR SOLICITUDES DE ASCENSO USANDO TU ESTRUCTURA
-- =============================================================================

-- IMPORTANTE: Reemplaza estas cédulas por las que realmente existan en tu BD
DECLARE @Docente1 NVARCHAR(10) = '1234567890';  -- Cambiar por cédula real
DECLARE @Docente2 NVARCHAR(10) = '0987654321';  -- Cambiar por cédula real  
DECLARE @Docente3 NVARCHAR(10) = '1111111111';  -- Cambiar por cédula real

-- IMPORTANTE: Reemplaza estos IDs por los que realmente existan en tu BD
DECLARE @RangoActual1 INT = 1;      -- Cambiar por ID real
DECLARE @RangoActual2 INT = 2;      -- Cambiar por ID real
DECLARE @RangoSolicitado1 INT = 2;  -- Cambiar por ID real
DECLARE @RangoSolicitado2 INT = 3;  -- Cambiar por ID real

-- Solicitud 1: Estado Borrador (Estado = 1)
-- Ideal para asociar artículos ya que está en borrador
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    FechaResolucion,
    Estado,
    ObservacionesAdmin
) VALUES (
    @Docente1,
    @RangoActual1,
    @RangoSolicitado1,
    GETDATE(),
    NULL,  -- Sin fecha de envío porque está en borrador
    NULL,  -- Sin fecha de resolución
    1,     -- Estado: Borrador
    NULL   -- Sin observaciones aún
);

-- Solicitud 2: Estado Enviada (Estado = 2)
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    FechaResolucion,
    Estado,
    ObservacionesAdmin
) VALUES (
    @Docente1,
    @RangoActual1,
    @RangoSolicitado2,
    DATEADD(day, -7, GETDATE()),
    DATEADD(day, -5, GETDATE()),
    NULL,
    2,     -- Estado: Enviada
    NULL
);

-- Solicitud 3: Estado En Revisión (Estado = 3)
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    FechaResolucion,
    Estado,
    ObservacionesAdmin
) VALUES (
    @Docente2,
    @RangoActual2,
    @RangoSolicitado2,
    DATEADD(day, -15, GETDATE()),
    DATEADD(day, -12, GETDATE()),
    NULL,
    3,     -- Estado: En Revisión
    NULL
);

-- Solicitud 4: Estado Aprobada (Estado = 4)
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    FechaResolucion,
    Estado,
    ObservacionesAdmin
) VALUES (
    @Docente2,
    @RangoActual1,
    @RangoSolicitado1,
    DATEADD(day, -30, GETDATE()),
    DATEADD(day, -28, GETDATE()),
    DATEADD(day, -3, GETDATE()),
    4,     -- Estado: Aprobada
    'Solicitud aprobada. El docente cumple con todos los requisitos establecidos para el ascenso.'
);

-- Solicitud 5: Estado Rechazada (Estado = 5)
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    FechaResolucion,
    Estado,
    ObservacionesAdmin
) VALUES (
    @Docente3,
    @RangoActual1,
    @RangoSolicitado2,
    DATEADD(day, -25, GETDATE()),
    DATEADD(day, -23, GETDATE()),
    DATEADD(day, -1, GETDATE()),
    5,     -- Estado: Rechazada
    'Solicitud rechazada. No cumple con el número mínimo de artículos científicos requeridos para el rango solicitado.'
);

-- =============================================================================
-- VERIFICAR SOLICITUDES INSERTADAS
-- =============================================================================

PRINT '=== SOLICITUDES INSERTADAS CORRECTAMENTE ==='
SELECT 
    s.Id,
    s.DocenteCedula,
    CONCAT(d.Nombre1, ' ', ISNULL(d.Nombre2 + ' ', ''), d.Apellido1, ' ', d.Apellido2) AS NombreCompleto,
    ra.Nombre AS RangoActual,
    rs.Nombre AS RangoSolicitado,
    s.FechaCreacion,
    s.FechaEnvio,
    s.FechaResolucion,
    CASE s.Estado 
        WHEN 1 THEN 'Borrador'
        WHEN 2 THEN 'Enviada'
        WHEN 3 THEN 'En Revisión'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
        ELSE 'Desconocido'
    END AS EstadoTexto,
    s.Estado AS EstadoNumero,
    s.ObservacionesAdmin
FROM SolicitudesAscenso s
INNER JOIN Docentes d ON s.DocenteCedula = d.Cedula
LEFT JOIN Rangos ra ON s.RangoActualId = ra.Id
INNER JOIN Rangos rs ON s.RangoSolicitadoId = rs.Id
ORDER BY s.FechaCreacion DESC;

-- =============================================================================
-- OBTENER GUIDs PARA PRUEBAS DEL API
-- =============================================================================

PRINT '=== GUIDs PARA USAR EN PRUEBAS DEL API ==='
SELECT 
    CAST(s.Id AS VARCHAR(36)) AS SolicitudGUID,
    CASE s.Estado 
        WHEN 1 THEN 'Borrador (Ideal para asociar artículos)'
        WHEN 2 THEN 'Enviada'
        WHEN 3 THEN 'En Revisión'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
    END AS Estado,
    CONCAT(d.Nombre1, ' ', d.Apellido1) AS Docente
FROM SolicitudesAscenso s
INNER JOIN Docentes d ON s.DocenteCedula = d.Cedula
ORDER BY s.Estado, s.FechaCreacion DESC;

-- =============================================================================
-- SCRIPT PARA LIMPIAR (OPCIONAL - SOLO SI NECESITAS EMPEZAR DE NUEVO)
-- =============================================================================

-- DESCOMENTA ESTAS LÍNEAS SOLO SI QUIERES ELIMINAR TODAS LAS SOLICITUDES
-- DELETE FROM ArticulosPorSolicitud;
-- DELETE FROM EvaluacionesPorSolicitud; 
-- DELETE FROM SolicitudesAscenso;
-- PRINT 'Todas las solicitudes han sido eliminadas';

-- =============================================================================
-- INSTRUCCIONES PARA PERSONALIZAR
-- =============================================================================

/*
ANTES DE EJECUTAR ESTE SCRIPT:

1. Ejecuta la primera parte para ver qué docentes y rangos tienes
2. Modifica las variables @Docente1, @Docente2, @Docente3 con cédulas reales
3. Modifica las variables @RangoActual1, @RangoActual2, etc. con IDs reales
4. Ejecuta el script completo
5. Guarda los GUIDs mostrados para usarlos en las pruebas del API

EJEMPLO DE COMO MODIFICAR:
DECLARE @Docente1 NVARCHAR(10) = '1723456789';  -- Tu cédula real
DECLARE @RangoActual1 INT = 1;                   -- ID del rango "Instructor"
DECLARE @RangoSolicitado1 INT = 2;               -- ID del rango "Asistente"
*/ 