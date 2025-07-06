# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install Entity Framework tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy solution file
COPY SIGAD.sln .

# Copy project files
COPY SIGAD.Domain/SIGAD.Domain.csproj SIGAD.Domain/
COPY SIGAD.Application/SIGAD.Application.csproj SIGAD.Application/
COPY SIGAD.Infrastructure/SIGAD.Infrastructure.csproj SIGAD.Infrastructure/
COPY SIGAD.WebAPI/SIGAD.WebAPI.csproj SIGAD.WebAPI/

# Restore dependencies
RUN dotnet restore SIGAD.WebAPI/SIGAD.WebAPI.csproj

# Copy source code
COPY SIGAD.Domain/ SIGAD.Domain/
COPY SIGAD.Application/ SIGAD.Application/
COPY SIGAD.Infrastructure/ SIGAD.Infrastructure/
COPY SIGAD.WebAPI/ SIGAD.WebAPI/

# Build the application
RUN dotnet build SIGAD.WebAPI/SIGAD.WebAPI.csproj -c Release -o /app/build

# Publish the application
RUN dotnet publish SIGAD.WebAPI/SIGAD.WebAPI.csproj -c Release -o /app/publish

# Runtime stage - Using SDK instead of runtime for EF tools
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /app

# Install Entity Framework tools globally
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy published application
COPY --from=build /app/publish .

# Create directories for uploads and templates
RUN mkdir -p /app/uploads/investigaciones && \
    mkdir -p /app/uploads/articulos && \
    mkdir -p /app/uploads/cursos && \
    mkdir -p /app/uploads/experiencias && \
    mkdir -p /app/uploads/evaluaciones && \
    mkdir -p /app/uploads/tesis && \
    mkdir -p /app/uploads/acciones_personal && \
    mkdir -p /app/Templates

# Set permissions
RUN chmod -R 755 /app/uploads

# Copy Docker-specific configuration
COPY SIGAD.WebAPI/appsettings.Docker.json /app/appsettings.Docker.json

# Set environment variable
ENV ASPNETCORE_ENVIRONMENT=Docker

# Expose ports
EXPOSE 5217

# Set entry point
ENTRYPOINT ["dotnet", "SIGAD.WebAPI.dll"]
