-- =============================================================================
-- SCRIPT PARA INSERTAR ARTÍCULOS DE PRUEBA
-- =============================================================================
-- Este script inserta artículos de ejemplo para probar el API de artículos
-- =============================================================================

USE SISTEMA_DOCENTES;
GO

-- Verificar docentes existentes
PRINT 'Docentes existentes para asociar artículos:'
SELECT Cedula, Nombre1, Apellido1 FROM Docentes;

-- =============================================================================
-- INSERTAR ARTÍCULOS DE PRUEBA
-- =============================================================================

-- Artículo 1
INSERT INTO Articulos (
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    ArchivoRuta,
    ContenidoHash,
    DocenteCedula
) VALUES (
    '10.1000/example.2023.001',
    'Análisis de Machine Learning en Sistemas Educativos',
    'Revista de Tecnología Educativa',
    2023,
    '',  -- Se llenará cuando se suba un archivo
    '',  -- Se calculará cuando se suba un archivo
    '1234567890'  -- Cambiar por una cédula que exista
);

-- Artículo 2
INSERT INTO Articulos (
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    ArchivoRuta,
    ContenidoHash,
    DocenteCedula
) VALUES (
    '10.1000/example.2022.002',
    'Metodologías Ágiles en el Desarrollo de Software',
    'Revista de Ingeniería de Software',
    2022,
    '',
    '',
    '1234567890'
);

-- Artículo 3
INSERT INTO Articulos (
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    ArchivoRuta,
    ContenidoHash,
    DocenteCedula
) VALUES (
    '10.1000/example.2023.003',
    'Inteligencia Artificial Aplicada a la Educación',
    'International Journal of AI in Education',
    2023,
    '',
    '',
    '0987654321'  -- Cambiar por otra cédula que exista
);

-- Artículo 4
INSERT INTO Articulos (
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    ArchivoRuta,
    ContenidoHash,
    DocenteCedula
) VALUES (
    '10.1000/example.2021.004',
    'Bases de Datos NoSQL para Aplicaciones Web',
    'Database Systems Journal',
    2021,
    '',
    '',
    '0987654321'
);

-- Artículo 5
INSERT INTO Articulos (
    DOI,
    Titulo,
    Revista,
    AnioPublicacion,
    ArchivoRuta,
    ContenidoHash,
    DocenteCedula
) VALUES (
    '10.1000/example.2023.005',
    'Seguridad en Aplicaciones Móviles',
    'Mobile Security Review',
    2023,
    '',
    '',
    '1111111111'  -- Cambiar por otra cédula que exista
);

-- =============================================================================
-- VERIFICAR ARTÍCULOS INSERTADOS
-- =============================================================================

PRINT 'Artículos insertados:';
SELECT 
    a.DOI,
    a.Titulo,
    a.Revista,
    a.AnioPublicacion,
    a.DocenteCedula,
    d.Nombre1 + ' ' + d.Apellido1 AS NombreDocente
FROM Articulos a
INNER JOIN Docentes d ON a.DocenteCedula = d.Cedula
ORDER BY a.AnioPublicacion DESC, a.Titulo;

-- =============================================================================
-- EJEMPLOS DE ASOCIACIÓN DE ARTÍCULOS A SOLICITUDES
-- =============================================================================

-- Para asociar artículos a solicitudes después de ejecutar el script de solicitudes:

-- Ejemplo: Asociar el primer artículo a la primera solicitud
-- INSERT INTO ArticulosPorSolicitud (SolicitudId, ArticuloDOI) 
-- VALUES (
--     '[GUID-DE-SOLICITUD]',  -- Reemplazar con un GUID real de solicitud
--     '10.1000/example.2023.001'
-- );

PRINT 'Para asociar artículos a solicitudes, usar los GUIDs del script anterior';
PRINT 'Ejemplo de query para asociar:';
PRINT 'INSERT INTO ArticulosPorSolicitud (SolicitudId, ArticuloDOI) VALUES ([GUID], [DOI])';

-- =============================================================================
-- CONSULTA ÚTIL PARA VER ASOCIACIONES
-- =============================================================================

SELECT 
    s.Id AS SolicitudId,
    d.Nombre1 + ' ' + d.Apellido1 AS Docente,
    CASE s.Estado 
        WHEN 1 THEN 'Borrador'
        WHEN 2 THEN 'Enviada'
        WHEN 3 THEN 'En Revisión'
        WHEN 4 THEN 'Aprobada'
        WHEN 5 THEN 'Rechazada'
    END AS EstadoSolicitud,
    a.DOI,
    a.Titulo AS TituloArticulo
FROM SolicitudesAscenso s
INNER JOIN Docentes d ON s.DocenteCedula = d.Cedula
LEFT JOIN ArticulosPorSolicitud aps ON s.Id = aps.SolicitudId
LEFT JOIN Articulos a ON aps.ArticuloDOI = a.DOI
ORDER BY s.FechaCreacion DESC;

-- =============================================================================
-- NOTAS IMPORTANTES:
-- =============================================================================
-- 1. Cambiar las cédulas de docentes por las que realmente existan
-- 2. Los DOI deben ser únicos en la tabla Articulos
-- 3. Los campos ArchivoRuta y ContenidoHash se llenarán al subir archivos via API
-- 4. Para probar las asociaciones, usar los GUIDs generados en el script anterior
-- ============================================================================= 