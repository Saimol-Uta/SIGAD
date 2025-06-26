# 🎓 SIGAD - Sistema de Gestión Académica Docente

Sistema completo para la gestión de ascensos docentes universitarios, desarrollado con .NET 9 y SQL Server. Incluye aplicación web Blazor Server y API REST.

## 📋 Características Principales

- ✅ **Aplicación Web Blazor Server** con interfaz moderna y responsiva
- ✅ **API REST completa** para gestión de documentos académicos
- ✅ **Arquitectura limpia** (Domain, Application, Infrastructure, WebAPI, BlazorApp)
- ✅ **Autenticación JWT** con roles diferenciados (Administrador/Docente)
- ✅ **Gestión completa de documentos:**
  - 📄 Artículos científicos
  - 🔬 Proyectos de investigación
  - 🎓 Cursos y capacitaciones
  - 📊 Evaluaciones docentes
  - 💼 Experiencia laboral
  - 📘 Tesis dirigidas
- ✅ **Subida y descarga de archivos** (PDF, DOC, DOCX)
- ✅ **Validación automática de requisitos** para ascensos
- ✅ **Base de datos SQL Server** con Entity Framework Core
- ✅ **Migraciones automáticas** de base de datos
- ✅ **Documentación Swagger** integrada
- ✅ **Sistema de solicitudes** con flujo de aprobación

## 🏗️ Arquitectura del Proyecto

```
SIGAD/
├── SIGAD.Domain/              # 🏛️ Entidades del dominio e interfaces
│   ├── Entities/              # Entidades principales (Docente, Solicitud, etc.)
│   ├── Enums/                 # Enumeraciones del sistema
│   └── Interfaces/            # Interfaces de repositorios
├── SIGAD.Application/         # 📋 Lógica de aplicación y DTOs
│   ├── DTOs/                  # Objetos de transferencia de datos
│   ├── Services/              # Servicios de aplicación
│   └── Interfaces/            # Interfaces de servicios
├── SIGAD.Infrastructure/      # 🗄️ Acceso a datos y servicios externos
│   ├── Persistence/           # Contexto de Entity Framework
│   ├── Repositories/          # Implementaciones de repositorios
│   └── Migrations/            # Migraciones de base de datos
├── SIGAD.WebAPI/             # 🌐 API REST y controladores
│   ├── Controllers/           # Controladores de la API
│   ├── Middleware/            # Middleware personalizado
│   └── uploads/               # Archivos subidos por usuarios
├── SIGAD.BlazorApp/          # 🖥️ Aplicación web Blazor Server
│   ├── Pages/                 # Páginas de la aplicación
│   ├── Components/            # Componentes reutilizables
│   ├── Layout/                # Layouts de página
│   └── Services/              # Servicios del cliente
└── basesExternas/            # 📊 Scripts de bases de datos externas
    ├── DITIC.sql             # Base de datos de sistemas
    ├── SGTH.sql              # Base de datos de talento humano
    └── SUT.sql               # Base de datos académica
```

## 🚀 Guía de Instalación Completa

### 📋 Prerrequisitos

Antes de comenzar, asegúrate de tener instalado:

- **[.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** (versión 9.0 o superior)
- **[SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)** (LocalDB, Express, Developer, o Full)
- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** (recomendado) o **[VS Code](https://code.visualstudio.com/)**
- **[Git](https://git-scm.com/)** para control de versiones

### 🔧 Verificar Instalaciones

Abre una terminal/PowerShell y verifica que tienes todo instalado:

```bash
# Verificar .NET
dotnet --version
# Debe mostrar 9.0.x

# Verificar SQL Server (LocalDB)
sqllocaldb info
# Debe mostrar las instancias disponibles

# Verificar Git
git --version
```

### 1️⃣ Clonar el Repositorio

```bash
# Clonar el proyecto
git clone https://github.com/tu-usuario/sigad.git
cd sigad

# Verificar la estructura del proyecto
dir  # En Windows
ls   # En Linux/Mac
```

### 2️⃣ Configurar la Base de Datos

#### Opción A: Usando SQL Server LocalDB (Recomendado para desarrollo)

```bash
# Crear una instancia de LocalDB si no existe
sqllocaldb create MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

#### Opción B: Usando SQL Server Express/Full

Si tienes SQL Server instalado, puedes usar tu instancia existente.

### 3️⃣ Configurar Cadenas de Conexión

1. **Navega a la carpeta del proyecto API:**
   ```bash
   cd SIGAD.WebAPI
   ```

2. **Crea el archivo de configuración:**
   ```bash
   # Copia el archivo de ejemplo
   copy appsettings.json appsettings.Development.json
   ```

3. **Edita `appsettings.Development.json`** con tus configuraciones:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SIGAD_DB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;",
       "SGTH": "Server=(localdb)\\MSSQLLocalDB;Database=SGTH;Trusted_Connection=true;TrustServerCertificate=true;",
       "SUT": "Server=(localdb)\\MSSQLLocalDB;Database=SUT;Trusted_Connection=true;TrustServerCertificate=true;",
       "DITIC": "Server=(localdb)\\MSSQLLocalDB;Database=DITIC;Trusted_Connection=true;TrustServerCertificate=true;"
     },
     "JwtSettings": {
       "SecretKey": "Mi-Clave-Super-Secreta-Para-JWT-Que-Debe-Ser-Muy-Larga-Y-Segura-123456789",
       "Issuer": "SIGAD-API",
       "Audience": "SIGAD-Users",
       "ExpiryMinutes": 60
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

### 4️⃣ Restaurar Dependencias

```bash
# Desde la carpeta raíz del proyecto
dotnet restore

# Esto descargará todas las dependencias necesarias
```

### 5️⃣ Ejecutar Migraciones de Base de Datos

```bash
# Navegar a la carpeta del proyecto API
cd SIGAD.WebAPI

# Crear y aplicar migraciones
dotnet ef database update

# Si es la primera vez, esto creará automáticamente:
# - La base de datos SIGAD_DB
# - Todas las tablas necesarias
# - Datos de prueba iniciales
```

#### 🔧 Solución de Problemas con Migraciones

Si encuentras errores con las migraciones:

```bash
# Verificar el estado de las migraciones
dotnet ef migrations list

# Si no hay migraciones, crear una inicial
dotnet ef migrations add InitialCreate

# Aplicar migraciones
dotnet ef database update

# Si hay problemas, eliminar la base de datos y recrear
dotnet ef database drop
dotnet ef database update
```

### 6️⃣ Crear Carpetas de Archivos

```bash
# Crear estructura de carpetas para archivos subidos
mkdir uploads
mkdir uploads\evaluaciones
mkdir uploads\articulos
mkdir uploads\certificados
mkdir uploads\investigaciones
mkdir uploads\cursos
mkdir uploads\experiencias
mkdir uploads\tesis

# En Linux/Mac usar:
# mkdir -p uploads/{evaluaciones,articulos,certificados,investigaciones,cursos,experiencias,tesis}
```

### 7️⃣ Configurar Bases de Datos Externas (Opcional)

Si quieres probar la integración con sistemas externos:

```bash
# Desde SQL Server Management Studio o Azure Data Studio
# Ejecutar los scripts en este orden:
# 1. basesExternas/SGTH.sql
# 2. basesExternas/SUT.sql  
# 3. basesExternas/DITIC.sql
```

### 8️⃣ Ejecutar la Aplicación

#### Opción A: Ejecutar solo la API

```bash
# Desde SIGAD.WebAPI
dotnet run

# O con hot reload para desarrollo
dotnet watch run
```

#### Opción B: Ejecutar la aplicación completa (API + Blazor)

```bash
# Terminal 1: Ejecutar la API
cd SIGAD.WebAPI
dotnet run

# Terminal 2: Ejecutar Blazor (en otra terminal)
cd SIGAD.BlazorApp
dotnet run
```

#### Opción C: Usando Visual Studio

1. Abrir `SIGAD.sln` en Visual Studio
2. Configurar múltiples proyectos de inicio:
   - Click derecho en la solución → "Establecer proyectos de inicio"
   - Seleccionar "Varios proyectos de inicio"
   - Establecer `SIGAD.WebAPI` y `SIGAD.BlazorApp` como "Iniciar"
3. Presionar F5 para ejecutar

## 🌐 URLs y Acceso al Sistema

Una vez que la aplicación esté ejecutándose, podrás acceder a:

### 🖥️ Aplicación Web Principal
- **URL:** `https://localhost:5001` o `http://localhost:5000`
- **Descripción:** Interfaz principal para docentes y administradores

### 🔧 API REST
- **URL Base:** `https://localhost:7087/api` o `http://localhost:5000/api`
- **Documentación Swagger:** `https://localhost:7087/swagger`
- **Descripción:** API para integraciones y desarrollo

### 🏥 Monitoreo (Si está habilitado)
- **Health Check:** `https://localhost:7087/health`

## 👥 Credenciales de Prueba

El sistema incluye usuarios de prueba listos para usar:

### 🧑‍🏫 Docentes de Prueba

| Nombre | Cédula | Correo | Contraseña | Rol |
|--------|---------|---------|------------|-----|
| Juan Carlos Pérez | 1234567890 | juan.perez@universidad.edu.co | password123 | DOCENTE |
| María Isabel Rodríguez | 0987654321 | maria.rodriguez@universidad.edu.co | password123 | DOCENTE |
| Carlos Eduardo Martínez | 1122334455 | carlos.martinez@universidad.edu.co | password123 | DOCENTE |

### 👑 Administrador de Prueba

| Nombre | Cédula | Correo | Contraseña | Rol |
|--------|---------|---------|------------|-----|
| Admin Sistema | 1111111111 | admin@universidad.edu.co | admin123 | ADMINISTRADOR |

## 🎯 Primeros Pasos

### 1. Acceder al Sistema

1. **Abrir navegador** y ir a `https://localhost:5001`
2. **Iniciar sesión** con cualquiera de las credenciales de prueba
3. **Explorar** las diferentes secciones del sistema

### 2. Como Docente

1. **Gestión de Documentos:** Subir y organizar documentos académicos
2. **Crear Solicitud:** Iniciar proceso de ascenso
3. **Agregar Documentos:** Artículos, investigaciones, cursos, etc.
4. **Verificar Requisitos:** Comprobar si cumple con los requisitos
5. **Enviar Solicitud:** Enviar para revisión administrativa

### 3. Como Administrador

1. **Gestión de Solicitudes:** Revisar y aprobar solicitudes
2. **Gestión de Rangos:** Configurar requisitos por rango
3. **Reportes:** Generar reportes del sistema
4. **Gestión de Usuarios:** Administrar docentes y permisos

## 📚 Módulos del Sistema

### ✅ Módulos Completamente Implementados

#### 🏠 **Dashboard Principal**
- Vista general del estado del docente
- Resumen de documentos y solicitudes
- Navegación principal del sistema

#### 📊 **Evaluaciones Docentes**
- CRUD completo de evaluaciones
- Subida de informes en PDF
- Cálculo automático de promedios
- Asociación con solicitudes de ascenso
- Filtros por docente y periodo

#### 📄 **Artículos Científicos**
- Registro de publicaciones
- Subida de PDFs de artículos
- Validación de duplicados
- Filtros por año y revista

#### � **Proyectos de Investigación**
- Gestión de proyectos de investigación
- Cálculo automático de meses de investigación
- Subida de informes y certificados
- Estados: En Proceso, Culminado, Suspendido

#### 🎓 **Cursos y Capacitaciones**
- Registro de cursos impartidos y recibidos
- Cálculo automático de horas
- Certificados digitales
- Categorización por tipo de curso

#### 💼 **Experiencia Laboral**
- Historial profesional completo
- Cálculo automático de años de experiencia
- Certificados laborales
- Validación de fechas

#### 📘 **Tesis Dirigidas**
- Registro de tesis dirigidas o co-dirigidas
- Control por nivel académico (Pregrado, Maestría, Doctorado)
- Estados de avance
- Documentos de certificación

#### 📋 **Sistema de Solicitudes**
- Creación de solicitudes de ascenso
- Validación automática de requisitos
- Flujo de aprobación completo
- Estados: Borrador, Enviada, En Revisión, Aprobada, Rechazada

#### 👑 **Panel Administrativo**
- Gestión de solicitudes
- Configuración de rangos y requisitos
- Generación de reportes
- Gestión de usuarios

### 🔄 Integraciones con Sistemas Externos

#### 📊 **SGTH (Sistema de Gestión de Talento Humano)**
- Importación automática de datos de docentes
- Sincronización de información laboral
- Historial de evaluaciones

#### 🎓 **SUT (Sistema Único de Títulos)**
- Verificación automática de títulos académicos
- Importación de información educativa
- Validación de credenciales

#### 💻 **DITIC (Dirección de Tecnologías)**
- Integración con sistemas institucionales
- Gestión de usuarios y permisos
- Auditoría de accesos

## 🔧 API REST - Endpoints Principales

### 🔐 **Autenticación (`/api/auth`)**
```
POST   /login                    # Iniciar sesión
POST   /logout                   # Cerrar sesión
POST   /register                 # Registrar nuevo usuario
GET    /verify                   # Verificar token JWT
POST   /forgot-password          # Recuperar contraseña
POST   /reset-password           # Restablecer contraseña
GET    /verificar-requisitos/{id} # Verificar requisitos de solicitud
POST   /enviar-solicitud/{id}    # Enviar solicitud para revisión
```

### 📊 **Evaluaciones (`/api/evaluaciones`)**
```
GET    /                         # Obtener todas las evaluaciones
GET    /{id}                     # Obtener evaluación por ID
GET    /docente/{cedula}         # Evaluaciones por docente
POST   /                         # Crear nueva evaluación
PUT    /{id}                     # Actualizar evaluación
DELETE /{id}                     # Eliminar evaluación
GET    /{id}/archivo             # Descargar archivo de evaluación
POST   /externas/importar/{cedula} # Importar desde sistemas externos
```

### 📄 **Artículos (`/api/articulos`)**
```
GET    /                         # Obtener todos los artículos
GET    /{id}                     # Obtener artículo por ID
GET    /docente/{cedula}         # Artículos por docente
POST   /                         # Crear nuevo artículo
PUT    /{id}                     # Actualizar artículo
DELETE /{id}                     # Eliminar artículo
GET    /externos/importar/{cedula} # Importar artículos externos
```

### 🔬 **Investigaciones (`/api/investigaciones`)**
```
GET    /                         # Obtener todas las investigaciones
GET    /{id}                     # Obtener investigación por ID
GET    /docente/{cedula}         # Investigaciones por docente
POST   /                         # Crear nueva investigación
PUT    /{id}                     # Actualizar investigación
DELETE /{id}                     # Eliminar investigación
POST   /externa/importar/{cedula} # Importar desde sistemas externos
```

### 📘 **Tesis Dirigidas (`/api/tesisDirigidas`)**
```
GET    /                         # Obtener todas las tesis
GET    /{id}                     # Obtener tesis por ID
GET    /docente/{cedula}         # Tesis por docente
POST   /                         # Crear nueva tesis
PUT    /{id}                     # Actualizar tesis
DELETE /{id}                     # Eliminar tesis
POST   /asociar                  # Asociar tesis a solicitud
DELETE /desasociar               # Desasociar tesis de solicitud
```

### 📤 **Subida de Archivos (`/api/upload`)**
```
POST   /                         # Subir archivo general
POST   /evaluacion               # Subir archivo de evaluación
POST   /articulo                 # Subir PDF de artículo
POST   /certificado              # Subir certificado
```

## 🧪 Ejemplos de Uso con Swagger

### 1. Acceder a Swagger UI
1. Ejecutar la aplicación API
2. Navegar a `https://localhost:7087/swagger`
3. Explorar todos los endpoints disponibles

### 2. Autenticación en Swagger
1. Usar el endpoint `/api/auth/login` con credenciales de prueba
2. Copiar el token JWT de la respuesta
3. Hacer clic en "Authorize" en la parte superior
4. Pegar el token en el formato: `Bearer {tu-token-aqui}`

### 3. Ejemplo: Crear una Evaluación Docente
```json
{
  "periodoAcademico": "2024-2",
  "fechaEvaluacion": "2024-06-15T00:00:00",
  "puntajePorcentual": 88.5,
  "informeRuta": "ruta/al/archivo.pdf",
  "docenteCedula": "1234567890"
}
```

### 4. Ejemplo: Crear un Artículo Científico
```json
{
  "titulo": "Inteligencia Artificial en la Educación Superior",
  "revista": "Revista Internacional de Tecnología Educativa",
  "anioPublicacion": 2024,
  "paginaInicio": 45,
  "paginaFin": 62,
  "url": "https://revista.ejemplo.com/articulo/123",
  "archivoRuta": "ruta/al/articulo.pdf",
  "docenteCedula": "1234567890"
}
```

## � Solución de Problemas Comunes

### ❌ Error: "No se puede conectar a la base de datos"

**Causa:** SQL Server no está ejecutándose o la cadena de conexión es incorrecta.

**Solución:**
```bash
# Verificar SQL Server LocalDB
sqllocaldb info

# Iniciar LocalDB si está detenido
sqllocaldb start MSSQLLocalDB

# Verificar la cadena de conexión en appsettings.Development.json
```

### ❌ Error: "No se encontró el archivo de configuración"

**Causa:** No existe el archivo `appsettings.Development.json`.

**Solución:**
```bash
# Crear el archivo desde la plantilla
cd SIGAD.WebAPI
copy appsettings.json appsettings.Development.json
# Editar el archivo con tus configuraciones
```

### ❌ Error: "Las migraciones fallan"

**Causa:** Problemas con Entity Framework o base de datos existente.

**Solución:**
```bash
# Eliminar migraciones existentes
dotnet ef migrations remove

# Crear nueva migración
dotnet ef migrations add InitialCreate

# Aplicar migración
dotnet ef database update

# Si persiste, eliminar la base de datos completa
dotnet ef database drop
dotnet ef database update
```

### ❌ Error: "No se pueden subir archivos"

**Causa:** Las carpetas de uploads no existen o no tienen permisos.

**Solución:**
```bash
# Crear carpetas manualmente
mkdir uploads
mkdir uploads\evaluaciones uploads\articulos uploads\certificados

# Verificar permisos de escritura en la carpeta uploads
```

### ❌ Error: "Puerto ya en uso"

**Causa:** Otra aplicación está usando el puerto.

**Solución:**
```bash
# Cambiar el puerto en launchSettings.json
# O terminar procesos que usan el puerto
netstat -ano | findstr :5001
taskkill /PID [ID_DEL_PROCESO] /F
```

### ❌ Error: "JWT Token inválido"

**Causa:** Token expirado o clave secreta incorrecta.

**Solución:**
- Verificar que `JwtSettings:SecretKey` tenga al menos 32 caracteres
- Volver a iniciar sesión para obtener un token nuevo
- Verificar que la configuración JWT sea consistente

### ❌ Error: "No se puede descargar archivos en Swagger"

**Causa:** Limitación normal de Swagger UI para descargas.

**Solución:**
- Usar la URL directa en el navegador: `https://localhost:7087/api/evaluaciones/{id}/archivo`
- Usar Postman o herramientas similares para probar descargas
- La funcionalidad funciona correctamente en la aplicación web principal

## 🔐 Características de Seguridad

- ✅ **Autenticación JWT** con tokens seguros
- ✅ **Autorización basada en roles** (ADMINISTRADOR/DOCENTE)
- ✅ **Validación de entrada** exhaustiva en todos los endpoints
- ✅ **Manejo seguro de archivos** con validación de tipos y tamaños
- ✅ **Hash SHA256** para verificación de integridad de documentos
- ✅ **Prevención de duplicados** mediante hash de contenido
- ✅ **Sanitización de nombres de archivo** para prevenir vulnerabilidades
- ✅ **Middleware de manejo de errores** personalizado
- ✅ **Logs de auditoría** para acciones críticas

## 📂 Organización de Archivos

Los archivos se organizan automáticamente en la siguiente estructura:

```
uploads/
├── evaluaciones/          # 📊 Informes de evaluaciones docentes
│   ├── 2024/             # Organizados por año
│   └── [cedula]/         # Subcarpetas por docente
├── articulos/            # 📄 PDFs de artículos científicos
│   ├── indexados/        # Artículos en revistas indexadas
│   └── no-indexados/     # Artículos en revistas no indexadas
├── certificados/         # 🎓 Certificados de cursos y capacitaciones
│   ├── cursos/           # Certificados de cursos
│   └── experiencia/      # Certificados laborales
├── investigaciones/      # 🔬 Informes de proyectos de investigación
│   ├── culminados/       # Proyectos terminados
│   └── en-proceso/       # Proyectos activos
└── tesis/               # 📘 Documentos de tesis dirigidas
    ├── pregrado/         # Tesis de pregrado
    ├── maestria/         # Tesis de maestría
    └── doctorado/        # Tesis doctorales
```

## 🔧 Comandos Útiles para Desarrollo

### Gestión de Migraciones
```bash
# Ver migraciones disponibles
dotnet ef migrations list

# Crear nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración específica
dotnet ef database update NombreDeMigracionAnterior

# Eliminar última migración (si no se ha aplicado)
dotnet ef migrations remove

# Generar script SQL de migración
dotnet ef migrations script

# Eliminar base de datos completamente
dotnet ef database drop
```

### Gestión del Proyecto
```bash
# Limpiar solución
dotnet clean

# Restaurar paquetes NuGet
dotnet restore

# Compilar solución
dotnet build

# Ejecutar con recarga automática
dotnet watch run

# Ejecutar tests (si existen)
dotnet test

# Publicar aplicación
dotnet publish -c Release
```

## 🌐 Configuración para Producción

### Variables de Entorno Recomendadas

```bash
# Configurar en el servidor de producción
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://*:443;http://*:80

# Cadenas de conexión seguras
ConnectionStrings__DefaultConnection="Server=servidor-prod;Database=SIGAD_PROD;User Id=usuario;Password=contraseña-segura;"

# JWT con clave fuerte
JwtSettings__SecretKey="clave-super-secreta-de-produccion-con-256-bits-minimo"
JwtSettings__Issuer="SIGAD-PRODUCTION"
JwtSettings__ExpiryMinutes=30
```

### Checklist de Despliegue

- [ ] Configurar HTTPS con certificados válidos
- [ ] Configurar base de datos de producción
- [ ] Establecer backups automáticos de base de datos
- [ ] Configurar logs centralizados
- [ ] Establecer monitoreo y alertas
- [ ] Configurar firewall y restricciones de acceso
- [ ] Verificar permisos de carpetas de uploads
- [ ] Configurar balanceador de carga (si aplica)
- [ ] Establecer políticas de retención de archivos
- [ ] Configurar SSL/TLS apropiadamente

## 🤝 Contribución al Proyecto

### Configurar Entorno de Desarrollo

1. **Fork el repositorio** en GitHub
2. **Clonar tu fork:**
   ```bash
   git clone https://github.com/tu-usuario/sigad.git
   cd sigad
   ```
3. **Configurar remote upstream:**
   ```bash
   git remote add upstream https://github.com/repositorio-original/sigad.git
   ```
4. **Crear rama para tu feature:**
   ```bash
   git checkout -b feature/nueva-funcionalidad
   ```

### Estándares de Código

- **Convención de nombres:** PascalCase para clases, camelCase para variables
- **Comentarios:** Documentar métodos públicos con XML comments
- **Pruebas:** Agregar tests unitarios para nueva funcionalidad
- **Migraciones:** Nombrar migraciones descriptivamente

### Proceso de Contribución

1. **Desarrollar** tu funcionalidad
2. **Escribir tests** apropiados  
3. **Ejecutar todas las pruebas:**
   ```bash
   dotnet test
   ```
4. **Commit con mensaje descriptivo:**
   ```bash
   git commit -m "feat: agregar validación de tesis dirigidas"
   ```
5. **Push a tu fork:**
   ```bash
   git push origin feature/nueva-funcionalidad
   ```
6. **Crear Pull Request** en GitHub

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles completos.

## 👥 Equipo de Desarrollo

- **Arquitectura:** .NET 9 + SQL Server + Blazor Server
- **Patrón:** Clean Architecture con DDD
- **Frontend:** Blazor Server + CSS personalizado
- **Backend:** ASP.NET Core Web API + Entity Framework Core
- **Base de Datos:** SQL Server con migraciones automáticas
- **Autenticación:** JWT Bearer tokens
- **Documentación:** Swagger/OpenAPI

## 🆘 Soporte y Contacto

### 📞 Canales de Soporte

- **Issues en GitHub:** Para reportar bugs o solicitar features
- **Documentación:** README.md y comentarios en código
- **Wiki:** Documentación técnica detallada (si está disponible)

### 🐛 Reportar Problemas

Al reportar un problema, incluye:

1. **Versión del sistema operativo**
2. **Versión de .NET instalada** (`dotnet --version`)
3. **Pasos para reproducir el error**
4. **Mensaje de error completo**
5. **Capturas de pantalla** (si aplica)
6. **Logs relevantes** de la aplicación

### 💡 Solicitar Nuevas Características

Para solicitar nuevas funcionalidades:

1. **Verifica** que no existe un issue similar
2. **Describe** claramente la funcionalidad deseada
3. **Explica** el caso de uso y beneficio
4. **Proporciona** mockups o ejemplos si es posible

---

## 🎉 ¡Gracias por usar SIGAD!

Este sistema ha sido desarrollado para facilitar la gestión académica y promover la excelencia docente en instituciones educativas.

**📧 Contacto:** Para consultas específicas sobre implementación o personalización, crear un issue en GitHub con la etiqueta correspondiente.

**🌟 ¡No olvides dejar una estrella si este proyecto te fue útil!**