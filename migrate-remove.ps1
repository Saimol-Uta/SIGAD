# Script para eliminar la última migración
# Uso: .\migrate-remove.ps1

Write-Host "🗑️ Eliminando la última migración..." -ForegroundColor Yellow

# Ejecutar usando el contenedor con EF tools ya instalado
docker run --rm -it --network sigad_sigad-network `
    -v "${PWD}:/src" -w /src `
    sigad-sigad-webapi:latest `
    bash -c "export PATH=`"`$PATH:/root/.dotnet/tools`" && dotnet ef migrations remove --project `"SIGAD.Infrastructure`" --startup-project `"SIGAD.WebAPI`""

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Última migración eliminada exitosamente" -ForegroundColor Green
} else {
    Write-Host "❌ Error al eliminar la migración" -ForegroundColor Red
}
