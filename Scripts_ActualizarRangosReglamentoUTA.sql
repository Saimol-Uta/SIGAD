-- Script para actualizar los valores de los Rangos según el Reglamento UTA
-- IMPORTANTE: Ejecuta primero Scripts_ConsultarRangosActuales.sql para ver los datos actuales
-- Revisa y ajusta los comandos según los IDs y nombres exactos de tu base de datos

USE [SISTEMA_DOCENTES]  -- Reemplaza con el nombre de tu base de datos
GO

-- INICIO DE TRANSACCIÓN PARA SEGURIDAD
BEGIN TRANSACTION;

-- Backup de seguridad (opcional, descomenta si deseas crear respaldo)
-- SELECT * INTO Rangos_Backup_$(FORMAT(GETDATE(), 'yyyyMMdd_HHmmss')) FROM Rangos;

-- =====================================================
-- ACTUALIZACIÓN DE RANGOS SEGÚN REGLAMENTO UTA
-- =====================================================

-- 1. TITULAR PRINCIPAL 1 (Sin requisitos específicos)
UPDATE Rangos SET
    ArticulosRequeridos = 0,
    AniosExperienciaRequeridos = 0,
    HorasCursoRequeridas = 0,
    MesesInvestigacionRequeridos = 0,
    TesisDirigidasRequeridas = 0,
    PuntajePromedioEvaluacionesRequerido = 0,
    HorasCapacitacionPedagogicaRequeridas = 0,
    HorasCapacitacionImpartidaRequeridas = 0,
    PublicacionesIdiomaExtranjeroRequeridas = 0,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 0,
    PermiteCoordinacionProyectos = 0
WHERE Nombre LIKE '%Titular Principal 1%' OR Nombre LIKE '%Principal 1%';

-- 2. TITULAR PRINCIPAL 2
UPDATE Rangos SET
    ArticulosRequeridos = 2,
    AniosExperienciaRequeridos = 2,
    HorasCursoRequeridas = 80,
    MesesInvestigacionRequeridos = 6,
    TesisDirigidasRequeridas = 1,
    PuntajePromedioEvaluacionesRequerido = 80,
    HorasCapacitacionPedagogicaRequeridas = 40,
    HorasCapacitacionImpartidaRequeridas = 20,
    PublicacionesIdiomaExtranjeroRequeridas = 1,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 1,
    PermiteCoordinacionProyectos = 1
WHERE Nombre LIKE '%Titular Principal 2%' OR Nombre LIKE '%Principal 2%';

-- 3. TITULAR PRINCIPAL 3
UPDATE Rangos SET
    ArticulosRequeridos = 4,
    AniosExperienciaRequeridos = 4,
    HorasCursoRequeridas = 120,
    MesesInvestigacionRequeridos = 12,
    TesisDirigidasRequeridas = 2,
    PuntajePromedioEvaluacionesRequerido = 85,
    HorasCapacitacionPedagogicaRequeridas = 60,
    HorasCapacitacionImpartidaRequeridas = 40,
    PublicacionesIdiomaExtranjeroRequeridas = 2,
    ProyectosInternacionalesRequeridos = 1,
    RequiereArticuloEnGradoActual = 1,
    PermiteCoordinacionProyectos = 1
WHERE Nombre LIKE '%Titular Principal 3%' OR Nombre LIKE '%Principal 3%';

-- 4. TITULAR AGREGADO 1
UPDATE Rangos SET
    ArticulosRequeridos = 1,
    AniosExperienciaRequeridos = 1,
    HorasCursoRequeridas = 40,
    MesesInvestigacionRequeridos = 3,
    TesisDirigidasRequeridas = 0,
    PuntajePromedioEvaluacionesRequerido = 75,
    HorasCapacitacionPedagogicaRequeridas = 20,
    HorasCapacitacionImpartidaRequeridas = 10,
    PublicacionesIdiomaExtranjeroRequeridas = 0,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 1,
    PermiteCoordinacionProyectos = 0
WHERE Nombre LIKE '%Titular Agregado 1%' OR Nombre LIKE '%Agregado 1%';

-- 5. TITULAR AGREGADO 2
UPDATE Rangos SET
    ArticulosRequeridos = 2,
    AniosExperienciaRequeridos = 2,
    HorasCursoRequeridas = 60,
    MesesInvestigacionRequeridos = 6,
    TesisDirigidasRequeridas = 1,
    PuntajePromedioEvaluacionesRequerido = 78,
    HorasCapacitacionPedagogicaRequeridas = 30,
    HorasCapacitacionImpartidaRequeridas = 15,
    PublicacionesIdiomaExtranjeroRequeridas = 0,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 1,
    PermiteCoordinacionProyectos = 1
WHERE Nombre LIKE '%Titular Agregado 2%' OR Nombre LIKE '%Agregado 2%';

-- 6. TITULAR AGREGADO 3
UPDATE Rangos SET
    ArticulosRequeridos = 3,
    AniosExperienciaRequeridos = 3,
    HorasCursoRequeridas = 80,
    MesesInvestigacionRequeridos = 9,
    TesisDirigidasRequeridas = 1,
    PuntajePromedioEvaluacionesRequerido = 80,
    HorasCapacitacionPedagogicaRequeridas = 40,
    HorasCapacitacionImpartidaRequeridas = 20,
    PublicacionesIdiomaExtranjeroRequeridas = 1,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 1,
    PermiteCoordinacionProyectos = 1
WHERE Nombre LIKE '%Titular Agregado 3%' OR Nombre LIKE '%Agregado 3%';

-- 7. TITULAR AUXILIAR 1
UPDATE Rangos SET
    ArticulosRequeridos = 0,
    AniosExperienciaRequeridos = 0,
    HorasCursoRequeridas = 20,
    MesesInvestigacionRequeridos = 0,
    TesisDirigidasRequeridas = 0,
    PuntajePromedioEvaluacionesRequerido = 70,
    HorasCapacitacionPedagogicaRequeridas = 10,
    HorasCapacitacionImpartidaRequeridas = 0,
    PublicacionesIdiomaExtranjeroRequeridas = 0,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 0,
    PermiteCoordinacionProyectos = 0
WHERE Nombre LIKE '%Titular Auxiliar 1%' OR Nombre LIKE '%Auxiliar 1%';

-- 8. TITULAR AUXILIAR 2
UPDATE Rangos SET
    ArticulosRequeridos = 1,
    AniosExperienciaRequeridos = 1,
    HorasCursoRequeridas = 30,
    MesesInvestigacionRequeridos = 2,
    TesisDirigidasRequeridas = 0,
    PuntajePromedioEvaluacionesRequerido = 72,
    HorasCapacitacionPedagogicaRequeridas = 15,
    HorasCapacitacionImpartidaRequeridas = 5,
    PublicacionesIdiomaExtranjeroRequeridas = 0,
    ProyectosInternacionalesRequeridos = 0,
    RequiereArticuloEnGradoActual = 0,
    PermiteCoordinacionProyectos = 0
WHERE Nombre LIKE '%Titular Auxiliar 2%' OR Nombre LIKE '%Auxiliar 2%';

-- Mostrar resumen de cambios
PRINT 'Actualizando rangos según Reglamento UTA...';

-- Verificar los cambios realizados
SELECT 
    Nombre,
    ArticulosRequeridos,
    AniosExperienciaRequeridos,
    HorasCursoRequeridas,
    MesesInvestigacionRequeridos,
    TesisDirigidasRequeridas,
    PuntajePromedioEvaluacionesRequerido,
    HorasCapacitacionPedagogicaRequeridas,
    HorasCapacitacionImpartidaRequeridas,
    PublicacionesIdiomaExtranjeroRequeridas,
    ProyectosInternacionalesRequeridos,
    RequiereArticuloEnGradoActual,
    PermiteCoordinacionProyectos
FROM Rangos
ORDER BY Id;

-- Si todo se ve correcto, ejecuta: COMMIT TRANSACTION
-- Si algo está mal, ejecuta: ROLLBACK TRANSACTION

PRINT 'Actualización completada. Revisa los resultados y ejecuta COMMIT o ROLLBACK según corresponda.';

-- DESCOMENTA LA LÍNEA SIGUIENTE SOLO SI ESTÁS SEGURO DE LOS CAMBIOS
-- COMMIT TRANSACTION;
