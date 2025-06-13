-- =============================================================================
-- SCRIPT CORREGIDO PARA INSERTAR SOLICITUDES DE ASCENSO
-- =============================================================================
-- SOLUCIONADO: Usa valores de texto para el campo Estado según el constraint
-- =============================================================================

USE SISTEMA_DOCENTES;
GO

-- Verificar docentes existentes
PRINT '=== DOCENTES EXISTENTES ==='
SELECT Cedula, Nombre1, Apellido1, Apellido2 FROM Docentes;

-- Verificar rangos existentes  
PRINT '=== RANGOS EXISTENTES ==='
SELECT Id, Nombre FROM Rangos ORDER BY Id;

-- Verificar el constraint del campo Estado
PRINT '=== VERIFICANDO CONSTRAINT DE ESTADO ==='
SELECT 
    CONSTRAINT_NAME,
    CHECK_CLAUSE
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE '%Estado%';

-- =============================================================================
-- INSERTAR SOLICITUDES DE ASCENSO CON ESTADOS CORRECTOS
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

-- Solicitud 1: Estado "Borrador"
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
    NULL,        -- Sin fecha de envío porque está en borrador
    NULL,        -- Sin fecha de resolución
    1,           -- Estado: Borrador
    NULL         -- Sin observaciones aún
);

-- Solicitud 2: Estado "Enviada"
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
    2,           -- Estado: Enviada
    NULL
);

-- Solicitud 3: Estado "En Revision"
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
    3,           -- Estado: En Revision
    NULL
);

-- Solicitud 4: Estado "Aprobada"
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
    4,           -- Estado: Aprobada
    'Solicitud aprobada. El docente cumple con todos los requisitos establecidos para el ascenso.'
);

-- Solicitud 5: Estado "Rechazada"
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
    5,           -- Estado: Rechazada
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
        WHEN 3 THEN 'En Revision'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
        ELSE 'Desconocido'
    END as EstadoTexto,
    s.Estado as EstadoNumero,
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
        WHEN 3 THEN 'En Revision'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
        ELSE 'Desconocido'
    END AS EstadoDescripcion,
    CONCAT(d.Nombre1, ' ', d.Apellido1) AS Docente
FROM SolicitudesAscenso s
INNER JOIN Docentes d ON s.DocenteCedula = d.Cedula
ORDER BY 
    CASE s.Estado 
        WHEN 1 THEN 1
        WHEN 2 THEN 2
        WHEN 3 THEN 3
        WHEN 4 THEN 4
        WHEN 5 THEN 5
        ELSE 6
    END, 
    s.FechaCreacion DESC;

-- =============================================================================
-- VERIFICAR LOS VALORES PERMITIDOS PARA ESTADO
-- =============================================================================

PRINT '=== VALORES PERMITIDOS PARA ESTADO ==='
PRINT 'Según el constraint, los valores permitidos son:'
PRINT '- Borrador (1)'
PRINT '- Enviada (2)' 
PRINT '- En Revision (3)'
PRINT '- Aprobada (4)'
PRINT '- Rechazada (5)'

-- =============================================================================
-- SCRIPT PARA LIMPIAR (OPCIONAL)
-- =============================================================================

-- DESCOMENTA ESTAS LÍNEAS SOLO SI QUIERES ELIMINAR TODAS LAS SOLICITUDES
-- DELETE FROM ArticulosPorSolicitud WHERE SolicitudId IN (SELECT Id FROM SolicitudesAscenso);
-- DELETE FROM EvaluacionesPorSolicitud WHERE SolicitudId IN (SELECT Id FROM SolicitudesAscenso);
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

NOTA IMPORTANTE SOBRE EL CAMPO ESTADO:
- Aunque aparece como 'int' en la estructura, en realidad almacena texto
- Los valores permitidos son: 'Borrador', 'Enviada', 'En Revision', 'Aprobada', 'Rechazada'
- Debe usar exactamente estos textos (con mayúsculas y sin acentos en "Revision")
*/ 