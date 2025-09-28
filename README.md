# SIGAD - Sistema de Gestión de Acción de Personal

Sistema desarrollado con arquitectura Clean Architecture usando .NET 9, Blazor WebAssembly y SQL Server, completamente dockerizado para facilitar el desarrollo y despliegue.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
   - [Arquitectura y SOLID (detallado)](README.Arquitectura-SOLID.md)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejecución del Proyecto](#ejecución-del-proyecto)
- [Gestión de Base de Datos](#gestión-de-base-de-datos)
- [Scripts de Migración](#scripts-de-migración)
- [URLs de Acceso](#urls-de-acceso)
- [Solución de Problemas](#solución-de-problemas)
- [Desarrollo](#desarrollo)

## 🎯 Descripción

SIGAD es un sistema integral para la gestión de acciones de personal en instituciones educativas. Permite manejar solicitudes de ascenso, generar certificados, y administrar el proceso completo de promoción docente siguiendo los lineamientos institucionales.

### Características principales:
- 📝 Gestión de solicitudes de ascenso docente
- 📊 Evaluación y seguimiento de expedientes
- 📋 Generación automática de certificados
- 👥 Sistema de roles y permisos
- 🔐 Autenticación JWT
- 📁 Gestión de documentos y archivos

## 🏗️ Arquitectura

Para una explicación detallada de la arquitectura, la aplicación de principios SOLID y las refactorizaciones sugeridas, consulta: [README.Arquitectura-SOLID.md](README.Arquitectura-SOLID.md).

## 🏗️ Arquitectura Detallada

### Clean Architecture Implementation

```
┌─────────────────────────────────────────────────────────────┐
│                    SIGAD.BlazorApp                          │
│                   (Presentation Layer)                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │   Pages     │  │ Components  │  │   Layout    │       │
│  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTP/SignalR
┌─────────────────────────▼───────────────────────────────────┐
│                    SIGAD.WebAPI                             │
│                (Interface Adapters)                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │Controllers  │  │ Middleware  │  │   Models    │       │
│  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────┬───────────────────────────────────┘
                          │ Dependency Injection
┌─────────────────────────▼───────────────────────────────────┐
│                  SIGAD.Application                          │
│                 (Application Layer)                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │  Services   │  │    DTOs     │  │ Interfaces  │       │
│  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────┬───────────────────────────────────┘
                          │ Abstractions
┌─────────────────────────▼───────────────────────────────────┐
│                SIGAD.Infrastructure                         │
│               (Infrastructure Layer)                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │Repositories │  │ Migrations  │  │  Services   │       │
│  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────┬───────────────────────────────────┘
                          │ Implementation
┌─────────────────────────▼───────────────────────────────────┐
│                    SIGAD.Domain                             │
│                   (Domain Layer)                           │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐       │
│  │  Entities   │  │    Enums    │  │    Rules    │       │
│  └─────────────┘  └─────────────┘  └─────────────┘       │
└─────────────────────────────────────────────────────────────┘
```

### Flujo de Datos

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   Browser   │───▶│ BlazorApp   │───▶│   WebAPI    │───▶│ Application │
│             │    │ (Client)    │    │ (Gateway)   │    │ (Business)  │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
                                                                 │
┌─────────────┐    ┌─────────────┐    ┌─────────────┐           │
│ SQL Server  │◀───│Infrastructure│◀───│   Domain    │◀──────────┘
│ (Database)  │    │ (Data)      │    │ (Entities)  │
└─────────────┘    └─────────────┘    └─────────────┘
```


### Estructura del Proyecto

```
SIGAD/
├── docker-compose.yml              # Configuración Docker principal
├── migrate-create.ps1              # Script para crear migraciones
├── migrate-update.ps1              # Script para aplicar migraciones
│
├── SIGAD.Domain/                  # Capa de dominio (Entidades, Enums)
├── SIGAD.Application/             # Capa de aplicación (DTOs, Servicios)
├── SIGAD.Infrastructure/          # Capa de infraestructura (Repositorios, BD)
├── SIGAD.WebAPI/                  # API REST con Swagger
└── SIGAD.BlazorApp/              # Frontend Blazor WebAssembly
```

### Contenedores Docker

| Contenedor | Imagen | Puerto | Descripción |
|------------|--------|--------|-------------|
| `sigad-database` | SQL Server 2022 | 1433 | Base de datos principal |
| `sigad-webapi` | .NET 9 Runtime | 5217 | API Backend |
| `sigad-blazorapp` | .NET 9 Runtime | 5250 | Frontend Blazor |

### Volúmenes Persistentes

| Volumen | Propósito |
|---------|-----------|
| `sigad-database-data` | Datos de SQL Server (tablas, registros) |
| `sigad-uploads` | Archivos subidos por usuarios |

## 📋 Requisitos Previos

### Software Necesario

1. **Docker Desktop** (versión 4.0 o superior)
   - [Descargar para Windows](https://www.docker.com/products/docker-desktop)
   - [Descargar para macOS](https://www.docker.com/products/docker-desktop)
   - [Descargar para Linux](https://docs.docker.com/desktop/linux/)

2. **Git** (para clonar el repositorio)
   - [Descargar Git](https://git-scm.com/downloads)

3. **PowerShell** (para ejecutar los scripts)
   - Windows: Instalar la última versión desde [Microsoft Store](https://www.microsoft.com/store/productId/9MZ1SNWT0N5D)
   - macOS/Linux: [Instalar PowerShell Core](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell)

### Recursos del Sistema

- **RAM**: Mínimo 4GB, recomendado 8GB
- **Espacio en disco**: Mínimo 5GB libres
- **Puertos disponibles**: 1433, 5217, 5250

## 🚀 Instalación y Configuración

### Paso 1: Clonar el Repositorio

```bash
git clone [URL_DEL_REPOSITORIO]
cd SIGAD
```

### Paso 2: Verificar Docker

```bash
# Verificar que Docker esté instalado y funcionando
docker --version
docker-compose --version

# Verificar que Docker Desktop esté ejecutándose
docker info
```

### Paso 3: Configuración Inicial

```powershell
# 1. Construir y levantar contenedores
docker-compose up --build -d

# 2. Esperar a que los servicios estén listos (30-60 segundos)
Start-Sleep -Seconds 30

# 3. Aplicar migraciones de base de datos
.\migrate-update.ps1
```

## ▶️ Ejecución del Proyecto

### Comandos Básicos

```powershell
# Levantar todos los servicios
docker-compose up -d

# Ver estado de los contenedores
docker-compose ps

# Ver logs en tiempo real
docker-compose logs -f

# Detener todos los servicios
docker-compose down

# Reiniciar un servicio específico
docker-compose restart sigad-webapi
```

### Verificar que Todo Funcione

1. **Verificar contenedores activos:**
   ```powershell
   docker-compose ps
   ```
   Deberías ver 3 contenedores en estado "Up"

2. **Verificar acceso a la API:**
   ```powershell
   curl http://localhost:5217/swagger
   ```

3. **Verificar frontend:**
   Abrir http://localhdf -host:5250 en el navegador

## 🗄️ Gestión de Base de Datos

### Entity Framework Migrations

El proyecto usa Entity Framework Core para manejar la base de datos mediante migraciones.

### Bases de Datos

| Base de Datos | Propósito | Ubicación |
|---------------|-----------|-----------|
| `SISTEMA_DOCENTES` | Principal (dockerizada) | Contenedor SQL Server |
| `SGTH`, `SUT`, `DITIC` | Externas (configurables) | Servidores externos |

## 📝 Scripts de Migración

### migrate-create.ps1

**Propósito:** Crear una nueva migración cuando cambias el modelo de datos.

**Uso:**
```powershell
.\migrate-create.ps1 "NombreDeLaMigracion"
```

**Ejemplos:**
```powershell
# Crear migración para nueva tabla
.\migrate-create.ps1 "AgregarTablaEvaluaciones"

# Crear migración para modificar columna
.\migrate-create.ps1 "ModificarColumnaEstado"

# Crear migración inicial
.\migrate-create.ps1 "InitialSchema"
```

**¿Cuándo usar?**
- ✅ Agregas una nueva entidad (tabla)
- ✅ Modificas propiedades de una entidad existente
- ✅ Cambias relaciones entre entidades
- ✅ Agregas o quitas índices

### migrate-update.ps1

**Propósito:** Aplicar las migraciones pendientes a la base de datos.

**Uso:**
```powershell
.\migrate-update.ps1
```

**¿Qué hace?**
1. 🔍 Verifica que el contenedor de base de datos esté ejecutándose
2. 📦 Descarga herramientas de Entity Framework en un contenedor temporal
3. 🔄 Aplica todas las migraciones pendientes
4. ✅ Actualiza la base de datos con los cambios

**¿Cuándo usar?**
- ✅ Primera vez que ejecutas el proyecto
- ✅ Después de descargar cambios de Git que incluyen nuevas migraciones
- ✅ Después de crear una nueva migración con `migrate-create.ps1`
- ✅ Cuando otros desarrolladores han agregado migraciones

### Flujo de Trabajo Típico

```powershell
# 1. Hacer cambios en el modelo (entidades)
# Ejemplo: Agregar nueva propiedad a una entidad

# 2. Crear migración
.\migrate-create.ps1 "AgregarNuevaPropiedad"

# 3. Aplicar migración
.\migrate-update.ps1

# 4. Verificar que funciona
# Abrir la aplicación y probar los cambios
```

## 🌐 URLs de Acceso

### Desarrollo Local

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Frontend** | http://localhost:5250 | Aplicación Blazor WebAssembly |
| **API** | http://localhost:5217 | API REST Backend |
| **Swagger** | http://localhost:5217/swagger | Documentación interactiva de la API |
| **Base de Datos** | localhost:1433 | SQL Server (usar SQL Server Management Studio) |

### Credenciales por Defecto

#### Base de Datos
- **Servidor:** localhost,1433
- **Usuario:** SA
- **Contraseña:** SIGAD123456!
- **Base de Datos:** SISTEMA_DOCENTES

#### Aplicación
Las credenciales de la aplicación dependen de los datos iniciales cargados en la base de datos.

## 🛠️ Solución de Problemas

### Error: "Docker no está ejecutándose"

```powershell
# Error típico:
# docker: error during connect: Head "http://...": open //./pipe/dockerDesktopLinuxEngine: El sistema no puede encontrar el archivo especificado

# Solución:
# 1. Abrir Docker Desktop
# 2. Esperar a que inicie completamente
# 3. Verificar con: docker info
```

### Error: "Puerto ocupado"

```powershell
# Verificar qué está usando los puertos
netstat -an | findstr :5217
netstat -an | findstr :5250
netstat -an | findstr :1433

# Solución: Detener el proceso que usa el puerto o cambiar el puerto en docker-compose.yml
```

### Error: "No se puede conectar a la base de datos"

```powershell
# 1. Verificar que el contenedor esté funcionando
docker-compose ps

# 2. Ver logs del contenedor de base de datos
docker-compose logs sigad-database

# 3. Reiniciar el contenedor
docker-compose restart sigad-database

# Si necesitas un reset completo:
docker-compose down -v
docker-compose up --build -d
.\migrate-update.ps1
```

### Error: "Migración ya aplicada"

```powershell
# Error típico al usar migrate-remove.ps1:
# "The migration 'XXXX' has already been applied to the database"

# Solución: Es normal, significa que la base de datos está actualizada
# Si necesitas un reset completo:
.\reset-db.ps1
```

### Error: "No se encuentra archivo de migración"

```powershell
# Si no tienes migraciones iniciales:
# 1. Crear migración inicial
.\migrate-create.ps1 "InitialSchema"

# 2. Aplicar migración
.\migrate-update.ps1
```

### Limpiar Todo el Sistema

```powershell
# Detener y eliminar todo (CUIDADO: Elimina todos los datos)
docker-compose down -v

# Limpiar imágenes no utilizadas
docker system prune -f

# Limpiar volúmenes no utilizados
docker volume prune -f

# Empezar desde cero
docker-compose up --build -d
.\migrate-update.ps1
```

## 👥 Desarrollo

### Flujo de Trabajo para Desarrolladores

#### Configuración Inicial (Una vez)

1. **Clonar y configurar:**
   ```bash
   git clone [URL_DEL_REPOSITORIO]
   cd SIGAD
   docker-compose up --build -d
   .\migrate-update.ps1
   ```

#### Desarrollo Diario

1. **Iniciar trabajo:**
   ```powershell
   # Levantar servicios
   docker-compose up -d
   
   # Aplicar nuevas migraciones (si las hay)
   .\migrate-update.ps1
   ```

2. **Hacer cambios en el código**

3. **Si cambias el modelo de datos:**
   ```powershell
   # Crear migración
   .\migrate-create.ps1 "DescripcionDelCambio"
   
   # Aplicar migración
   .\migrate-update.ps1
   ```

4. **Finalizar trabajo:**
   ```powershell
   # Opcional: Detener servicios
   docker-compose down
   ```

### Reconstruir Después de Cambios

```powershell
# Si cambias código del backend
docker-compose build sigad-webapi
docker-compose up -d sigad-webapi

# Si cambias código del frontend
docker-compose build sigad-blazorapp
docker-compose up -d sigad-blazorapp

# Reconstruir todo
docker-compose build --no-cache
docker-compose up -d
```

### Colaboración en Equipo

#### Al Recibir Cambios de Git

```powershell
# 1. Descargar cambios
git pull

# 2. Reconstruir si hay cambios en Dockerfile
docker-compose build

# 3. Aplicar nuevas migraciones
.\migrate-update.ps1

# 4. Levantar servicios
docker-compose up -d
```

#### Al Enviar Cambios

```powershell
# 1. Asegurarse de que las migraciones estén incluidas
git add SIGAD.Infrastructure/Migrations/

# 2. Commit y push
git commit -m "Agregar migración para [descripción]"
git push
```

### Conectar a la Base de Datos Externamente

#### SQL Server Management Studio (SSMS)

- **Server name:** localhost,1433
- **Authentication:** SQL Server Authentication
- **Login:** SA
- **Password:** SIGAD123456!

#### Azure Data Studio

```json
{
  "server": "localhost,1433",
  "database": "SISTEMA_DOCENTES",
  "user": "SA",
  "password": "SIGAD123456!",
  "trustServerCertificate": true
}
```

## 📚 Recursos Adicionales

### Documentación

- [Entity Framework Core Migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Blazor WebAssembly](https://docs.microsoft.com/en-us/aspnet/core/blazor/)

### Estructura de Archivos

```
SIGAD/
├── README.md                      # Este archivo
├── docker-compose.yml            # Configuración Docker principal
├── migrate-create.ps1            # Script para crear migraciones
├── migrate-update.ps1            # Script para aplicar migraciones
│
├── SIGAD.Domain/                 # 🏗️ Capa de Dominio
│   ├── Entities/                 # Entidades del negocio
│   ├── Enums/                    # Enumeraciones
│   └── Interfaces/               # Interfaces de dominio
│
├── SIGAD.Application/            # 📋 Capa de Aplicación
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Interfaces de servicios
│   └── Services/                 # Servicios de aplicación
│
├── SIGAD.Infrastructure/         # 🔧 Capa de Infraestructura
│   ├── Migrations/               # Migraciones de EF Core
│   ├── Persistence/              # Configuración de DbContext
│   ├── Repositories/             # Implementación de repositorios
│   └── Services/                 # Servicios de infraestructura
│
├── SIGAD.WebAPI/                 # 🌐 API REST
│   ├── Controllers/              # Controladores de API
│   ├── Middleware/               # Middleware personalizado
│   ├── Templates/                # Plantillas de documentos
│   └── uploads/                  # Archivos subidos
│
└── SIGAD.BlazorApp/             # 🎨 Frontend Blazor
    ├── Components/               # Componentes reutilizables
    ├── Layout/                   # Layouts de página
    ├── Pages/                    # Páginas de la aplicación
    └── Services/                 # Servicios de frontend
```

---

## 📝 Notas Importantes

- ⚠️ **Datos persistentes:** Los datos se almacenan en volúmenes Docker y se mantienen entre reinicios
- 🔄 **Migraciones automáticas:** Siempre ejecuta `migrate-update.ps1` después de cambios en el modelo
- 🚀 **Primera ejecución:** Usa `docker-compose up --build -d` y `.\migrate-update.ps1` para configuración inicial
- 👥 **Colaboración:** Comparte cambios de migración a través de Git
- 🐳 **Docker:** Asegúrate de que Docker Desktop esté ejecutándose antes de usar cualquier script

---

**Desarrollado con ❤️ para la gestión eficiente de acción de personal**
