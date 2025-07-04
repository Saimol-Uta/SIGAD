-- Script para verificar los valores actuales de Estado en la tabla SolicitudesAscenso
-- Ejecuta este script para ver qué valores están siendo utilizados

USE [SISTEMA_DOCENTES]  -- Reemplaza con el nombre de tu base de datos
GO

-- Verificar los valores únicos de Estado actualmente en uso
SELECT DISTINCT Estado, COUNT(*) as Cantidad
FROM SolicitudesAscenso 
GROUP BY Estado
ORDER BY Estado;

-- Verificar la restricción CHECK actual
SELECT 
    cc.name AS ConstraintName,
    cc.definition AS ConstraintDefinition
FROM sys.check_constraints cc
INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
WHERE t.name = 'SolicitudesAscenso' 
  AND cc.name = 'CK_SolicitudesAscenso_Estado';

-- Mostrar información sobre la columna Estado
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length,
    c.is_nullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
INNER JOIN sys.tables ta ON c.object_id = ta.object_id
WHERE ta.name = 'SolicitudesAscenso' 
  AND c.name = 'Estado';
