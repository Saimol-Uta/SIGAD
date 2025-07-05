# Script para gestión completa de migraciones
#[ESTO ELIMINA EL CONTENEDOR Y VOLUMEN DE LA BASE DE DATOS]
# Uso: .\reset-db.ps1

Write-Host "🔄 Reiniciando base de datos SIGAD..." -ForegroundColor Yellow
Write-Host "⚠️  ADVERTENCIA: Esto eliminará TODOS los datos existentes" -ForegroundColor Red

$confirmation = Read-Host "¿Estás seguro? Escribe 'CONFIRMAR' para continuar"
if ($confirmation -ne "CONFIRMAR") {
    Write-Host "❌ Operación cancelada" -ForegroundColor Yellow
    exit 0
}

Write-Host "🛑 Deteniendo servicios..." -ForegroundColor Yellow
docker-compose stop sigad-database

Write-Host "🗑️  Eliminando contenedor y volumen..." -ForegroundColor Yellow
docker rm sigad-database -f 2>$null
docker volume rm sigad_sigad-database-data -f 2>$null

Write-Host "🗑️  Eliminando migraciones existentes..." -ForegroundColor Yellow
if (Test-Path "SIGAD.Infrastructure\Migrations") {
    Remove-Item "SIGAD.Infrastructure\Migrations\*" -Force -Recurse
    Write-Host "✅ Migraciones eliminadas" -ForegroundColor Green
}

Write-Host "📝 Creando migración inicial..." -ForegroundColor Yellow
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    mcr.microsoft.com/dotnet/sdk:9.0 `
    bash -c "dotnet tool install --global dotnet-ef && export PATH=`"`$PATH:/root/.dotnet/tools`" && dotnet ef migrations add InitialSchema --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al crear la migración inicial" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Levantando base de datos..." -ForegroundColor Yellow
docker-compose up -d sigad-database

Write-Host "⏳ Esperando que la base de datos esté lista..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

Write-Host "📊 Aplicando migración inicial..." -ForegroundColor Yellow
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    -e ASPNETCORE_ENVIRONMENT=Docker `
    mcr.microsoft.com/dotnet/sdk:9.0 `
    bash -c "dotnet tool install --global dotnet-ef && export PATH=`"`$PATH:/root/.dotnet/tools`" && dotnet ef database update --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Base de datos reiniciada exitosamente!" -ForegroundColor Green
    Write-Host "🚀 Levantando todos los servicios..." -ForegroundColor Yellow
    docker-compose up -d
    Write-Host "🎯 Sistema SIGAD listo!" -ForegroundColor Cyan
} else {
    Write-Host "❌ Error al aplicar la migración" -ForegroundColor Red
    exit 1
}
