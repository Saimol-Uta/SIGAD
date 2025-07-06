# Script para aplicar migraciones pendientes
# Uso: .\migrate-update.ps1

Write-Host "🔄 Aplicando migraciones a la base de datos..." -ForegroundColor Green

# Verificar que el contenedor del WebAPI esté disponible
if (-not (docker ps -q -f name=sigad-database)) {
    Write-Host "❌ Error: El contenedor sigad-database no está ejecutándose. Ejecuta 'docker-compose up -d' primero." -ForegroundColor Red
    exit 1
}

# Ejecutar usando imagen SDK temporal con EF tools
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    -e ASPNETCORE_ENVIRONMENT=Docker `
    -e ConnectionStrings__DefaultConnection="Server=sigad-database;Database=SISTEMA_DOCENTES;User Id=SA;Password=SIGAD123456!;TrustServerCertificate=True;Encrypt=False;" `
    mcr.microsoft.com/dotnet/sdk:9.0 `
    bash -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef database update --project SIGAD.Infrastructure --startup-project SIGAD.WebAPI"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migraciones aplicadas exitosamente!" -ForegroundColor Green
} else {
    Write-Host "❌ Error al aplicar las migraciones" -ForegroundColor Red
}
