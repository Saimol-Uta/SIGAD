-- ============================================
-- Script para modificar bases externas y agregar campos para PDFs
-- Fecha: 01/07/2025
-- ============================================

-- MODIFICAR TABLA TesisDirigidas en base externa SUT
USE SUT;

-- Agregar columna para guardar PDF en binario
ALTER TABLE TesisDirigidas 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA TesisDirigidas en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario
ALTER TABLE TesisDirigidas 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SUT
-- ============================================

USE SUT;

-- Función para generar contenido PDF simulado
-- En un escenario real, aquí cargarías PDFs reales
DECLARE @PdfSimulado VARBINARY(MAX);

-- Crear un "PDF" simulado (en realidad solo texto para pruebas)
-- En producción aquí cargarías archivos reales
SET @PdfSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Tesis Dirigida SUT - Documento PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE TesisDirigidas 
SET 
    PdfDocumento = @PdfSimulado
WHERE Id IN (1, 2, 3, 4); -- Actualizar los registros existentes

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE TesisDirigidas 
SET 
    PdfDocumento = @PdfSimulado
WHERE Id IN (1, 2, 3, 4); -- Actualizar los registros existentes

-- ============================================
-- VALIDACIONES FINALES
-- ============================================

-- Verificar SUT
USE SUT;
SELECT 
    'SUT' as BaseDatos,
    Id,
    Estado,
    FechaInicio,
    FechaFin,
    Institucion,
    CertificacionRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM TesisDirigidas;

-- Verificar SGTH
USE SGTH;
SELECT 
    'SGTH' as BaseDatos,
    Id,
    Estado,
    FechaInicio,
    FechaFin,
    Institucion,
    CertificacionRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM TesisDirigidas;

-- Mostrar estadísticas de los PDFs
USE SUT;
SELECT 
    'SUT' as BaseDatos,
    COUNT(*) as TotalTesis,
    COUNT(PdfDocumento) as TesisConPDF
FROM TesisDirigidas;

USE SGTH;
SELECT 
    'SGTH' as BaseDatos,
    COUNT(*) as TotalTesis,
    COUNT(PdfDocumento) as TesisConPDF
FROM TesisDirigidas;

PRINT 'Modificación de bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.TesisDirigidas y SGTH.TesisDirigidas';

-- ============================================
-- MODIFICAR TABLA Cursos en base externa SUT
-- ============================================
USE SUT;

-- Agregar columna para guardar PDF en binario en tabla Cursos
ALTER TABLE Cursos 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA Cursos en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario en tabla Cursos
ALTER TABLE Cursos 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN CURSOS
-- ============================================

USE SUT;

-- PDF simulado para cursos
DECLARE @PdfCursoSimulado VARBINARY(MAX);
SET @PdfCursoSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Certificado de Curso SUT - PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE Cursos 
SET 
    PdfDocumento = @PdfCursoSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE Cursos 
SET 
    PdfDocumento = @PdfCursoSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- VALIDACIONES FINALES PARA CURSOS
-- ============================================

-- Verificar SUT Cursos
USE SUT;
SELECT 
    'SUT_Cursos' as BaseDatos,
    Id,
    Nombre,
    Organizacion,
    NumeroHoras,
    FechaFinalizacion,
    CertificadoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Cursos;

-- Verificar SGTH Cursos
USE SGTH;
SELECT 
    'SGTH_Cursos' as BaseDatos,
    Id,
    Nombre,
    Organizacion,
    NumeroHoras,
    FechaFinalizacion,
    CertificadoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Cursos;

-- Estadísticas de PDFs en Cursos
USE SUT;
SELECT 
    'SUT_Cursos' as BaseDatos,
    COUNT(*) as TotalCursos,
    COUNT(PdfDocumento) as CursosConPDF
FROM Cursos;

USE SGTH;
SELECT 
    'SGTH_Cursos' as BaseDatos,
    COUNT(*) as TotalCursos,
    COUNT(PdfDocumento) as CursosConPDF
FROM Cursos;

PRINT 'Modificación de tabla Cursos en bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.Cursos y SGTH.Cursos';

-- ============================================
-- MODIFICAR TABLA Evaluaciones en base externa SUT
-- ============================================
USE SUT;

-- Agregar columna para guardar PDF en binario en tabla Evaluaciones
ALTER TABLE Evaluaciones 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA Evaluaciones en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario en tabla Evaluaciones
ALTER TABLE Evaluaciones 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN EVALUACIONES
-- ============================================

USE SUT;

-- PDF simulado para evaluaciones
DECLARE @PdfEvaluacionSimulado VARBINARY(MAX);
SET @PdfEvaluacionSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Evaluacion Docente SUT - PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE Evaluaciones 
SET 
    PdfDocumento = @PdfEvaluacionSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE Evaluaciones 
SET 
    PdfDocumento = @PdfEvaluacionSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- VALIDACIONES FINALES PARA EVALUACIONES
-- ============================================

-- Verificar SUT Evaluaciones
USE SUT;
SELECT 
    'SUT_Evaluaciones' as BaseDatos,
    Id,
    TipoEvaluacion,
    FechaEvaluacion,
    Periodo,
    Puntuacion,
    InformeRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Evaluaciones;

-- Verificar SGTH Evaluaciones
USE SGTH;
SELECT 
    'SGTH_Evaluaciones' as BaseDatos,
    Id,
    TipoEvaluacion,
    FechaEvaluacion,
    Periodo,
    Puntuacion,
    InformeRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Evaluaciones;

-- Estadísticas de PDFs en Evaluaciones
USE SUT;
SELECT 
    'SUT_Evaluaciones' as BaseDatos,
    COUNT(*) as TotalEvaluaciones,
    COUNT(PdfDocumento) as EvaluacionesConPDF
FROM Evaluaciones;

USE SGTH;
SELECT 
    'SGTH_Evaluaciones' as BaseDatos,
    COUNT(*) as TotalEvaluaciones,
    COUNT(PdfDocumento) as EvaluacionesConPDF
FROM Evaluaciones;

PRINT 'Modificación de tabla Evaluaciones en bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.Evaluaciones y SGTH.Evaluaciones';

-- ============================================
-- MODIFICAR TABLA Investigaciones en base externa SUT
-- ============================================
USE SUT;

-- Agregar columna para guardar PDF en binario en tabla Investigaciones
ALTER TABLE Investigaciones 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA Investigaciones en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario en tabla Investigaciones
ALTER TABLE Investigaciones 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN INVESTIGACIONES
-- ============================================

USE SUT;

-- PDF simulado para investigaciones
DECLARE @PdfInvestigacionSimulado VARBINARY(MAX);
SET @PdfInvestigacionSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Informe de Investigacion SUT - PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE Investigaciones 
SET 
    PdfDocumento = @PdfInvestigacionSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE Investigaciones 
SET 
    PdfDocumento = @PdfInvestigacionSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- VALIDACIONES FINALES PARA INVESTIGACIONES
-- ============================================

-- Verificar SUT Investigaciones
USE SUT;
SELECT 
    'SUT_Investigaciones' as BaseDatos,
    Id,
    Titulo,
    FechaInicio,
    FechaFinalizacion,
    RolEnInvestigacion,
    MesesDeInvestigacion,
    InformeRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Investigaciones;

-- Verificar SGTH Investigaciones
USE SGTH;
SELECT 
    'SGTH_Investigaciones' as BaseDatos,
    Id,
    Titulo,
    FechaInicio,
    FechaFinalizacion,
    RolEnInvestigacion,
    MesesDeInvestigacion,
    InformeRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Investigaciones;

-- Estadísticas de PDFs en Investigaciones
USE SUT;
SELECT 
    'SUT_Investigaciones' as BaseDatos,
    COUNT(*) as TotalInvestigaciones,
    COUNT(PdfDocumento) as InvestigacionesConPDF
FROM Investigaciones;

USE SGTH;
SELECT 
    'SGTH_Investigaciones' as BaseDatos,
    COUNT(*) as TotalInvestigaciones,
    COUNT(PdfDocumento) as InvestigacionesConPDF
FROM Investigaciones;

PRINT 'Modificación de tabla Investigaciones en bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.Investigaciones y SGTH.Investigaciones';

-- ============================================
-- MODIFICAR TABLA Articulos en base externa SUT
-- ============================================
USE SUT;

-- Agregar columna para guardar PDF en binario en tabla Articulos
ALTER TABLE Articulos 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA Articulos en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario en tabla Articulos
ALTER TABLE Articulos 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN ARTICULOS
-- ============================================

USE SUT;

-- PDF simulado para artículos
DECLARE @PdfArticuloSimulado VARBINARY(MAX);
SET @PdfArticuloSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Articulo Cientifico SUT - PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE Articulos 
SET 
    PdfDocumento = @PdfArticuloSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE Articulos 
SET 
    PdfDocumento = @PdfArticuloSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- VALIDACIONES FINALES PARA ARTICULOS
-- ============================================

-- Verificar SUT Artículos
USE SUT;
SELECT 
    'SUT_Articulos' as BaseDatos,
    Id,
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    IdiomaPublicacion,
    ArchivoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Articulos;

-- Verificar SGTH Artículos
USE SGTH;
SELECT 
    'SGTH_Articulos' as BaseDatos,
    Id,
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    IdiomaPublicacion,
    ArchivoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Articulos;

-- Estadísticas de PDFs en Artículos
USE SUT;
SELECT 
    'SUT_Articulos' as BaseDatos,
    COUNT(*) as TotalArticulos,
    COUNT(PdfDocumento) as ArticulosConPDF
FROM Articulos;

USE SGTH;
SELECT 
    'SGTH_Articulos' as BaseDatos,
    COUNT(*) as TotalArticulos,
    COUNT(PdfDocumento) as ArticulosConPDF
FROM Articulos;

PRINT 'Modificación de tabla Articulos en bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.Articulos y SGTH.Articulos';

-- ============================================
-- MODIFICAR TABLA Experiencias en base externa SUT
-- ============================================
USE SUT;

-- Agregar columna para guardar PDF en binario en tabla Experiencias
ALTER TABLE Experiencias 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- MODIFICAR TABLA Experiencias en base externa SGTH
-- ============================================
USE SGTH;

-- Agregar columna para guardar PDF en binario en tabla Experiencias
ALTER TABLE Experiencias 
ADD PdfDocumento VARBINARY(MAX) NULL;

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN EXPERIENCIAS
-- ============================================

USE SUT;

-- PDF simulado para experiencias laborales
DECLARE @PdfExperienciaSimulado VARBINARY(MAX);
SET @PdfExperienciaSimulado = CONVERT(VARBINARY(MAX), 
    '%PDF-1.4
    1 0 obj
    <<
    /Type /Catalog
    /Pages 2 0 R
    >>
    endobj
    
    2 0 obj
    <<
    /Type /Pages
    /Kids [3 0 R]
    /Count 1
    >>
    endobj
    
    3 0 obj
    <<
    /Type /Page
    /Parent 2 0 R
    /MediaBox [0 0 612 792]
    /Contents 4 0 R
    >>
    endobj
    
    4 0 obj
    <<
    /Length 44
    >>
    stream
    BT
    /F1 12 Tf
    100 700 Td
    (Certificado Experiencia Laboral SUT - PDF) Tj
    ET
    endstream
    endobj
    
    xref
    0 5
    0000000000 65535 f 
    0000000009 00000 n 
    0000000058 00000 n 
    0000000115 00000 n 
    0000000207 00000 n 
    trailer
    <<
    /Size 5
    /Root 1 0 R
    >>
    startxref
    309
    %%EOF');

-- Actualizar registros existentes con PDFs simulados en SUT
UPDATE Experiencias 
SET 
    PdfDocumento = @PdfExperienciaSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- INSERTAR DATOS DE PRUEBA CON PDFs SIMULADOS EN SGTH
-- ============================================

USE SGTH;

-- Actualizar registros existentes con PDFs simulados en SGTH
UPDATE Experiencias 
SET 
    PdfDocumento = @PdfExperienciaSimulado
WHERE Id IN (1, 2, 3, 4);

-- ============================================
-- VALIDACIONES FINALES PARA EXPERIENCIAS
-- ============================================

-- Verificar SUT Experiencias
USE SUT;
SELECT 
    'SUT_Experiencias' as BaseDatos,
    Id,
    Organizacion,
    Cargo,
    FechaInicio,
    FechaFin,
    CertificadoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Experiencias;

-- Verificar SGTH Experiencias
USE SGTH;
SELECT 
    'SGTH_Experiencias' as BaseDatos,
    Id,
    Organizacion,
    Cargo,
    FechaInicio,
    FechaFin,
    CertificadoRuta,
    CASE 
        WHEN PdfDocumento IS NOT NULL THEN 'PDF Presente'
        ELSE 'Sin PDF'
    END as EstadoPDF
FROM Experiencias;

-- Estadísticas de PDFs en Experiencias
USE SUT;
SELECT 
    'SUT_Experiencias' as BaseDatos,
    COUNT(*) as TotalExperiencias,
    COUNT(PdfDocumento) as ExperienciasConPDF
FROM Experiencias;

USE SGTH;
SELECT 
    'SGTH_Experiencias' as BaseDatos,
    COUNT(*) as TotalExperiencias,
    COUNT(PdfDocumento) as ExperienciasConPDF
FROM Experiencias;

PRINT 'Modificación de tabla Experiencias en bases externas completada exitosamente';
PRINT 'Campo PdfDocumento agregado a SUT.Experiencias y SGTH.Experiencias';

-- ============================================
-- RESUMEN FINAL DE TODAS LAS MODIFICACIONES
-- ============================================

PRINT '==============================================';
PRINT 'RESUMEN FINAL DE MODIFICACIONES COMPLETADAS:';
PRINT '==============================================';
PRINT '✓ Campo PdfDocumento agregado a SUT.TesisDirigidas y SGTH.TesisDirigidas';
PRINT '✓ Campo PdfDocumento agregado a SUT.Cursos y SGTH.Cursos';
PRINT '✓ Campo PdfDocumento agregado a SUT.Evaluaciones y SGTH.Evaluaciones';
PRINT '✓ Campo PdfDocumento agregado a SUT.Investigaciones y SGTH.Investigaciones';
PRINT '✓ Campo PdfDocumento agregado a SUT.Articulos y SGTH.Articulos';
PRINT '✓ Campo PdfDocumento agregado a SUT.Experiencias y SGTH.Experiencias';
PRINT '✓ Datos de prueba con PDFs simulados insertados en todas las entidades';
PRINT '✓ Validaciones y estadísticas ejecutadas para verificar integridad';
PRINT '==============================================';
PRINT 'Las bases externas están listas para la importación unificada de PDFs';
