-- Script de diagnóstico y corrección para el problema de estados de solicitud
-- Ejecuta este script paso a paso en SQL Server Management Studio

USE [SISTEMA_DOCENTES]  -- Reemplaza con el nombre de tu base de datos
GO

PRINT '=== DIAGNÓSTICO DEL PROBLEMA DE ESTADOS ===';
PRINT '';

-- 1. Verificar la restricción CHECK actual
PRINT '1. RESTRICCIÓN CHECK ACTUAL:';
SELECT 
    cc.name AS ConstraintName,
    cc.definition AS ConstraintDefinition
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
WHERE t.name = 'SolicitudesAscenso' 
  AND cc.name = 'CK_SolicitudesAscenso_Estado';

PRINT '';

-- 2. Verificar los valores únicos de Estado actualmente en uso
PRINT '2. VALORES DE ESTADO ACTUALMENTE EN LA TABLA:';
SELECT DISTINCT Estado, COUNT(*) as Cantidad
FROM SolicitudesAscenso 
GROUP BY Estado
ORDER BY Estado;

PRINT '';

-- 3. Verificar la tabla __EFMigrationsHistory para ver qué migraciones se han aplicado
PRINT '3. MIGRACIONES APLICADAS:';
SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;

PRINT '';

-- 4. Intentar el UPDATE que está fallando para ver el error exacto
PRINT '4. SIMULANDO EL ERROR:';
BEGIN TRY
    -- Crear una transacción de prueba
    BEGIN TRANSACTION TestTransaction;
    
    -- Intentar insertar el estado problemático
    INSERT INTO SolicitudesAscenso (
        Id, DocenteCedula, RangoSolicitadoId, FechaCreacion, Estado,
        NotificacionEnviada, AprobadoPorComision, AprobadoPorConsejo
    ) VALUES (
        NEWID(), '1234567890', 1, GETDATE(), 'En Apelacion',
        0, 0, 0
    );
    
    PRINT 'SUCCESS: El estado "En Apelacion" es válido';
    ROLLBACK TRANSACTION TestTransaction;
    
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION TestTransaction;
    PRINT 'ERROR DETECTADO:';
    PRINT 'Mensaje: ' + ERROR_MESSAGE();
    PRINT 'Número: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Línea: ' + CAST(ERROR_LINE() AS VARCHAR(10));
END CATCH

PRINT '';

-- 5. Mostrar la definición completa de la tabla
PRINT '5. ESTRUCTURA ACTUAL DE LA COLUMNA Estado:';
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length,
    c.is_nullable,
    c.collation_name
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
INNER JOIN sys.tables ta ON c.object_id = ta.object_id
WHERE ta.name = 'SolicitudesAscenso' 
  AND c.name = 'Estado';

PRINT '';
PRINT '=== FIN DEL DIAGNÓSTICO ===';

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
