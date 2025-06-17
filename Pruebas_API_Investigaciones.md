# API de Investigaciones - Documentación y Pruebas

## Descripción General
El API de Investigaciones permite gestionar las investigaciones académicas de los docentes, incluyendo la subida de informes y su asociación automática con solicitudes de ascenso.

## Endpoints Disponibles

### 1. Obtener todas las investigaciones
**GET** `/api/investigaciones`

Retorna todas las investigaciones con información completa.

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "titulo": "Investigación en Inteligencia Artificial",
    "fechaInicio": "2023-01-15T00:00:00",
    "fechaFinalizacion": "2023-12-15T00:00:00",
    "rolEnInvestigacion": "Investigador Principal",
    "mesesDeInvestigacion": 11,
    "nombreDocente": "Juan Pérez",
    "docenteCedula": "1234567890",
    "informeRuta": "uploads/investigaciones/guid.pdf",
    "contenidoHash": "ABC123..."
  }
]
```

### 2. Obtener investigación por ID
**GET** `/api/investigaciones/{id}`

**Parámetros:**
- `id` (int): ID de la investigación

**Respuesta exitosa (200):**
```json
{
  "id": 1,
  "titulo": "Investigación en Inteligencia Artificial",
  "fechaInicio": "2023-01-15T00:00:00",
  "fechaFinalizacion": "2023-12-15T00:00:00",
  "rolEnInvestigacion": "Investigador Principal",
  "mesesDeInvestigacion": 11,
  "nombreDocente": "Juan Pérez",
  "docenteCedula": "1234567890",
  "informeRuta": "uploads/investigaciones/guid.pdf",
  "contenidoHash": "ABC123..."
}
```

**Respuesta de error (404):**
```json
"Investigación con ID 1 no encontrada"
```

### 3. Obtener investigaciones por docente
**GET** `/api/investigaciones/docente/{cedula}`

**Parámetros:**
- `cedula` (string): Cédula del docente

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "titulo": "Investigación en IA",
    "fechaInicio": "2023-01-15T00:00:00",
    "fechaFinalizacion": "2023-12-15T00:00:00",
    "rolEnInvestigacion": "Investigador Principal",
    "mesesDeInvestigacion": 11,
    "nombreDocente": "Juan Pérez",
    "docenteCedula": "1234567890",
    "informeRuta": "uploads/investigaciones/guid.pdf",
    "contenidoHash": "ABC123..."
  }
]
```

### 4. Obtener investigaciones por solicitud
**GET** `/api/investigaciones/solicitud/{solicitudId}`

**Parámetros:**
- `solicitudId` (Guid): ID de la solicitud de ascenso

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "titulo": "Investigación en IA",
    "fechaInicio": "2023-01-15T00:00:00",
    "fechaFinalizacion": "2023-12-15T00:00:00",
    "rolEnInvestigacion": "Investigador Principal",
    "mesesDeInvestigacion": 11,
    "nombreDocente": "Juan Pérez",
    "docenteCedula": "1234567890",
    "informeRuta": "uploads/investigaciones/guid.pdf",
    "contenidoHash": "ABC123..."
  }
]
```

### 5. Crear nueva investigación
**POST** `/api/investigaciones`

**Content-Type:** `multipart/form-data`

**Parámetros del formulario:**
```json
{
  "titulo": "Nueva Investigación en Machine Learning",
  "fechaInicio": "2024-01-01",
  "fechaFinalizacion": "2024-12-31",
  "rolEnInvestigacion": "Co-investigador",
  "mesesDeInvestigacion": 12,
  "docenteCedula": "1234567890",
  "solicitudId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Archivo:**
- `informe`: Archivo PDF, DOC o DOCX (máximo 25MB)

**Validaciones:**
- Título: requerido, máximo 200 caracteres
- Fechas: requeridas, fecha fin debe ser posterior a fecha inicio
- Rol: requerido, máximo 50 caracteres
- Meses: requerido, entre 1 y 120
- Cédula docente: requerida, entre 8 y 10 caracteres
- Solicitud ID: requerido, debe existir en el sistema
- Archivo: requerido, tipos permitidos: PDF, DOC, DOCX

**Respuesta exitosa (201):**
```json
{
  "id": 2,
  "titulo": "Nueva Investigación en Machine Learning",
  "fechaInicio": "2024-01-01T00:00:00",
  "fechaFinalizacion": "2024-12-31T00:00:00",
  "rolEnInvestigacion": "Co-investigador",
  "mesesDeInvestigacion": 12,
  "nombreDocente": "Juan Pérez",
  "docenteCedula": "1234567890",
  "informeRuta": "uploads/investigaciones/new-guid.pdf",
  "contenidoHash": "DEF456..."
}
```

**Respuestas de error (400):**
```json
"El informe es requerido"
"Tipo de archivo no permitido. Use: PDF, DOC, DOCX"
"El archivo no puede exceder los 25MB"
"La solicitud especificada no existe"
"La fecha de finalización debe ser posterior a la fecha de inicio"
```

### 6. Actualizar investigación
**PUT** `/api/investigaciones/{id}`

**Content-Type:** `application/json`

**Parámetros:**
- `id` (int): ID de la investigación a actualizar

**Cuerpo de la solicitud:**
```json
{
  "titulo": "Investigación Actualizada en Deep Learning",
  "fechaInicio": "2024-02-01",
  "fechaFinalizacion": "2024-11-30",
  "rolEnInvestigacion": "Investigador Principal",
  "mesesDeInvestigacion": 10
}
```

**Nota:** Este endpoint solo actualiza los datos básicos, no el archivo de informe.

**Respuesta exitosa (200):**
```json
{
  "id": 1,
  "titulo": "Investigación Actualizada en Deep Learning",
  "fechaInicio": "2024-02-01T00:00:00",
  "fechaFinalizacion": "2024-11-30T00:00:00",
  "rolEnInvestigacion": "Investigador Principal",
  "mesesDeInvestigacion": 10,
  "nombreDocente": "Juan Pérez",
  "docenteCedula": "1234567890",
  "informeRuta": "uploads/investigaciones/guid.pdf",
  "contenidoHash": "ABC123..."
}
```

**Respuestas de error:**
- **404:** `"Investigación con ID 1 no encontrada"`
- **400:** `"La fecha de finalización debe ser posterior a la fecha de inicio"`

### 7. Eliminar investigación
**DELETE** `/api/investigaciones/{id}`

**Parámetros:**
- `id` (int): ID de la investigación a eliminar

**Respuesta exitosa (204):** Sin contenido

**Respuesta de error (404):**
```json
"Investigación con ID 1 no encontrada"
```

**Nota:** Al eliminar una investigación, también se elimina el archivo de informe asociado del sistema de archivos.

### 8. Vista simplificada de investigaciones
**GET** `/api/investigaciones/ver`

Retorna una vista simplificada de todas las investigaciones para listados rápidos.

**Respuesta exitosa (200):**
```json
[
  {
    "id": 1,
    "titulo": "Investigación en IA",
    "rolEnInvestigacion": "Investigador Principal",
    "mesesDeInvestigacion": 11,
    "nombreDocente": "Juan Pérez"
  },
  {
    "id": 2,
    "titulo": "Investigación en ML",
    "rolEnInvestigacion": "Co-investigador",
    "mesesDeInvestigacion": 8,
    "nombreDocente": "María García"
  }
]
```

### 9. Descargar informe de investigación
**GET** `/api/investigaciones/{id}/descargar-informe`

**Parámetros:**
- `id` (int): ID de la investigación

**Respuesta exitosa (200):**
- **Content-Type:** Según el tipo de archivo (application/pdf, application/msword, etc.)
- **Content-Disposition:** `attachment; filename="Informe_Investigacion_1_Titulo.pdf"`
- **Cuerpo:** Contenido binario del archivo

**Respuesta de error (404):**
```json
"Informe no encontrado"
```

## Flujo de Trabajo Recomendado

### Crear una nueva investigación:
1. **POST** `/api/investigaciones` con datos del formulario y archivo
   - La investigación se crea automáticamente asociada a la solicitud especificada
   - El archivo se almacena de forma segura con hash SHA256
   - Se retorna la investigación creada con toda la información

### Consultar investigaciones:
1. **GET** `/api/investigaciones/ver` para vista rápida
2. **GET** `/api/investigaciones/{id}` para detalles completos
3. **GET** `/api/investigaciones/docente/{cedula}` para investigaciones de un docente
4. **GET** `/api/investigaciones/solicitud/{solicitudId}` para investigaciones de una solicitud

### Actualizar investigación:
1. **PUT** `/api/investigaciones/{id}` para actualizar datos básicos
   - No se puede cambiar el archivo de informe mediante este endpoint

### Gestión de archivos:
1. **GET** `/api/investigaciones/{id}/descargar-informe` para descargar informes

## Configuración Requerida

### appsettings.json:
```json
{
  "FileStorage": {
    "InvestigacionesPath": "uploads/investigaciones"
  }
}
```

### Validaciones de Archivos:
- **Tipos permitidos:** PDF, DOC, DOCX
- **Tamaño máximo:** 25MB
- **Seguridad:** Hash SHA256 para verificación de integridad
- **Almacenamiento:** Nombres únicos con GUID para evitar conflictos

## Códigos de Estado HTTP

- **200 OK:** Operación exitosa
- **201 Created:** Investigación creada exitosamente
- **204 No Content:** Eliminación exitosa
- **400 Bad Request:** Datos inválidos o validaciones fallidas
- **404 Not Found:** Recurso no encontrado
- **500 Internal Server Error:** Error interno del servidor

## Notas Importantes

1. **Asociación Automática:** Al crear una investigación, se asocia automáticamente a la solicitud especificada
2. **Gestión de Archivos:** Los archivos se almacenan con nombres únicos y se eliminan automáticamente al borrar la investigación
3. **Validación de Integridad:** Se genera un hash SHA256 para cada archivo subido
4. **Seguridad:** Solo se permiten tipos de archivo específicos y se valida el tamaño máximo
5. **Relaciones:** Las investigaciones están vinculadas a docentes y pueden asociarse a múltiples solicitudes de ascenso 