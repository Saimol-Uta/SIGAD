# SIGAD - Generador de Certificados de Acción de Personal

Este proyecto proporciona una API para generar certificados de acción de personal para docentes de la Universidad Técnica de Ambato, siguiendo el formato oficial de la institución.

## Características

- Generación de certificados de acción de personal en formato PDF
- Plantilla HTML personalizable
- API RESTful para integración con otros sistemas
- Diseño idéntico al formato oficial de la UTA

## Requisitos previos

- Node.js (v14 o superior)
- npm o yarn
- Dependencias del sistema para canvas:
  - En Ubuntu/Debian: `sudo apt-get install build-essential libcairo2-dev libpango1.0-dev libjpeg-dev libgif-dev librsvg2-dev`
  - En Windows: Instalar Visual Studio Build Tools con C++ build tools
  - En macOS: `brew install pkg-config cairo pango libpng jpeg giflib librsvg`

## Instalación

1. Clonar el repositorio:
```
git clone <url-del-repositorio>
cd sigad-certificados
```

2. Instalar las dependencias:
```
npm install
```

3. Crear la estructura de directorios necesaria:
```
mkdir -p public/img temp
```

4. Agregar las imágenes requeridas en la carpeta `public/img`:
   - `logo_uta.png`: Logo oficial de la Universidad Técnica de Ambato
   - `firma_presidente.png`: Firma digitalizada del Presidente del H. Consejo Universitario
   - `firma_secretario.png`: Firma digitalizada del Secretario General

## Configuración

No se requiere configuración adicional para el funcionamiento básico. El servidor utilizará el puerto 3000 por defecto, pero puede cambiarse mediante la variable de entorno `PORT`.

## Uso

1. Iniciar el servidor:
```
npm start
```

2. Para desarrollo con recarga automática:
```
npm run dev
```

3. Acceder a la API:
   - Página principal: `http://localhost:3000/`
   - Endpoints de la API: `http://localhost:3000/api/certificados/`

## Endpoints de la API

### Generar un certificado de acción de personal
```
POST /api/certificados/accion-personal
```

Cuerpo de la solicitud (JSON):
```json
{
  "nombreCompleto": "Nombre del Docente",
  "cedula": "1234567890",
  "rangoAnterior": "Auxiliar 1",
  "rangoNuevo": "Auxiliar 2",
  "fechaSesion": "15 de julio de 2023",
  "periodoConvocatoria": "Primer semestre 2023",
  "fechaEfectivaPromocion": "1 de agosto de 2023",
  "dia": "20",
  "mes": "julio",
  "anio": "2023",
  "nombrePresidente": "Dr. Juan Pérez",
  "nombreSecretario": "Abg. María López",
  "consecutivo": "001"
}
```

Respuesta: Archivo PDF del certificado.

### Obtener la plantilla HTML del certificado
```
GET /api/certificados/plantilla-accion-personal
```

Respuesta: Archivo HTML con la plantilla del certificado.

## Integración con SIGAD

Para integrar este generador de certificados con el sistema SIGAD principal:

1. Asegúrese de que ambos sistemas puedan comunicarse (misma red o mediante API pública)
2. Realice llamadas a la API desde el sistema SIGAD cuando se necesite generar un certificado
3. Utilice los datos de la solicitud de ascenso aprobada para completar los campos del certificado

## Personalización

Para personalizar la apariencia del certificado:

1. Modifique el archivo `accion_personal_template.html` para cambiar la estructura HTML
2. Ajuste los estilos CSS dentro del mismo archivo
3. Para cambios más profundos en la generación del PDF, modifique `accion_personal_generator.js`

## Licencia

Este proyecto es propiedad de la Universidad Técnica de Ambato y está destinado para uso interno.
