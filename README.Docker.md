# SIGAD - Dockerización

Esta documentación describe cómo ejecutar SIGAD usando Docker con una arquitectura de 3 contenedores.

## Arquitectura de Contenedores

### 1. **sigad-database** (SQL Server)
- **Imagen**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Puerto**: `1433:1433`
- **Bases de datos**: SISTEMA_DOCENTES, SGTH, SUT, DITIC
- **Credenciales**: SA / SIGAD123456!

### 2. **sigad-webapi** (Backend - Clean Architecture)
- **Capas incluidas**: Domain, Application, Infrastructure, WebAPI
- **Puerto**: `5217:5217` (HTTP)
- **Swagger**: http://localhost:5217/swagger

### 3. **sigad-blazorapp** (Frontend)
- **Tecnología**: Blazor WebAssembly
- **Puerto**: `5250:5250` (HTTP)
- **URL**: http://localhost:5250

## Requisitos Previos

- Docker Desktop instalado
- Docker Compose
- Al menos 4GB de RAM disponible
- Puertos 1433, 5217, 5250 libres

### Usando Docker Compose Directamente

```bash
# Construir y ejecutar todos los servicios
docker-compose up --build -d

# Aplicar migraciones a la base de datos (REQUERIDO la primera vez)
.\migrate-update.ps1

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down

# Reiniciar un servicio específico
docker-compose restart sigad-webapi
```

## Gestión de Base de Datos

### Scripts de Migración
```powershell
# Aplicar migraciones pendientes (usar después de docker-compose up)
.\migrate-update.ps1

# Crear nueva migración
.\migrate-create.ps1 "NombreDeLaMigracion"

# Eliminar última migración
.\migrate-remove.ps1
```

**Importante**: Después de ejecutar `docker-compose up` por primera vez, debes ejecutar `.\migrate-update.ps1` para crear la base de datos SISTEMA_DOCENTES.

## Configuración de Red

Los contenedores se comunican a través de la red `sigad-network`:

- **Base de datos**: `sigad-database:1433`
- **API Backend**: `sigad-webapi:5217`
- **Frontend**: `sigad-blazorapp:5250`

## Volúmenes

- **sigad-database-data**: Datos persistentes de SQL Server
- **sigad-uploads**: Archivos subidos por usuarios

## URLs de Acceso

- **Frontend**: http://localhost:5250
- **API**: http://localhost:5217
- **Swagger**: http://localhost:5217/swagger
- **Base de datos**: localhost:1433

## Credenciales por Defecto

### Base de Datos
- **Usuario**: SA
- **Contraseña**: SIGAD123456!

### Aplicación
Las credenciales de la aplicación dependen de los datos iniciales en la base de datos.

## Solución de Problemas

### 1. Error de conexión a base de datos
```bash
# Verificar que SQL Server esté ejecutándose
docker-compose logs sigad-database

# Verificar conectividad
docker exec sigad-database /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'SIGAD123456!' -Q 'SELECT 1'
```

### 2. Problemas de certificados SSL
Los contenedores están configurados para usar HTTP solamente. Para HTTPS en producción, usar el archivo docker-compose.prod.yml.

### 3. Puertos ocupados
```bash
# Verificar puertos en uso
netstat -an | findstr :5217
netstat -an | findstr :5250
netstat -an | findstr :1433
```

### 4. Reiniciar base de datos
```bash
# Windows
docker-manager.bat db-reset

## Comandos de Mantenimiento

### Limpiar todo el sistema
```bash
# Detener y eliminar todo
docker-compose down -v

# Limpiar imágenes no utilizadas
docker system prune -f

# Limpiar volúmenes
docker volume prune -f
```

### Logs específicos
```bash
# Logs de la base de datos
docker-compose logs -f sigad-database

# Logs del API
docker-compose logs -f sigad-webapi

# Logs del frontend
docker-compose logs -f sigad-blazorapp
```

## Desarrollo

### Reconstruir después de cambios
```bash
# Reconstruir solo el backend
docker-compose build sigad-webapi

# Reconstruir solo el frontend
docker-compose build sigad-blazorapp

# Reconstruir todo
docker-compose build --no-cache
```

### Conectar a la base de datos externamente
```bash
# Usando SQL Server Management Studio
Server: localhost,1433
User: SA
Password: SIGAD123456!
```

## Estructura de Archivos Docker

```
├── docker-compose.yml          # Configuración principal
├── .dockerignore              # Archivos a ignorar
├── docker-manager.bat         # Script de gestión (Windows)
├── docker-manager.sh          # Script de gestión (Linux/macOS)
│
├── SIGAD.WebAPI/
│   ├── Dockerfile            # Imagen del backend
│   └── appsettings.Docker.json
│
├── SIGAD.BlazorApp/
│   ├── Dockerfile            # Imagen del frontend
│   └── appsettings.Docker.json
│
└── Database/
    └── Scripts/
        └── init-database.sh  # Script de inicialización
```
