-- Migración manual para agregar campos de aprobación según Reglamento UTA
-- Artículo 5: Responsabilidades del Honorable Consejo Universitario y Comisión Académica

-- Agregar columnas para el flujo de aprobación de dos etapas
ALTER TABLE [SolicitudesAscenso] 
ADD [AprobadoPorComision] bit NOT NULL DEFAULT 0;

ALTER TABLE [SolicitudesAscenso] 
ADD [AprobadoPorConsejo] bit NOT NULL DEFAULT 0;

ALTER TABLE [SolicitudesAscenso] 
ADD [FechaAprobacionComision] datetime2 NULL;

ALTER TABLE [SolicitudesAscenso] 
ADD [FechaAprobacionConsejo] datetime2 NULL;

ALTER TABLE [SolicitudesAscenso] 
ADD [ObservacionesComision] nvarchar(max) NULL;

ALTER TABLE [SolicitudesAscenso] 
ADD [ObservacionesConsejo] nvarchar(max) NULL;

-- Comentarios explicativos del reglamento
-- AprobadoPorComision: Estado de aprobación por la Comisión Académica de Escalafón y Promoción (Art. 4)
-- AprobadoPorConsejo: Estado de aprobación por el Honorable Consejo Universitario (Art. 5.1.b)
-- FechaAprobacionComision: Fecha cuando la Comisión aprobó la solicitud
-- FechaAprobacionConsejo: Fecha cuando el Consejo Universitario aprobó la solicitud  
-- ObservacionesComision: Observaciones de la Comisión Académica de Escalafón y Promoción
-- ObservacionesConsejo: Observaciones del Honorable Consejo Universitario

PRINT 'Migración completada: Campos de aprobación según Reglamento UTA agregados exitosamente';
