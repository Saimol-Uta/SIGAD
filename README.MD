# 🎓 SIGAD - Sistema de Gestión Académica Docente

Sistema completo para la gestión de ascensos docentes, desarrollado en .NET Core 9 con SQL Server.

## 📋 Características

- ✅ **API REST completa** para gestión de evaluaciones docentes
- ✅ **Arquitectura limpia** (Domain, Application, Infrastructure, WebAPI)
- ✅ **Autenticación JWT** con roles (Admin/Docente)
- ✅ **Manejo de archivos** (PDFs, documentos)
- ✅ **Base de datos SQL Server** con Entity Framework Core
- ✅ **Documentación Swagger** integrada
- ✅ **Validaciones** y manejo de errores robusto

## 🏗️ Arquitectura del Proyecto

```
SIGAD/
├── SIGAD.Domain/           # Entidades y interfaces principales
├── SIGAD.Application/      # DTOs y servicios de aplicación
├── SIGAD.Infrastructure/   # Repositorios y contexto de BD
├── SIGAD.WebAPI/          # Controladores y configuración API
└── Scripts SQL/           # Scripts de base de datos
```

## 🚀 Instalación y Configuración

### Prerrequisitos

- .NET 9 SDK
- SQL Server (LocalDB, Express, o Full)
- Visual Studio 2022 o VS Code

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/sigad.git
cd sigad
```

### 2. Configurar la base de datos

1. **Crear la base de datos** ejecutando el script completo de creación de esquema
2. **Insertar datos de prueba** ejecutando `script_insertar_datos_prueba_completo.sql`

### 3. Configurar la aplicación

1. **Copiar archivo de configuración:**
   ```bash
   cp appsettings.Example.json appsettings.Development.json
   ```

2. **Editar `appsettings.Development.json`** con tus datos:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqlocaldb;Database=SISTEMA_DOCENTES;Trusted_Connection=true;TrustServerCertificate=true;"
     },
     "JwtSettings": {
       "SecretKey": "tu-clave-secreta-muy-larga-y-segura-de-al-menos-32-caracteres"
     }
   }
   ```

3. **Crear carpetas de archivos:**
   ```bash
   mkdir uploads
   mkdir uploads/evaluaciones
   mkdir uploads/articulos
   mkdir uploads/certificados
   mkdir uploads/investigaciones
   ```

### 4. Restaurar dependencias y ejecutar

```bash
dotnet restore
dotnet run --project SIGAD.WebAPI
```

## 🌐 URLs del Sistema

- **API Base:** `https://localhost:7087/api`
- **Swagger UI:** `https://localhost:7087/swagger`
- **Health Check:** `https://localhost:7087/api/health` (si está configurado)

## 📚 Módulos Implementados

### ✅ Evaluaciones Docentes
- CRUD completo de evaluaciones
- Subida y descarga de archivos PDF
- Asociación con solicitudes de ascenso
- Filtros por docente y periodo
- Exportación a JSON/CSV

### 🔄 Próximos Módulos (En Desarrollo)
- Gestión de Artículos
- Gestión de Cursos
- Experiencia Laboral
- Investigaciones

## 🧪 Datos de Prueba

El sistema incluye datos de prueba listos para usar:

### Docentes de Prueba:
- **Juan Carlos Pérez González** (1234567890)
- **María Isabel Rodríguez López** (0987654321)
- **Carlos Eduardo Martínez Silva** (1122334455)

### Credenciales de Prueba:
- **Correo:** `juan.perez@universidad.edu.co`
- **Contraseña:** `password123`

## 🔧 Endpoints Principales

### Evaluaciones (`/api/evaluaciones`)
```
GET    /                      # Obtener todas las evaluaciones
GET    /{id}                  # Obtener evaluación por ID
GET    /docente/{cedula}      # Evaluaciones por docente
POST   /                      # Crear nueva evaluación
PUT    /{id}                  # Actualizar evaluación
DELETE /{id}                  # Eliminar evaluación
GET    /{id}/archivo          # Descargar archivo de evaluación
GET    /exportar              # Exportar datos
```

## 📱 Uso con Swagger

1. Navegar a `https://localhost:7087/swagger`
2. Expandir el endpoint deseado
3. Hacer clic en "Try it out"
4. Completar los parámetros requeridos
5. Ejecutar y revisar la respuesta

### Ejemplo de Creación de Evaluación:
```json
{
  "periodoAcademico": "2024-1",
  "fechaEvaluacion": "2024-01-15",
  "puntajePorcentual": 85.5,
  "docenteCedula": "1234567890",
  "solicitudId": "guid-de-solicitud-existente"
}
```

## 🔐 Seguridad

- **Autenticación JWT** implementada
- **Roles de usuario** (ADMINISTRADOR/DOCENTE)
- **Validación de entrada** en todos los endpoints
- **Manejo seguro de archivos** con hash SHA256

## 📂 Estructura de Archivos

Los archivos subidos se organizan automáticamente:
```
uploads/
├── evaluaciones/    # Informes de evaluaciones docentes
├── articulos/       # PDFs de artículos científicos
├── certificados/    # Certificados de cursos y experiencia
└── investigaciones/ # Informes de investigación
```

## 🐛 Resolución de Problemas

### Error de conexión a base de datos
- Verificar que SQL Server esté ejecutándose
- Confirmar la cadena de conexión en `appsettings.json`
- Verificar que la base de datos `SISTEMA_DOCENTES` exista

### Error de archivos no encontrados
- Verificar que las carpetas `uploads/` existan
- Confirmar permisos de escritura en la carpeta

### Error de descarga de PDFs en Swagger
- Normal en Swagger UI, probar la URL directa en el navegador
- Usar Postman para probar descargas de archivos

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👥 Equipo de Desarrollo

- **Desarrollador Principal:** [Tu Nombre]
- **Arquitectura:** .NET Core 9 + SQL Server
- **Patrón:** Clean Architecture

---

**📞 Soporte:** Para problemas o preguntas, crear un issue en GitHub.