# Script para crear una nueva migración
# Uso: .\migrate-create.ps1 "NombreDeLaMigracion"

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "📝 Creando migración: $MigrationName" -ForegroundColor Green

# Ejecutar usando una imagen SDK temporal con EF tools
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    -e ConnectionStrings__DefaultConnection="Server=sigad-database;Database=SISTEMA_DOCENTES;User Id=SA;Password=SIGAD123456!;TrustServerCertificate=True;Encrypt=False;" `
    mcr.microsoft.com/dotnet/sdk:9.0 `
    bash -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef migrations add `"$MigrationName`" --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migración '$MigrationName' creada exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al crear la migración" -ForegroundColor Red
}
