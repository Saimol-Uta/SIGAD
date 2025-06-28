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


### 2️⃣ Configurar la Base de Datos

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

# Navegar a la carpeta del proyecto API

* Eliminar los archivos de migración si existen
* Abrir Consola del administrador de paquetes de Visual Studio 2022
* cambiar el proyecto predeterminado a SIGAD.infrastructure
* Ejecutar los siguientes comandos:

```bash
Add-Migration InitialSchema -StartupProject SIGAD.WebAPI

Update-Database -StartupProject SIGAD.WebAPI
```

#### 🔧 Solución de Problemas con Migraciones

Si encuentras errores con las migraciones:

* Eliminar el contenido en la carpeta `SIGAD.Infrastructure\Migrations`
* Asegúrate de que la cadena de conexión en `appsettings.Development.json` sea correcta
* Eliminar la base de datos SISTEMA_DOCENTES en local
* volver a ejecutar los comandos de migración

```bash
Add-Migration InitialSchema -StartupProject SIGAD.WebAPI

Update-Database -StartupProject SIGAD.WebAPI
```

### 6️⃣ Ejecutar la Aplicación

#### Ejecutar solo la API

* Selecciona la carpeta `SIGAD.WebAPI` y establece como proyecto de inicio en Visual Studio o ejecuta desde la terminal:
```bash
# Desde SIGAD.WebAPI
dotnet run

# O con hot reload para desarrollo
dotnet watch run
```
#### Ejecutar Usando Visual Studio

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
