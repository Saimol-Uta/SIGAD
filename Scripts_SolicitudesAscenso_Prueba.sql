-- =============================================================================
-- SCRIPT PARA INSERTAR SOLICITUDES DE ASCENSO DE PRUEBA
-- =============================================================================
-- Este script inserta solicitudes de ascenso para probar el API de artículos
-- Asegúrate de tener docentes y rangos existentes en la base de datos
-- =============================================================================

USE SISTEMA_DOCENTES;
GO

-- Verificar docentes existentes
PRINT 'Docentes existentes en la base de datos:'
SELECT Cedula, Nombre1, Apellido1 FROM Docentes;

-- Verificar rangos existentes  
PRINT 'Rangos existentes en la base de datos:'
SELECT Id, Nombre FROM Rangos;

-- =============================================================================
-- INSERTAR SOLICITUDES DE ASCENSO DE PRUEBA
-- =============================================================================

-- Nota: Los valores del enum EstadoSolicitud son:
-- 1 = Borrador
-- 2 = Enviada  
-- 3 = EnRevision
-- 4 = Aprobada
-- 5 = Rechazada

-- Solicitud 1: Borrador (para poder asociar artículos)
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    Estado
) VALUES (
    '1234567890',  -- Cambiar por una cédula que exista en tu BD
    1,             -- Cambiar por un ID de rango actual que exista
    2,             -- Cambiar por un ID de rango solicitado que exista
    GETDATE(),
    1              -- Estado: Borrador
);

-- Solicitud 2: Enviada
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    Estado
) VALUES (
    '1234567890',  -- Cambiar por una cédula que exista en tu BD
    1,             -- Cambiar por un ID de rango actual que exista
    3,             -- Cambiar por un ID de rango solicitado que exista
    DATEADD(day, -5, GETDATE()),
    DATEADD(day, -3, GETDATE()),
    2              -- Estado: Enviada
);

-- Solicitud 3: En Revisión
INSERT INTO SolicitudesAscenso (
    DocenteCedula, 
    RangoActualId, 
    RangoSolicitadoId, 
    FechaCreacion, 
    FechaEnvio,
    Estado
) VALUES (
    '0987654321',  -- Cambiar por otra cédula que exista en tu BD
    2,             -- Cambiar por un ID de rango actual que exista
    3,             -- Cambiar por un ID de rango solicitado que exista
    DATEADD(day, -10, GETDATE()),
    DATEADD(day, -8, GETDATE()),
    3              -- Estado: EnRevision
);

-- Solicitud 4: Aprobada
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
    '0987654321',  -- Cambiar por una cédula que exista en tu BD
    1,             -- Cambiar por un ID de rango actual que exista
    2,             -- Cambiar por un ID de rango solicitado que exista
    DATEADD(day, -20, GETDATE()),
    DATEADD(day, -18, GETDATE()),
    DATEADD(day, -2, GETDATE()),
    4,             -- Estado: Aprobada
    'Solicitud aprobada. Cumple con todos los requisitos.'
);

-- Solicitud 5: Rechazada
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
    '1111111111',  -- Cambiar por una cédula que exista en tu BD
    1,             -- Cambiar por un ID de rango actual que exista
    3,             -- Cambiar por un ID de rango solicitado que exista
    DATEADD(day, -15, GETDATE()),
    DATEADD(day, -13, GETDATE()),
    DATEADD(day, -1, GETDATE()),
    5,             -- Estado: Rechazada
    'No cumple con el número mínimo de artículos requeridos.'
);

-- =============================================================================
-- VERIFICAR SOLICITUDES INSERTADAS
-- =============================================================================

PRINT 'Solicitudes insertadas:';
SELECT 
    s.Id,
    s.DocenteCedula,
    d.Nombre1 + ' ' + d.Apellido1 AS NombreDocente,
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
    END AS Estado,
    s.ObservacionesAdmin
FROM SolicitudesAscenso s
INNER JOIN Docentes d ON s.DocenteCedula = d.Cedula
LEFT JOIN Rangos ra ON s.RangoActualId = ra.Id
INNER JOIN Rangos rs ON s.RangoSolicitadoId = rs.Id
ORDER BY s.FechaCreacion DESC;

-- =============================================================================
-- SCRIPT PARA OBTENER IDs DE SOLICITUDES (útil para pruebas de API)
-- =============================================================================

PRINT 'GUIDs de solicitudes para usar en pruebas de API:';
SELECT 
    'Solicitud ID: ' + CAST(Id AS VARCHAR(36)) + 
    ' - Estado: ' + CASE Estado 
        WHEN 1 THEN 'Borrador'
        WHEN 2 THEN 'Enviada'
        WHEN 3 THEN 'En Revisión'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
    END AS SolicitudInfo
FROM SolicitudesAscenso
ORDER BY FechaCreacion DESC;

-- =============================================================================
-- NOTAS IMPORTANTES:
-- =============================================================================
-- 1. Cambiar las cédulas de docentes por las que realmente existan en tu BD
-- 2. Cambiar los IDs de rangos por los que realmente existan en tu BD
-- 3. Las solicitudes en estado "Borrador" son ideales para asociar artículos
-- 4. Guarda los GUIDs generados para usarlos en las pruebas del API
-- ============================================================================= 