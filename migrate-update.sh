#!/bin/bash

echo -e "\e[32m🔄 Aplicando migraciones a la base de datos...\e[0m"

# Verificar que el contenedor de la base de datos esté disponible
if ! docker ps -q -f name=sigad-database >/dev/null; then
    echo -e "\e[31m❌ Error: El contenedor sigad-database no está ejecutándose. Ejecuta 'docker-compose up -d' primero.\e[0m"
    exit 1
fi

# Ejecutar usando imagen SDK temporal con EF tools
docker run --rm -it --network sigad_sigad-network \
    -v "${PWD}:/src" -w /src \
    -e ASPNETCORE_ENVIRONMENT=Docker \
    -e ConnectionStrings__DefaultConnection="Server=sigad-database;Database=SISTEMA_DOCENTES;User Id=SA;Password=SIGAD123456!;TrustServerCertificate=True;Encrypt=False;" \
    mcr.microsoft.com/dotnet/sdk:9.0 \
    bash -c "dotnet tool install --global dotnet-ef && /root/.dotnet/tools/dotnet-ef database update --project 'SIGAD.Infrastructure' --startup-project 'SIGAD.WebAPI'"

if [ $? -eq 0 ]; then
    echo -e "\e[32m✅ Migraciones aplicadas exitosamente!\e[0m"
else
    echo -e "\e[31m❌ Error al aplicar las migraciones\e[0m"
fi