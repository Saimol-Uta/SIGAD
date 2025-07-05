# Script para aplicar migraciones pendientes
# Uso: .\migrate-update.ps1

Write-Host "🔄 Aplicando migraciones a la base de datos..." -ForegroundColor Green

# Verificar que el contenedor del WebAPI esté disponible
if (-not (docker images -q sigad-sigad-webapi:latest)) {
    Write-Host "❌ Error: La imagen sigad-sigad-webapi no existe. Ejecuta 'docker-compose build' primero." -ForegroundColor Red
    exit 1
}

# Ejecutar usando el contenedor con EF tools ya instalado
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    -e ASPNETCORE_ENVIRONMENT=Docker `
    sigad-sigad-webapi:latest `
    bash -c "export PATH=`"`$PATH:/root/.dotnet/tools`" && dotnet ef database update --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migraciones aplicadas exitosamente!" -ForegroundColor Green
} else {
    Write-Host "❌ Error al aplicar las migraciones" -ForegroundColor Red
}
