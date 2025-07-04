-- Script para crear la tabla de Apelaciones en SIGAD
-- Este script debe ejecutarse en la base de datos existente

USE [SISTEMA_DOCENTES] -- Cambia por el nombre de tu base de datos si es diferente
GO

-- Crear tabla Apelaciones
CREATE TABLE [dbo].[Apelaciones](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [SolicitudAscensoId] [uniqueidentifier] NOT NULL,
    [Motivo] [nvarchar](2000) NOT NULL,
    [DocumentosRespaldo] [nvarchar](500) NULL,
    [FechaPresentacion] [datetime2](7) NOT NULL,
    [FechaLimiteRespuesta] [datetime2](7) NOT NULL,
    [Estado] [int] NOT NULL,
    [ObservacionesComision] [nvarchar](1000) NULL,
    [FechaResolucion] [datetime2](7) NULL,
    [Aceptada] [bit] NOT NULL DEFAULT 0,
    [FechaCreacion] [datetime2](7) NOT NULL,
    [CreadoPor] [nvarchar](50) NOT NULL,
    [FechaModificacion] [datetime2](7) NULL,
    [ModificadoPor] [nvarchar](50) NULL,
    CONSTRAINT [PK_Apelaciones] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Apelaciones_SolicitudAscenso] FOREIGN KEY([SolicitudAscensoId]) 
        REFERENCES [dbo].[SolicitudAscenso] ([Id]) ON DELETE CASCADE
)
GO

-- Crear índices para mejorar el rendimiento
CREATE NONCLUSTERED INDEX [IX_Apelaciones_SolicitudAscensoId] 
ON [dbo].[Apelaciones] ([SolicitudAscensoId])
GO

CREATE NONCLUSTERED INDEX [IX_Apelaciones_Estado] 
ON [dbo].[Apelaciones] ([Estado])
GO

CREATE NONCLUSTERED INDEX [IX_Apelaciones_FechaPresentacion] 
ON [dbo].[Apelaciones] ([FechaPresentacion])
GO

-- Agregar comentarios a las columnas
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Identificador único de la apelación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'Id'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'ID de la solicitud de ascenso asociada', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'SolicitudAscensoId'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Motivo detallado de la apelación (máximo 2000 caracteres)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'Motivo'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Lista de documentos de respaldo adicionales (rutas de archivos separadas por coma)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'DocumentosRespaldo'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha en que se presentó la apelación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'FechaPresentacion'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha límite para resolver la apelación (3 días según reglamento)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'FechaLimiteRespuesta'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Estado de la apelación: 1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Vencida', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'Estado'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Observaciones de la Comisión sobre la resolución de la apelación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'ObservacionesComision'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Fecha en que se resolvió la apelación', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'FechaResolucion'
GO

EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Indica si la apelación fue aceptada (true) o rechazada (false)', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'Apelaciones', 
    @level2type = N'COLUMN', @level2name = N'Aceptada'
GO

-- Verificar que la tabla se creó correctamente
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Apelaciones' 
ORDER BY ORDINAL_POSITION;

PRINT 'Tabla Apelaciones creada exitosamente.'
PRINT 'Estructura de la tabla:'
PRINT '- Id: Clave primaria autoincremental'
PRINT '- SolicitudAscensoId: Referencia a la solicitud (FK)'
PRINT '- Motivo: Texto de la justificación (hasta 2000 caracteres)'
PRINT '- DocumentosRespaldo: Rutas de archivos adicionales'
PRINT '- FechaPresentacion: Cuando se presentó la apelación'
PRINT '- FechaLimiteRespuesta: Límite para resolverla (3 días)'
PRINT '- Estado: 1=Pendiente, 2=Aceptada, 3=Rechazada, 4=Vencida'
PRINT '- ObservacionesComision: Respuesta de la comisión'
PRINT '- FechaResolucion: Cuando se resolvió'
PRINT '- Aceptada: Si fue aceptada o no'
PRINT '- Campos de auditoría: FechaCreacion, CreadoPor, etc.'
