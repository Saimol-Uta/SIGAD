# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file first for better caching
COPY SIGAD.sln .

# Copy all project files to resolve dependencies
COPY SIGAD.Domain/SIGAD.Domain.csproj SIGAD.Domain/
COPY SIGAD.Application/SIGAD.Application.csproj SIGAD.Application/
COPY SIGAD.BlazorApp/SIGAD.BlazorApp.csproj SIGAD.BlazorApp/

# Restore dependencies
RUN dotnet restore SIGAD.BlazorApp/SIGAD.BlazorApp.csproj

# Copy source code
COPY SIGAD.Domain/ SIGAD.Domain/
COPY SIGAD.Application/ SIGAD.Application/
COPY SIGAD.BlazorApp/ SIGAD.BlazorApp/

# Build and publish the application
RUN dotnet publish SIGAD.BlazorApp/SIGAD.BlazorApp.csproj -c Release -o /app/publish

# Runtime stage - Use nginx for static files
FROM nginx:alpine AS runtime

# Copy published wwwroot to nginx
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Create nginx configuration
RUN echo 'server { \
    listen 5250; \
    server_name localhost; \
    root /usr/share/nginx/html; \
    index index.html; \
    \
    location / { \
        try_files $uri $uri/ /index.html; \
    } \
    \
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ { \
        expires 1y; \
        add_header Cache-Control "public, immutable"; \
    } \
    \
    location ~* \.(json)$ { \
        add_header Cache-Control "no-cache"; \
    } \
    \
    location ~* \.(dll|wasm)$ { \
        add_header Content-Type "application/wasm"; \
    } \
}' > /etc/nginx/conf.d/default.conf

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf.dpkg-dist 2>/dev/null || true

# Expose port
EXPOSE 5250

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
