# Script para eliminar la última migración
# Uso: .\migrate-remove.ps1

Write-Host "🗑️ Eliminando la última migración..." -ForegroundColor Yellow

# Ejecutar usando imagen SDK temporal con EF tools
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    -e ConnectionStrings__DefaultConnection="Server=sigad-database;Database=SISTEMA_DOCENTES;User Id=SA;Password=SIGAD123456!;TrustServerCertificate=True;Encrypt=False;" `
    mcr.microsoft.com/dotnet/sdk:9.0 `
    bash -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef migrations remove --project SIGAD.Infrastructure --startup-project SIGAD.WebAPI"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Última migración eliminada exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al eliminar la migración" -ForegroundColor Red
}
