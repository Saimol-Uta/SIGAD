-- Script para insertar rangos académicos en la tabla Rangos
-- Basado en rangos típicos universitarios con requisitos progresivos

USE SigadDB;
GO

-- Verificar si la tabla Rangos tiene datos
IF EXISTS (SELECT 1 FROM Rangos)
BEGIN
    PRINT 'ADVERTENCIA: La tabla Rangos ya contiene datos.';
    PRINT 'Ejecutando SELECT para mostrar datos existentes:';
    SELECT * FROM Rangos ORDER BY Id;
    PRINT '';
    PRINT 'Si desea reemplazar los datos, elimine los registros existentes primero.';
    RETURN;
END

-- Insertar rangos académicos progresivos
PRINT 'Insertando rangos académicos...';

-- Rango 1: Instructor - Nivel de entrada
INSERT INTO Rangos (
    Nombre, 
    ArticulosRequeridos, 
    AniosExperienciaRequeridos, 
    HorasCursoRequeridas, 
    MesesInvestigacionRequeridos, 
    PuntajePromedioEvaluacionesRequerido
) VALUES (
    'Instructor',
    0,          -- Sin artículos requeridos (nivel de entrada)
    0,          -- Sin experiencia previa requerida
    40,         -- 40 horas de cursos/capacitación
    0,          -- Sin investigación requerida
    75.00       -- Puntaje mínimo de evaluaciones 75%
);

-- Rango 2: Asistente - Primer ascenso
INSERT INTO Rangos (
    Nombre, 
    ArticulosRequeridos, 
    AniosExperienciaRequeridos, 
    HorasCursoRequeridas, 
    MesesInvestigacionRequeridos, 
    PuntajePromedioEvaluacionesRequerido
) VALUES (
    'Asistente',
    2,          -- 2 artículos científicos
    2,          -- 2 años de experiencia docente
    80,         -- 80 horas de cursos
    6,          -- 6 meses de investigación
    78.00       -- Puntaje mínimo 78%
);

-- Rango 3: Agregado - Nivel intermedio
INSERT INTO Rangos (
    Nombre, 
    ArticulosRequeridos, 
    AniosExperienciaRequeridos, 
    HorasCursoRequeridas, 
    MesesInvestigacionRequeridos, 
    PuntajePromedioEvaluacionesRequerido
) VALUES (
    'Agregado',
    5,          -- 5 artículos científicos
    5,          -- 5 años de experiencia
    120,        -- 120 horas de formación continua
    12,         -- 12 meses de investigación
    82.00       -- Puntaje mínimo 82%
);

-- Rango 4: Asociado - Nivel avanzado
INSERT INTO Rangos (
    Nombre, 
    ArticulosRequeridos, 
    AniosExperienciaRequeridos, 
    HorasCursoRequeridas, 
    MesesInvestigacionRequeridos, 
    PuntajePromedioEvaluacionesRequerido
) VALUES (
    'Asociado',
    10,         -- 10 artículos científicos
    8,          -- 8 años de experiencia
    160,        -- 160 horas de formación
    24,         -- 24 meses de investigación activa
    85.00       -- Puntaje mínimo 85%
);

-- Rango 5: Titular - Máximo nivel
INSERT INTO Rangos (
    Nombre, 
    ArticulosRequeridos, 
    AniosExperienciaRequeridos, 
    HorasCursoRequeridas, 
    MesesInvestigacionRequeridos, 
    PuntajePromedioEvaluacionesRequerido
) VALUES (
    'Titular',
    20,         -- 20 artículos científicos
    15,         -- 15 años de experiencia
    200,        -- 200 horas de formación
    36,         -- 36 meses de investigación
    88.00       -- Puntaje mínimo 88%
);

PRINT 'Rangos insertados correctamente.';

-- Verificar los datos insertados
PRINT '';
PRINT '=== RANGOS ACADÉMICOS CREADOS ===';
SELECT 
    Id,
    Nombre,
    ArticulosRequeridos as 'Artículos Req.',
    AniosExperienciaRequeridos as 'Años Exp.',
    HorasCursoRequeridas as 'Horas Curso',
    MesesInvestigacionRequeridos as 'Meses Inv.',
    PuntajePromedioEvaluacionesRequerido as 'Puntaje Min.'
FROM Rangos 
ORDER BY Id;

PRINT '';
PRINT '=== RESUMEN DE REQUISITOS POR RANGO ===';
PRINT 'Instructor: Nivel de entrada sin requisitos previos';
PRINT 'Asistente: Primeros pasos en investigación y docencia';
PRINT 'Agregado: Consolidación académica e investigativa';
PRINT 'Asociado: Experticia reconocida y liderazgo';
PRINT 'Titular: Máxima categoría académica';

/*
NOTAS IMPORTANTES:

1. PROGRESIÓN ACADÉMICA:
   - Los requisitos aumentan progresivamente con cada rango
   - Reflejan una carrera académica típica universitaria
   - Balancean docencia, investigación y formación continua

2. ARTÍCULOS CIENTÍFICOS:
   - Instructor: 0 (nivel de entrada)
   - Asistente: 2 (inicio de producción científica)
   - Agregado: 5 (consolidación investigativa)
   - Asociado: 10 (productividad sostenida)
   - Titular: 20 (liderazgo científico)

3. EXPERIENCIA DOCENTE:
   - Aumenta de 0 a 15 años progresivamente
   - Refleja la madurez pedagógica esperada

4. FORMACIÓN CONTINUA:
   - Horas de cursos/capacitación aumentan con el rango
   - Importante para actualización profesional

5. INVESTIGACIÓN:
   - Meses de investigación activa requeridos
   - Crítico para rangos superiores

6. EVALUACIONES:
   - Puntaje promedio mínimo aumenta con el rango
   - Refleja expectativas de excelencia crecientes

7. PARA USAR EN SOLICITUDES:
   - Estos IDs (1-5) corresponden a los usados en Scripts_SolicitudesAscenso_Corregido.sql
   - Los requisitos son realistas y permiten progresión natural
*/ 