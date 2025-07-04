-- Script de verificación posterior a la actualización de Rangos
-- Ejecuta este script después de aplicar Scripts_ActualizarRangosReglamentoUTA.sql

USE [SIGAD_DB]  -- Reemplaza con el nombre de tu base de datos
GO

-- =====================================================
-- VERIFICACIÓN DE VALORES ACTUALIZADOS
-- =====================================================

PRINT 'VERIFICACIÓN DE RANGOS ACTUALIZADOS SEGÚN REGLAMENTO UTA';
PRINT '=======================================================';

-- Verificar todos los rangos con formato legible
SELECT 
    ROW_NUMBER() OVER (ORDER BY Id) as Orden,
    Nombre,
    CONCAT('Art: ', ArticulosRequeridos, 
           ' | Exp: ', AniosExperienciaRequeridos, ' años',
           ' | Cursos: ', HorasCursoRequeridas, 'h',
           ' | Inv: ', MesesInvestigacionRequeridos, ' meses') as Requisitos_Principales,
    CONCAT('Tesis: ', TesisDirigidasRequeridas,
           ' | Eval: ', PuntajePromedioEvaluacionesRequerido, '%',
           ' | Cap.Ped: ', HorasCapacitacionPedagogicaRequeridas, 'h',
           ' | Cap.Imp: ', HorasCapacitacionImpartidaRequeridas, 'h') as Requisitos_Adicionales,
    CONCAT('Pub.Ext: ', PublicacionesIdiomaExtranjeroRequeridas,
           ' | Proy.Int: ', ProyectosInternacionalesRequeridos,
           ' | Art.Grado: ', CASE WHEN RequiereArticuloEnGradoActual = 1 THEN 'Sí' ELSE 'No' END,
           ' | Coord: ', CASE WHEN PermiteCoordinacionProyectos = 1 THEN 'Sí' ELSE 'No' END) as Requisitos_Especiales
FROM Rangos
ORDER BY Id;

PRINT '';
PRINT 'VALIDACIÓN DE RANGOS ESPECÍFICOS:';
PRINT '=================================';

-- Validar rangos específicos mencionados en el reglamento
SELECT 'TITULAR PRINCIPAL 2' as Rango, * FROM Rangos WHERE Nombre LIKE '%Principal 2%'
UNION ALL
SELECT 'TITULAR PRINCIPAL 3' as Rango, * FROM Rangos WHERE Nombre LIKE '%Principal 3%'
UNION ALL
SELECT 'TITULAR AGREGADO 3' as Rango, * FROM Rangos WHERE Nombre LIKE '%Agregado 3%';

PRINT '';
PRINT 'RESUMEN DE REQUISITOS POR RANGO:';
PRINT '=================================';

-- Resumen consolidado
SELECT 
    CASE 
        WHEN Nombre LIKE '%Principal 1%' THEN 'P1 - Sin requisitos'
        WHEN Nombre LIKE '%Principal 2%' THEN 'P2 - Intermedio'
        WHEN Nombre LIKE '%Principal 3%' THEN 'P3 - Avanzado'
        WHEN Nombre LIKE '%Agregado 1%' THEN 'A1 - Básico'
        WHEN Nombre LIKE '%Agregado 2%' THEN 'A2 - Intermedio'
        WHEN Nombre LIKE '%Agregado 3%' THEN 'A3 - Avanzado'
        WHEN Nombre LIKE '%Auxiliar 1%' THEN 'X1 - Inicial'
        WHEN Nombre LIKE '%Auxiliar 2%' THEN 'X2 - Básico'
        ELSE 'OTRO'
    END as Categoria,
    Nombre,
    ArticulosRequeridos as Art,
    AniosExperienciaRequeridos as Exp,
    HorasCursoRequeridas as Cursos,
    TesisDirigidasRequeridas as Tesis,
    PublicacionesIdiomaExtranjeroRequeridas as PubExt,
    ProyectosInternacionalesRequeridos as PrInt
FROM Rangos
ORDER BY 
    CASE 
        WHEN Nombre LIKE '%Principal%' THEN 1
        WHEN Nombre LIKE '%Agregado%' THEN 2
        WHEN Nombre LIKE '%Auxiliar%' THEN 3
        ELSE 4
    END,
    Nombre;

PRINT '';
PRINT 'VERIFICACIÓN DE INTEGRIDAD:';
PRINT '============================';

-- Verificar que no hay valores negativos
SELECT 'Valores negativos encontrados' as Problema, COUNT(*) as Cantidad
FROM Rangos 
WHERE ArticulosRequeridos < 0 
   OR AniosExperienciaRequeridos < 0 
   OR HorasCursoRequeridas < 0 
   OR MesesInvestigacionRequeridos < 0 
   OR TesisDirigidasRequeridas < 0
   OR PuntajePromedioEvaluacionesRequerido < 0
   OR HorasCapacitacionPedagogicaRequeridas < 0
   OR HorasCapacitacionImpartidaRequeridas < 0
   OR PublicacionesIdiomaExtranjeroRequeridas < 0
   OR ProyectosInternacionalesRequeridos < 0;

-- Verificar campos booleanos
SELECT 'Campos booleanos incorrectos' as Problema, COUNT(*) as Cantidad
FROM Rangos 
WHERE RequiereArticuloEnGradoActual NOT IN (0, 1) 
   OR PermiteCoordinacionProyectos NOT IN (0, 1);

PRINT 'Verificación completada. Revisa los resultados arriba.';
