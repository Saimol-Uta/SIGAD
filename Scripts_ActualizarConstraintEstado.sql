-- Script para actualizar el constraint de Estado en SolicitudesAscenso
-- Agregar soporte para el nuevo estado EN_APELACION = 6

USE [SISTEMA_DOCENTES] -- Cambia por el nombre de tu base de datos si es diferente
GO

-- Primero vamos a ver el constraint actual
SELECT 
    cc.CONSTRAINT_NAME,
    cc.CHECK_CLAUSE
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS cc
WHERE cc.CONSTRAINT_NAME LIKE '%Estado%' 
AND cc.TABLE_NAME = 'SolicitudesAscenso';
GO

-- Eliminar el constraint actual
ALTER TABLE [dbo].[SolicitudesAscenso] 
DROP CONSTRAINT [CK_SolicitudesAscenso_Estado];
GO

-- Crear el nuevo constraint que incluya TODOS los estados posibles
ALTER TABLE [dbo].[SolicitudesAscenso]
ADD CONSTRAINT [CK_SolicitudesAscenso_Estado] 
CHECK ([Estado] IN (1, 2, 3, 4, 5, 6, 7, 8, 9));
GO

-- Verificar que el constraint se creó correctamente
SELECT 
    cc.CONSTRAINT_NAME,
    cc.CHECK_CLAUSE,
    cc.TABLE_NAME
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS cc
WHERE cc.CONSTRAINT_NAME = 'CK_SolicitudesAscenso_Estado';
GO

PRINT 'Constraint actualizado exitosamente.';
PRINT 'Estados permitidos:';
PRINT '1 = Borrador';
PRINT '2 = Enviada';
PRINT '3 = En Revisión';
PRINT '4 = Aprobada';
PRINT '5 = Rechazada';
PRINT '6 = En Apelación';
PRINT '7 = Rechazada Definitiva';
PRINT '8 = Aprobada por Apelación';
PRINT '9 = Cerrada Sin Respuesta';
GO
