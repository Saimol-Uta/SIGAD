-- Script para actualizar el constraint de Estado en SolicitudesAscenso
-- Usar los nombres exactos del enum como aparecen en C#

USE [SISTEMA_DOCENTES] -- Cambia por el nombre de tu base de datos si es diferente
GO

-- Primero vamos a ver los valores actuales de Estado
SELECT DISTINCT Estado FROM SolicitudesAscenso;
GO

-- Eliminar el constraint actual si existe
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS WHERE CONSTRAINT_NAME = 'CK_SolicitudesAscenso_Estado')
BEGIN
    ALTER TABLE [dbo].[SolicitudesAscenso] 
    DROP CONSTRAINT [CK_SolicitudesAscenso_Estado];
END
GO

-- Crear el nuevo constraint que incluya todos los estados usando los nombres exactos del enum
ALTER TABLE [dbo].[SolicitudesAscenso]
ADD CONSTRAINT [CK_SolicitudesAscenso_Estado] 
CHECK ([Estado] IN (
    'Borrador', 
    'Enviada', 
    'EnRevision', 
    'Aprobada', 
    'Rechazada', 
    'EnApelacion', 
    'RechazadaDefinitiva', 
    'AprobadaPorApelacion', 
    'CerradaSinRespuesta'
));
GO

PRINT 'Constraint actualizado exitosamente con nombres exactos del enum.';
PRINT 'Estados permitidos:';
PRINT 'Borrador';
PRINT 'Enviada';
PRINT 'EnRevision';
PRINT 'Aprobada';
PRINT 'Rechazada';
PRINT 'EnApelacion';
PRINT 'RechazadaDefinitiva';
PRINT 'AprobadaPorApelacion';
PRINT 'CerradaSinRespuesta';
GO
