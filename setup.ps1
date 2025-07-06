# Script de configuración inicial de SIGAD
# Uso: .\setup.ps1

Write-Host "🚀 Configurando SIGAD..." -ForegroundColor Green

# Verificar que Docker esté instalado
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Docker no está instalado. Por favor instala Docker Desktop." -ForegroundColor Red
    exit 1
}

# Verificar que Docker esté ejecutándose
docker info 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker no está ejecutándose. Por favor inicia Docker Desktop." -ForegroundColor Red
    exit 1
}

Write-Host "🔧 Construyendo y levantando contenedores..." -ForegroundColor Yellow
docker-compose up --build -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al levantar los contenedores" -ForegroundColor Red
    exit 1
}

Write-Host "⏳ Esperando a que los servicios estén listos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "📊 Aplicando migraciones a la base de datos..." -ForegroundColor Yellow
.\migrate-update.ps1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ ¡SIGAD configurado exitosamente!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 URLs disponibles:" -ForegroundColor Cyan
    Write-Host "   Frontend: http://localhost:5250" -ForegroundColor White
    Write-Host "   API: http://localhost:5217" -ForegroundColor White
    Write-Host "   Swagger: http://localhost:5217/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Comandos útiles:" -ForegroundColor Cyan
    Write-Host "   Ver logs: docker-compose logs -f" -ForegroundColor White
    Write-Host "   Detener: docker-compose down" -ForegroundColor White
    Write-Host "   Reiniciar: docker-compose restart" -ForegroundColor White
} else {
    Write-Host "❌ Error al configurar la base de datos" -ForegroundColor Red
    Write-Host "Verifica los logs con: docker-compose logs" -ForegroundColor Yellow
}
