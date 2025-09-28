# Arquitectura y Principios SOLID en SIGAD

Este documento describe la arquitectura de SIGAD y cómo se aplican (o deben aplicarse) los principios SOLID en cada capa. Además, propone mejoras y refactorizaciones de bajo riesgo para fortalecer la mantenibilidad, escalabilidad y testabilidad del sistema.

## Visión General

SIGAD implementa Clean Architecture con cinco proyectos:

- `SIGAD.Domain`: Entidades del dominio, enums y reglas inmutables del negocio.
- `SIGAD.Application`: Casos de uso, DTOs, validaciones, puertos (interfaces) hacia afuera.
- `SIGAD.Infrastructure`: Implementaciones de persistencia (EF Core), servicios externos, repositorios.
- `SIGAD.WebAPI`: Endpoints REST, composición de casos de uso, autenticación/autorización, CORS, Swagger.
- `SIGAD.BlazorApp`: Presentación (WASM), estado de autenticación, clientes HTTP y componentes de UI.

Comunicación y dependencias:

- `WebAPI` depende de `Application`, que depende de `Domain`.
- `Infrastructure` implementa interfaces definidas en `Application` y se inyecta en `WebAPI` vía DI.
- `BlazorApp` consume `WebAPI` mediante `HttpClient` usando JWT.

## Comunicación entre capas

Resumen del flujo de comunicación y tecnologías usadas entre capas:

- Capa Presentación (`SIGAD.BlazorApp`)
    - Hace llamadas HTTP a `SIGAD.WebAPI` usando `HttpClient`.
    - Anexa el token JWT mediante un `DelegatingHandler` (`AuthorizationMessageHandler`).
    - Configura la URL base de la API desde `wwwroot/appsettings.json` en `Program.cs`.

- Capa API (`SIGAD.WebAPI`)
    - Expone controladores REST (en `SIGAD.WebAPI/Controllers`) que reciben DTOs del frontend y devuelven DTOs/Resultados.
    - Orquesta casos de uso de `SIGAD.Application` mediante DI.
    - Autentica con JWT (Bearer) y aplica CORS, Swagger, middleware de validación y archivos estáticos (uploads).

- Capa Aplicación (`SIGAD.Application`)
    - Define casos de uso y contratos (interfaces) hacia repositorios/servicios externos.
    - Usa DTOs para comunicar datos hacia/desde la API y mapear con entidades del dominio.

- Capa Dominio (`SIGAD.Domain`)
    - Entidades, enums y reglas internas del negocio (sin dependencias externas).

- Capa Infraestructura (`SIGAD.Infrastructure`)
    - Implementa los contratos de `Application`: repositorios EF Core, servicios externos (correo, almacenamiento), DbContext y migraciones.
    - Expone `SigadDbContext` y repositorios `Ef*Repository` inyectados en `WebAPI`.

Diagrama textual de dependencias:

- Blazor (WASM) → HTTP → WebAPI (Controllers) → Application (Services/Use Cases) → Domain (Entities)
- Application → (Interfaces) ← Infrastructure (EF/External services) → SQL Server

## Funcionamiento por capa y archivos clave

### 1) Presentación: `SIGAD.BlazorApp`
- Responsabilidad: UI, navegación, estado de autenticación, interacción con la API.
- Archivos/ubicaciones relevantes:
    - `SIGAD.BlazorApp/Program.cs`: configuración de DI, `HttpClient` (BaseAddress desde `wwwroot/appsettings.json`), registro de servicios y `AuthorizationMessageHandler`.
    - `SIGAD.BlazorApp/wwwroot/appsettings.json`: clave `ApiBaseUrl` con la URL pública de la API.
    - `SIGAD.BlazorApp/Services/AuthService.cs`: login, registro, manejo de token en LocalStorage.
    - `SIGAD.BlazorApp/Services/AuthorizationMessageHandler.cs`: agrega `Authorization: Bearer {token}` a cada request.
    - `SIGAD.BlazorApp/Services/SolicitudesService.cs`, `ReporteService.cs`, `NotificacionClienteService.cs`: servicios de consumo de endpoints de negocio.
    - `SIGAD.BlazorApp/Pages/*.razor`: componentes de UI; deben llamar a servicios (evitar URLs absolutas).

Flujo de comunicación:
1) Componente `.razor` invoca un método del servicio (p.ej., `AuthService.Login`).
2) Servicio usa `HttpClient` con `BaseAddress` configurada para llamar a `api/...`.
3) `AuthorizationMessageHandler` añade el JWT si está presente en LocalStorage.

### 2) API: `SIGAD.WebAPI`
- Responsabilidad: exponer endpoints REST, autenticar, autorizar y orquestar casos de uso.
- Archivos/ubicaciones relevantes:
    - `SIGAD.WebAPI/Program.cs`: DI de repositorios y servicios, configuración de JWT, CORS, Swagger, middleware y `UseStaticFiles` (`/uploads`).
    - `SIGAD.WebAPI/Controllers/*Controller.cs`: endpoints (e.g., Auth, Solicitudes, Artículos, etc.).
    - `SIGAD.WebAPI/Middleware/*`: validación y manejo de errores centralizados.
    - `SIGAD.WebAPI/appsettings*.json`: cadenas de conexión y `JwtSettings` (Issuer, Audience, SecretKey).

Flujo de comunicación:
1) Recibe request HTTP del frontend.
2) `JwtBearer` valida el token si la ruta lo requiere.
3) Controlador invoca servicios de `Application` usando interfaces inyectadas.
4) Devuelve `ActionResult` con DTO/resultado.

### 3) Aplicación: `SIGAD.Application`
- Responsabilidad: lógica de aplicación (casos de uso), contratos (interfaces) y DTOs.
- Archivos/ubicaciones relevantes:
    - `SIGAD.Application/Interfaces/*`: puertos/contratos (repos, servicios externos).
    - `SIGAD.Application/Services/*`: casos de uso (coordinan repos y reglas de dominio).
    - `SIGAD.Application/DTOs/*`: modelos de transporte entre API y capa de aplicación.
    - `SIGAD.Application/Common/*`: utilidades/constantes (por ejemplo `Fuente.cs`).

Flujo de comunicación:
1) Servicio de aplicación recibe DTO desde el controlador.
2) Interactúa con repositorios (interfaces) y entidades del dominio.
3) Retorna DTO/resultado uniforme hacia la API.

### 4) Dominio: `SIGAD.Domain`
- Responsabilidad: modelo de negocio puro (entidades, enums, invariantes).
- Archivos/ubicaciones relevantes:
    - `SIGAD.Domain/Entities/*`: entidades (Docente, Solicitud, Rango, etc.).
    - `SIGAD.Domain/Enums/*`: tipos enumerados de negocio.
    - `SIGAD.Domain/Interfaces/*`: contratos del dominio (si existen).

Flujo de comunicación:
1) Usado por `Application` para crear/modificar entidades respetando las reglas del negocio.

### 5) Infraestructura: `SIGAD.Infrastructure`
- Responsabilidad: persistencia (EF Core), servicios externos y migraciones.
- Archivos/ubicaciones relevantes:
    - `SIGAD.Infrastructure/Persistence/SigadDbContext.cs`: DbContext de EF Core con DbSets y configuración.
    - `SIGAD.Infrastructure/Repositories/*`: repositorios `Ef*Repository` que implementan interfaces de `Application`.
    - `SIGAD.Infrastructure/Migrations/*`: migraciones y snapshot de EF Core.
    - `SIGAD.Infrastructure/Services/*` y `SIGAD.Infrastructure/ExternalServices/*`: servicios (correo, almacenamiento, etc.).

Flujo de comunicación:
1) Repositorios usan `SigadDbContext` para leer/escribir en SQL Server.
2) Servicios externos (e.g., email) consumen APIs/SDKs externos.

## Flujo end-to-end (ejemplo: Login)

1) Usuario ingresa credenciales en `SIGAD.BlazorApp/Pages/Login.razor` y se invoca `AuthService.Login`.
2) `AuthService` (frontend) ejecuta `POST api/Auth/login` usando `HttpClient` (BaseAddress desde `wwwroot/appsettings.json`).
3) `SIGAD.WebAPI/Controllers/AuthController` recibe la solicitud y llama al servicio de aplicación correspondiente (p.ej., `IAuthService` en `SIGAD.Application`).
4) `SIGAD.Application.Services.AuthService` valida usuario y credenciales consultando `ICuentaRepository` y entidades del dominio.
5) `SIGAD.Infrastructure.Repositories.EfCuentaRepository` realiza la consulta vía `SigadDbContext` a SQL Server.
6) `SIGAD.Application` genera un JWT (con `JwtSettings` de `SIGAD.WebAPI/appsettings.json`) y devuelve un DTO (`LoginResponseDto`) con el token.
7) El controlador retorna `200 OK` con el DTO; el frontend guarda el token en LocalStorage (`Blazored.LocalStorage`) y las siguientes peticiones pasan por `AuthorizationMessageHandler`, que adjunta el `Bearer {token}`.

Nota adicional (descarga/visualización de archivos):
- La API expone estáticos bajo `/uploads` (configurado en `SIGAD.WebAPI/Program.cs`). Los componentes `.razor` deben construir URLs relativas a `Http.BaseAddress` para obtener `BaseAddress/uploads/...` en Codespaces o Docker.

## Referencias rápidas de archivos

- Presentación
    - `SIGAD.BlazorApp/Program.cs`
    - `SIGAD.BlazorApp/wwwroot/appsettings.json`
    - `SIGAD.BlazorApp/Services/AuthService.cs`
    - `SIGAD.BlazorApp/Services/AuthorizationMessageHandler.cs`
    - `SIGAD.BlazorApp/Services/SolicitudesService.cs`, `ReporteService.cs`, `NotificacionClienteService.cs`

- API
    - `SIGAD.WebAPI/Program.cs`
    - `SIGAD.WebAPI/Controllers/*`
    - `SIGAD.WebAPI/Middleware/*`
    - `SIGAD.WebAPI/appsettings*.json`

- Aplicación
    - `SIGAD.Application/Interfaces/*`
    - `SIGAD.Application/Services/*`
    - `SIGAD.Application/DTOs/*`
    - `SIGAD.Application/Common/*`

- Dominio
    - `SIGAD.Domain/Entities/*`
    - `SIGAD.Domain/Enums/*`
    - `SIGAD.Domain/Interfaces/*`

- Infraestructura
    - `SIGAD.Infrastructure/Persistence/SigadDbContext.cs`
    - `SIGAD.Infrastructure/Repositories/*`
    - `SIGAD.Infrastructure/Migrations/*`
    - `SIGAD.Infrastructure/Services/*`, `SIGAD.Infrastructure/ExternalServices/*`

## Flujo completo resumido

Blazor (WASM) → `HttpClient` (BaseAddress + JWT) → WebAPI (Controllers + JWT + CORS + Middleware) → Application (Casos de uso + Interfaces) → Domain (Entidades) → Infrastructure (Repos EF) → SQL Server.

Todas las dependencias hacia afuera se invierten con interfaces en `Application`, y se resuelven en tiempo de ejecución por `WebAPI/Program.cs` mediante inyección de dependencias.

## Principios SOLID aplicados

### S — Single Responsibility Principle (SRP)

- Separación por proyectos y funcionalidades: cada capa tiene una única razón de cambio.
- Servicios de aplicación enfocados en casos de uso; DTOs sólo transportan datos.
- En frontend, separar lógica de llamadas HTTP en servicios (`AuthService`, `SolicitudesService`, etc.).

Mejoras sugeridas:
- Asegurar que componentes `.razor` no contengan llamadas directas a URLs absolutas; delegar a servicios.
- Extraer manejo de tokens a un proveedor dedicado (ver DIP abajo).

### O — Open/Closed Principle (OCP)

- En `Application`, programar contra interfaces permite agregar nuevas implementaciones (e.g., otro proveedor de almacenamiento o email) sin modificar los consumidores.
- En frontend, usar `HttpClient` tipado por funcionalidad facilita extender endpoints sin tocar consumidores.

Mejoras sugeridas:
- Añadir `HttpClient` tipados por módulo (Auth, Solicitudes, Reportes) en Blazor.

### L — Liskov Substitution Principle (LSP)

- Evitar métodos que lancen excepciones por estados esperables; preferir un contrato uniforme de resultados.

Mejora propuesta:
- Introducir un tipo `Result<T>` (éxito/fallo) en `Application` para estandarizar respuestas de casos de uso y facilitar sustitución sin romper expectativas.

### I — Interface Segregation Principle (ISP)

- Interfaces enfocadas a propósitos concretos evitan “Dios Interfaces”.

Mejora propuesta:
- Segregar interfaces extensas (p.ej. `ISolicitudesService`) en `ISolicitudesQueryService` y `ISolicitudesCommandService`.

### D — Dependency Inversion Principle (DIP)

- `Application` define puertos (interfaces) que `Infrastructure` implementa (repositorios, email, archivos, etc.).

Mejoras propuestas:
- Frontend: introducir `ITokenProvider` para abstraer cómo se obtiene el JWT (LocalStorage hoy, otro mañana). El `AuthorizationMessageHandler` dependerá de la interfaz.
- Backend: validar que todas las dependencias externas estén invertidas (interfaces en `Application`, implementaciones en `Infrastructure`).

## Diagnóstico SOLID por capa

### Blazor (SIGAD.BlazorApp)
- SRP (brechas): componentes `.razor` contienen lógica de datos (HTTP, composición de URLs). Deben enfocarse en presentación y delegar la lógica a servicios.
- OCP (brechas): cambios en endpoints/host exigen editar múltiples componentes si hay URLs embebidas.
- LSP (revisión): asegurar que los servicios devuelvan contratos consistentes (colecciones vacías vs. `null`, errores predecibles).
- ISP (brechas): servicios de UI con demasiadas responsabilidades (agregan Auth, Solicitudes y Reportes en uno solo).
- DIP (brechas): dependencia directa de `HttpClient` y `LocalStorage` sin abstracciones.

Evidencia: ocurrencias de `localhost` en varias páginas `.razor` y uso directo de `LocalStorage` en handlers.

### WebAPI (SIGAD.WebAPI)
- SRP (en general aplica): controllers coordinan casos de uso; mover validaciones repetitivas a filtros.
- OCP (brechas): reglas en controladores que cambian con frecuencia; mover a policies/servicios.
- LSP (revisión): repositorios deben cumplir contratos (no devolver `null` donde se espera vacío).
- ISP (mejorable): interfaces de servicios de aplicación grandes; dividir comandos/consultas.
- DIP (aplica): DI correctamente; fortalecer uso de opciones tipadas para configuración.

### Application (SIGAD.Application)
- SRP (aplica): casos de uso separados por servicio; verificar que no mezclen concern de infraestructura.
- OCP (aplica): nuevos casos se agregan sin cambiar los existentes.
- LSP (mejorable): unificar resultados con `Result<T>` y evitar lanzar excepciones para errores de negocio esperables.
- ISP (mejorable): separar interfaces de Query/Command.
- DIP (aplica): depende de interfaces; mantener libre de frameworks.

### Domain (SIGAD.Domain)
- SRP (aplica): entidades enfocadas en invariantes del negocio.
- OCP (aplica): se pueden extender reglas con Value Objects y Policies.
- LSP (N/A en gran medida): garantizar que subtipo/sustitución respete invariantes.
- ISP (N/A): contratos mínimos; preferir objetos ricos vs. getters/setters anémicos.
- DIP (aplica): no depende de infraestructura ni frameworks.

### Infrastructure (SIGAD.Infrastructure)
- SRP (aplica): persistencia y servicios externos; evitar lógica de negocio.
- OCP (aplica): agregar nuevos proveedores sin tocar `Application`.
- LSP (aplica): implementaciones deben cumplir los contratos de `Application`.
- ISP (mejorable): repositorios muy generales pueden dividirse por agregado/operación.
- DIP (aplica): solo implementa interfaces de `Application`.

## Mejoras por capa para alinear con SOLID

### Blazor (SIGAD.BlazorApp)
- Centralizar endpoints y rutas en servicios o typed `HttpClient`; eliminar URLs hardcodeadas.
- Introducir `ITokenProvider` y usarlo en `AuthorizationMessageHandler` (DIP).
- Segregar servicios por bounded context: `IAuthService`, `ISolicitudesService`, `IReportesService`, `IUploadsService` (ISP).
- Componentes `.razor` sin lógica de datos; usar ViewModels simples si hay estado complejo (SRP).

### WebAPI (SIGAD.WebAPI)
- Filtros/Middleware + `FluentValidation` para validaciones repetitivas (SRP).
- `IOptions<JwtSettings>` + `ITokenService` para emitir/validar JWT (DIP).
- Endpoints consistentes; controllers delgados delegando a `Application` (SRP/OCP).
- Autorización por policies/claims; agregar nuevas sin tocar controladores (OCP).

### Application (SIGAD.Application)
- Introducir `Result<T>`/`OneOf` como contrato uniforme de salida (LSP).
- Segregar interfaces Commands/Queries; handlers por caso de uso (ISP/SRP).
- Perfiles de AutoMapper para mapeos DTO↔Entidad; evitar duplicación (SRP/OCP).
- Domain Services/Policies para reglas transversales (OCP).

### Domain (SIGAD.Domain)
- Value Objects (Email, Periodo, Identificadores) con validaciones internas (SRP/OCP).
- Invariantes en constructores/fábricas; evitar entidades anémicas (SRP).
- Eventos de dominio para side-effects (opcional), manejados en `Application/Infrastructure` (DIP).

### Infrastructure (SIGAD.Infrastructure)
- Repositorios con `AsNoTracking` para consultas; `Specification` o Query Objects para filtros (SRP/OCP).
- Transacciones explícitas cuando el caso de uso lo requiera; Unit of Work por request (SRP).
- Servicios externos con resiliencia (timeouts, reintentos, circuit breaker con Polly) (OCP).

## Refactorizaciones recomendadas (prioridad baja-riesgo)

1) Frontend: eliminar URLs hardcodeadas
- Reemplazar `https://localhost:7072/...` en `.razor` por rutas relativas o `NavigationManager.BaseUri`/`HttpClient.BaseAddress`.
- Centralizar `ApiBaseUrl` en `wwwroot/appsettings.json` y leerlo en `Program.cs`.

2) Frontend: proveedor de tokens (DIP)
- Crear `ITokenProvider` y `LocalStorageTokenProvider` (usa Blazored.LocalStorage).
- Inyectar `ITokenProvider` en `AuthorizationMessageHandler`.

3) Frontend: HttpClient tipados
- Registrar `HttpClient` tipados: `IAuthClient`, `ISolicitudesClient`, `IReportesClient` con `BaseAddress` común y `AuthorizationMessageHandler`.

4) Backend: Result<T> en Application
- Definir `Result<T>` y helpers `Success/Failure`.
- Hacer que los casos de uso y controladores devuelvan `ActionResult` derivados de `Result`.

5) Backend: ISP en servicios
- Dividir servicios con demasiadas responsabilidades en Query/Command.

6) Configuración y seguridad
- CORS: en desarrollo `AllowAll`, en producción restringir orígenes.
- Secretos: usar variables de entorno y `User Secrets`/`KeyVault` (fuera de repo).

## Ejemplos de implementación

### 1. Frontend: leer ApiBaseUrl desde configuración

Archivo: `SIGAD.BlazorApp/wwwroot/appsettings.json`

```json
{
  "ApiBaseUrl": "https://TU-URL-DE-API"
}
```

En `Program.cs`:

```csharp
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient("SIGAD.WebApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SIGAD.WebApi"));
```

### 2. Frontend: proveedor de tokens

Interfaz:

```csharp
public interface ITokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken ct = default);
}
```

Implementación:

```csharp
public class LocalStorageTokenProvider : ITokenProvider
{
    private readonly ILocalStorageService _localStorage;
    public LocalStorageTokenProvider(ILocalStorageService localStorage) => _localStorage = localStorage;
    public Task<string?> GetTokenAsync(CancellationToken ct = default) => _localStorage.GetItemAsStringAsync("authToken", ct);
}
```

Uso en `AuthorizationMessageHandler`:

```csharp
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;
    public AuthorizationMessageHandler(ITokenProvider tokenProvider) => _tokenProvider = tokenProvider;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
```

### 3. Backend: Result<T> en Application

```csharp
public sealed record Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? Error { get; init; }
    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}
```

En un servicio de aplicación:

```csharp
public async Task<Result<SolicitudDto>> ObtenerSolicitudAsync(int id)
{
    var entity = await _repo.GetByIdAsync(id);
    if (entity is null) return Result<SolicitudDto>.Failure($"Solicitud {id} no encontrada");
    return Result<SolicitudDto>.Success(Map(entity));
}
```

En un controlador:

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> Get(int id)
{
    var result = await _service.ObtenerSolicitudAsync(id);
    return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
}
```

## Checklist de adopción

- [ ] Blazor: `ApiBaseUrl` en configuración, sin URLs hardcodeadas.
- [ ] `AuthorizationMessageHandler` usa `ITokenProvider`.
- [ ] `HttpClient` tipados por módulo.
- [ ] `Result<T>` usado en servicios de aplicación y controladores.
- [ ] Servicios divididos en Query/Command donde aplique.
- [ ] CORS restringido en producción.

## Cómo validar

- Compilar y ejecutar contenedores: `docker-compose up --build -d`.
- Ver que Blazor usa la URL configurada en `appsettings.json` para la API.
- Navegar a Swagger y ejecutar endpoints autenticados.
- Ejecutar pruebas manuales en UI para flujos de solicitudes, carga de archivos y apelaciones.
