# Script para crear una nueva migración
# Uso: .\migrate-create.ps1 "NombreDeLaMigracion"

param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

Write-Host "📝 Creando migración: $MigrationName" -ForegroundColor Green

# Ejecutar usando el contenedor con EF tools ya instalado
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    sigad-sigad-webapi:latest `
    bash -c "export PATH=`"`$PATH:/root/.dotnet/tools`" && dotnet ef migrations add $MigrationName --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migración '$MigrationName' creada exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al crear la migración" -ForegroundColor Red
}
