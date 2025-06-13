# API de Cursos - Documentación y Ejemplos de Prueba

## Descripción General
API completo para gestión de cursos académicos con asociación automática a solicitudes de ascenso. Los cursos se crean directamente vinculados a una solicitud, siguiendo el flujo natural del proceso académico.

## Endpoints Disponibles

### 1. Obtener Todos los Cursos
```http
GET /api/Cursos
```

**Respuesta de ejemplo:**
```json
[
  {
    "id": 1,
    "nombre": "Metodologías de Investigación",
    "nombreOrganizacion": "Universidad Nacional",
    "tipoOrganizacion": "Universidad",
    "numeroHoras": 40,
    "fechaFinalizacion": "2024-03-15",
    "nombreDocente": "Juan Pérez",
    "docenteCedula": "1234567890",
    "certificadoRuta": "uploads/cursos/guid.pdf",
    "contenidoHash": "ABC123...",
    "organizacionId": 1
  }
]
```

### 2. Crear Curso (Con Asociación Automática)
```http
POST /api/Cursos
Content-Type: multipart/form-data
```

**Parámetros de formulario:**
```
nombre: "Estadística Avanzada"
organizacionId: 1
numeroHoras: 60
fechaFinalizacion: "2024-06-30"
docenteCedula: "1234567890"
solicitudId: "550e8400-e29b-41d4-a716-446655440000"
certificado: [archivo PDF/JPG/PNG/DOC/DOCX, máx 10MB]
```

**Respuesta exitosa (201 Created):**
```json
{
  "id": 2,
  "nombre": "Estadística Avanzada",
  "nombreOrganizacion": "Instituto de Matemáticas",
  "tipoOrganizacion": "Instituto",
  "numeroHoras": 60,
  "fechaFinalizacion": "2024-06-30",
  "nombreDocente": "Juan Pérez",
  "docenteCedula": "1234567890",
  "certificadoRuta": "uploads/cursos/nuevo-guid.pdf",
  "contenidoHash": "DEF456...",
  "organizacionId": 1
}
```

### 3. Obtener Curso por ID
```http
GET /api/Cursos/1
```

### 4. Actualizar Curso
```http
PUT /api/Cursos/1
Content-Type: multipart/form-data
```

**Parámetros:**
```
id: 1
nombre: "Metodologías de Investigación Actualizado"
organizacionId: 1
numeroHoras: 45
fechaFinalizacion: "2024-04-15"
docenteCedula: "1234567890"
certificado: [nuevo archivo - opcional]
```

### 5. Eliminar Curso
```http
DELETE /api/Cursos/1
```

### 6. Verificar Existencia de Curso
```http
HEAD /api/Cursos/1
```
- **200 OK**: El curso existe
- **404 Not Found**: El curso no existe

### 7. Obtener Cursos por Docente
```http
GET /api/Cursos/docente/1234567890
```

### 8. Obtener Cursos por Solicitud
```http
GET /api/Cursos/solicitud/550e8400-e29b-41d4-a716-446655440000
```

### 9. Asociar Curso Existente a Solicitud
```http
POST /api/Cursos/asociar
Content-Type: application/json

{
  "solicitudId": "550e8400-e29b-41d4-a716-446655440000",
  "cursoId": 1
}
```

### 10. Desasociar Curso de Solicitud
```http
POST /api/Cursos/desasociar
Content-Type: application/json

{
  "solicitudId": "550e8400-e29b-41d4-a716-446655440000",
  "cursoId": 1
}
```

### 11. Descargar Certificado
```http
GET /api/Cursos/1/certificado
```
**Respuesta:** Archivo del certificado con content-type apropiado

### 12. Vista Simplificada - Todos los Cursos
```http
GET /api/Cursos/ver
```

**Respuesta:**
```json
[
  {
    "id": 1,
    "nombre": "Metodologías de Investigación",
    "nombreOrganizacion": "Universidad Nacional",
    "numeroHoras": 40,
    "fechaFinalizacion": "2024-03-15",
    "nombreDocente": "Juan Pérez",
    "docenteCedula": "1234567890",
    "tieneCertificado": true
  }
]
```

### 13. Vista Simplificada - Cursos por Docente
```http
GET /api/Cursos/ver/docente/1234567890
```

## Validaciones

### Archivo de Certificado
- **Tipos permitidos:** PDF, JPG, JPEG, PNG, DOC, DOCX
- **Tamaño máximo:** 10MB
- **Requerido:** Sí para crear curso

### Datos del Curso
- **Nombre:** Requerido, máx 100 caracteres
- **OrganizacionId:** Requerido, debe existir
- **NumeroHoras:** Requerido, entre 1 y 1000
- **FechaFinalizacion:** Requerida
- **DocenteCedula:** Requerida, entre 8 y 10 caracteres
- **SolicitudId:** Requerida, debe existir

## Ejemplos de Uso con cURL

### Crear curso con certificado:
```bash
curl -X POST "https://localhost:7001/api/Cursos" \
  -H "Content-Type: multipart/form-data" \
  -F "nombre=Curso de Prueba" \
  -F "organizacionId=1" \
  -F "numeroHoras=40" \
  -F "fechaFinalizacion=2024-12-31" \
  -F "docenteCedula=1234567890" \
  -F "solicitudId=550e8400-e29b-41d4-a716-446655440000" \
  -F "certificado=@certificado.pdf"
```

### Obtener cursos de una solicitud:
```bash
curl -X GET "https://localhost:7001/api/Cursos/solicitud/550e8400-e29b-41d4-a716-446655440000"
```

### Descargar certificado:
```bash
curl -X GET "https://localhost:7001/api/Cursos/1/certificado" \
  --output certificado_descargado.pdf
```

## Códigos de Estado HTTP

- **200 OK**: Operación exitosa
- **201 Created**: Curso creado exitosamente
- **204 No Content**: Eliminación exitosa
- **400 Bad Request**: Datos inválidos
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error del servidor

## Notas Importantes

1. **Asociación Automática**: Al crear un curso, se asocia automáticamente a la solicitud especificada
2. **Integridad de Archivos**: Se genera hash SHA256 para verificar integridad
3. **Gestión de Archivos**: Los archivos se eliminan automáticamente al borrar un curso
4. **Validación de Referencias**: Se verifica que la solicitud y organización existan antes de crear

## Flujo Típico de Uso

1. **Docente crea solicitud de ascenso**
2. **Docente agrega curso directamente asociado:** `POST /api/Cursos` (incluye solicitudId)
3. **Sistema asocia automáticamente** el curso a la solicitud
4. **Evaluadores consultan cursos:** `GET /api/Cursos/solicitud/{id}`
5. **Evaluadores descargan certificados:** `GET /api/Cursos/{id}/certificado`

Este diseño elimina pasos innecesarios y sigue el flujo natural del proceso académico. 