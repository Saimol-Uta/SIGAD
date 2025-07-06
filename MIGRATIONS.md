# 🚀 SIGAD - Migraciones Docker Optimizadas

Este sistema utiliza **contenedores Docker optimizados** con herramientas EF Core preinstaladas para manejar migraciones de manera eficiente.

## 📋 Scripts Disponibles

### `migrate.ps1` - Script Principal
```powershell
# Aplicar migraciones pendientes
.\migrate.ps1

# Crear nueva migración y aplicarla
.\migrate.ps1 -Create "NombreDeLaMigracion"
```

### `migrate-create.ps1` - Solo Crear Migración
```powershell
.\migrate-create.ps1 "NombreDeLaMigracion"
```

### `migrate-update.ps1` - Solo Aplicar Migraciones
```powershell
.\migrate-update.ps1
```

### `migrate-remove.ps1` - Eliminar Última Migración
```powershell
.\migrate-remove.ps1
```

## ⚡ Ventajas del Sistema Optimizado

- **✅ Rápido**: Usa la imagen Docker existente con EF tools preinstalados
- **✅ Eficiente**: No reinstala herramientas en cada ejecución
- **✅ Consistente**: Misma versión de .NET en desarrollo y producción
- **✅ Red Compartida**: Acceso directo a la base de datos Docker
- **✅ Simplicidad**: Comandos cortos y claros

## 🔧 Requisitos

1. Docker corriendo
2. Imagen `sigad-sigad-webapi` construida (se construye automáticamente si no existe)
3. Red Docker `sigad_sigad-network` activa

## 📝 Ejemplos de Uso

```powershell
# Crear una migración para agregar nueva tabla
.\migrate.ps1 -Create "AgregarTablaReportes"

# Eliminar la última migración si hay errores
.\migrate-remove.ps1
```

## 🚨 Notas Importantes

- Los scripts deben ejecutarse desde la raíz del proyecto (donde está `SIGAD.sln`)
- La base de datos debe estar corriendo (`docker-compose up -d sigad-database`)
- Las migraciones se crean en `SIGAD.Infrastructure/Migrations/`
