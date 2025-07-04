-- Script para actualizar el constraint de Estado en SolicitudesAscenso
-- El campo Estado es NVARCHAR, así que usamos nombres de estados en lugar de números

USE [SISTEMA_DOCENTES] -- Cambia por el nombre de tu base de datos si es diferente
GO

-- Primero vamos a ver los valores actuales de Estado
SELECT DISTINCT Estado FROM SolicitudesAscenso;
GO

-- Eliminar el constraint actual
ALTER TABLE [dbo].[SolicitudesAscenso] 
DROP CONSTRAINT [CK_SolicitudesAscenso_Estado];
GO

-- Crear el nuevo constraint que incluya el estado EN_APELACION como texto
ALTER TABLE [dbo].[SolicitudesAscenso]
ADD CONSTRAINT [CK_SolicitudesAscenso_Estado] 
CHECK ([Estado] IN (
    'BORRADOR', 
    'ENVIADA', 
    'EN_REVISION', 
    'APROBADA', 
    'RECHAZADA', 
    'EN_APELACION', 
    'RECHAZADA_DEFINITIVA', 
    'APROBADA_POR_APELACION', 
    'CERRADA_SIN_RESPUESTA'
));
GO

PRINT 'Constraint actualizado exitosamente para usar cadenas de texto.';
PRINT 'Estados permitidos:';
PRINT 'BORRADOR';
PRINT 'ENVIADA';
PRINT 'EN_REVISION';
PRINT 'APROBADA';
PRINT 'RECHAZADA';
PRINT 'EN_APELACION';
PRINT 'RECHAZADA_DEFINITIVA';
PRINT 'APROBADA_POR_APELACION';
PRINT 'CERRADA_SIN_RESPUESTA';
GO
