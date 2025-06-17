# Pruebas del API de Artículos - SIGAD

Este documento contiene ejemplos de peticiones HTTP para probar completamente el API de artículos.

## Configuración previa

1. Ejecutar el script `Scripts_SolicitudesAscenso_Prueba.sql` para insertar solicitudes
2. Ejecutar el script `Scripts_ArticulosPrueba.sql` para insertar artículos (opcional)
3. Iniciar el proyecto: `dotnet run` desde SIGAD.WebAPI
4. La API estará disponible en: `https://localhost:7087` (verificar en launchSettings.json)

## 1. Obtener todos los artículos

```http
GET https://localhost:7087/api/articulos
Accept: application/json
```

## 2. Obtener un artículo específico por DOI

```http
GET https://localhost:7087/api/articulos/10.1000%2Fexample.2023.001
Accept: application/json
```

**Nota:** El DOI debe estar URL-encoded (%2F en lugar de /)

## 3. Obtener artículos de un docente específico

```http
GET https://localhost:7087/api/articulos/docente/1234567890
Accept: application/json
```

## 4. Obtener artículos de una solicitud específica

```http
GET https://localhost:7087/api/articulos/solicitud/[GUID-DE-SOLICITUD]
Accept: application/json
```

## 5. Crear un nuevo artículo (sin archivo)

```http
POST https://localhost:7087/api/articulos
Content-Type: application/x-www-form-urlencoded

DOI=10.1000%2Fnuevo.2024.001&Titulo=Nuevo%20Art%C3%ADculo%20de%20Prueba&Revista=Revista%20de%20Pruebas&AnioPublicacion=2024&DocenteCedula=1234567890
```

## 6. Crear un artículo con archivo

```http
POST https://localhost:7087/api/articulos
Content-Type: multipart/form-data

--boundary123
Content-Disposition: form-data; name="DOI"

10.1000/conarchivo.2024.001
--boundary123
Content-Disposition: form-data; name="Titulo"

Artículo con Archivo de Prueba
--boundary123
Content-Disposition: form-data; name="Revista"

Revista de Pruebas con Archivo
--boundary123
Content-Disposition: form-data; name="AnioPublicacion"

2024
--boundary123
Content-Disposition: form-data; name="DocenteCedula"

1234567890
--boundary123
Content-Disposition: form-data; name="archivo"; filename="articulo.pdf"
Content-Type: application/pdf

[CONTENIDO DEL ARCHIVO PDF]
--boundary123--
```

## 7. Crear artículo y asociarlo a una solicitud

```http
POST https://localhost:7087/api/articulos
Content-Type: application/x-www-form-urlencoded

DOI=10.1000%2Fconasociacion.2024.001&Titulo=Art%C3%ADculo%20Asociado&Revista=Revista%20Asociada&AnioPublicacion=2024&DocenteCedula=1234567890&SolicitudId=[GUID-DE-SOLICITUD]
```

## 8. Actualizar un artículo existente

```http
PUT https://localhost:7087/api/articulos/10.1000%2Fexample.2023.001
Content-Type: application/x-www-form-urlencoded

Titulo=T%C3%ADtulo%20Actualizado&Revista=Revista%20Actualizada&AnioPublicacion=2024
```

## 9. Eliminar un artículo

```http
DELETE https://localhost:7087/api/articulos/10.1000%2Fexample.2023.001
```

## 10. Asociar artículo existente a solicitud

```http
POST https://localhost:7087/api/articulos/asociar-solicitud
Content-Type: application/json

{
  "SolicitudId": "[GUID-DE-SOLICITUD]",
  "ArticuloDOI": "10.1000/example.2022.002"
}
```

## 11. Desasociar artículo de solicitud

```http
DELETE https://localhost:7087/api/articulos/desasociar-solicitud/[GUID-DE-SOLICITUD]/10.1000%2Fexample.2022.002
```

## 12. Descargar archivo de un artículo

```http
GET https://localhost:7087/api/articulos/10.1000%2Fconarchivo.2024.001/archivo
Accept: application/octet-stream
```

## 13. Exportar todos los artículos en JSON

```http
GET https://localhost:7087/api/articulos/exportar?formato=json
Accept: application/json
```

## 14. Exportar todos los artículos en CSV

```http
GET https://localhost:7087/api/articulos/exportar?formato=csv
Accept: text/csv
```

## Respuestas esperadas

### Respuesta exitosa (200/201)
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {
    "doi": "10.1000/example.2023.001",
    "titulo": "Análisis de Machine Learning en Sistemas Educativos",
    "revista": "Revista de Tecnología Educativa",
    "anioPublicacion": 2023,
    "archivoRuta": "",
    "docenteCedula": "1234567890",
    "docenteNombreCompleto": "Juan Pérez"
  }
}
```

### Respuesta de error (400/404/500)
```json
{
  "success": false,
  "message": "Descripción del error",
  "error": "Detalles técnicos del error"
}
```

## Notas importantes

1. **URL Encoding**: Los DOI contienen caracteres especiales que deben ser URL-encoded
2. **GUIDs**: Reemplazar `[GUID-DE-SOLICITUD]` con GUIDs reales obtenidos del script SQL
3. **Cédulas**: Usar cédulas de docentes que realmente existan en la base de datos
4. **Archivos**: Para pruebas con archivos, usar herramientas como Postman o Thunder Client
5. **Content-Type**: Prestar atención al Content-Type correcto para cada tipo de petición

## Herramientas recomendadas para pruebas

- **Postman**: Para pruebas completas con interfaz gráfica
- **Thunder Client** (VS Code): Plugin ligero para VS Code
- **curl**: Para pruebas desde línea de comandos
- **Swagger UI**: Disponible en `https://localhost:7087/swagger` cuando el proyecto esté corriendo 