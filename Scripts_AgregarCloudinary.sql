-- ============================================
-- Script para agregar columnas UrlCloudinary a todas las entidades
-- Sistema de Respaldo Dual: Local + Cloudinary
-- Fecha: 02/07/2025
-- ============================================

USE SISTEMA_DOCENTES;
GO

PRINT '==============================================';
PRINT 'INICIANDO MODIFICACIONES PARA CLOUDINARY';
PRINT '==============================================';

-- ============================================
-- AGREGAR COLUMNA UrlCloudinary A TODAS LAS ENTIDADES
-- ============================================

-- 1. Tabla Cursos
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Cursos') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE Cursos 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla Cursos';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla Cursos';
END

-- 2. Tabla Articulos
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Articulos') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE Articulos 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla Articulos';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla Articulos';
END

-- 3. Tabla EvaluacionesDocentes
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EvaluacionesDocentes') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE EvaluacionesDocentes 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla EvaluacionesDocentes';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla EvaluacionesDocentes';
END

-- 4. Tabla ExperienciasLaborales
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExperienciasLaborales') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE ExperienciasLaborales 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla ExperienciasLaborales';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla ExperienciasLaborales';
END

-- 5. Tabla Investigaciones
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Investigaciones') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE Investigaciones 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla Investigaciones';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla Investigaciones';
END

-- 6. Tabla TesisDirigidas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TesisDirigidas') AND name = 'UrlCloudinary')
BEGIN
    ALTER TABLE TesisDirigidas 
    ADD UrlCloudinary NVARCHAR(500) NULL;
    PRINT '✓ Columna UrlCloudinary agregada a tabla TesisDirigidas';
END
ELSE
BEGIN
    PRINT '⚠ Columna UrlCloudinary ya existe en tabla TesisDirigidas';
END

-- ============================================
-- VERIFICACIÓN DE COLUMNAS AGREGADAS
-- ============================================

PRINT '==============================================';
PRINT 'VERIFICANDO COLUMNAS AGREGADAS:';
PRINT '==============================================';

-- Verificar todas las tablas
SELECT 
    'Cursos' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Cursos') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado
UNION ALL
SELECT 
    'Articulos' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Articulos') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado
UNION ALL
SELECT 
    'EvaluacionesDocentes' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EvaluacionesDocentes') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado
UNION ALL
SELECT 
    'ExperienciasLaborales' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ExperienciasLaborales') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado
UNION ALL
SELECT 
    'Investigaciones' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Investigaciones') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado
UNION ALL
SELECT 
    'TesisDirigidas' as Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TesisDirigidas') AND name = 'UrlCloudinary')
        THEN '✓ UrlCloudinary PRESENTE'
        ELSE '❌ UrlCloudinary FALTANTE'
    END as Estado;

PRINT '==============================================';
PRINT 'MODIFICACIONES COMPLETADAS EXITOSAMENTE';
PRINT 'Sistema de Respaldo Dual (Local + Cloudinary) LISTO';
PRINT '==============================================';

-- ============================================
-- ESTADÍSTICAS FINALES
-- ============================================

PRINT 'ESTADÍSTICAS DE REGISTROS POR TABLA:';

SELECT 'Cursos' as Tabla, COUNT(*) as TotalRegistros FROM Cursos
UNION ALL
SELECT 'Articulos' as Tabla, COUNT(*) as TotalRegistros FROM Articulos
UNION ALL
SELECT 'EvaluacionesDocentes' as Tabla, COUNT(*) as TotalRegistros FROM EvaluacionesDocentes
UNION ALL
SELECT 'ExperienciasLaborales' as Tabla, COUNT(*) as TotalRegistros FROM ExperienciasLaborales
UNION ALL
SELECT 'Investigaciones' as Tabla, COUNT(*) as TotalRegistros FROM Investigaciones
UNION ALL
SELECT 'TesisDirigidas' as Tabla, COUNT(*) as TotalRegistros FROM TesisDirigidas;

PRINT '==============================================';
PRINT 'NOTA: Ahora puede proceder con:';
PRINT '1. Instalar CloudinaryDotNet NuGet package';
PRINT '2. Configurar credenciales de Cloudinary en appsettings.json';
PRINT '3. Implementar FileStorageService';
PRINT '4. Actualizar entidades del dominio';
PRINT '==============================================';
