# Guía de Integración - Refactorización SOLID

## 📋 Objetivo
Integrar las nuevas interfaces segregadas y clientes tipados en el proyecto existente, siguiendo los principios SOLID.

---

## 1. Registrar Clientes Tipados en Blazor

### Archivo: `SIGAD.BlazorApp/Program.cs`

Agregar después de la configuración del HttpClient base:

```csharp
// Registrar abstracción de token
builder.Services.AddScoped<ITokenProvider, LocalStorageTokenProvider>();

// Registrar clientes tipados de API
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddHttpClient<ISolicitudesQueryApiClient, SolicitudesQueryApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddHttpClient<ISolicitudesCommandApiClient, SolicitudesCommandApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();
```

---

## 2. Actualizar AuthService para Usar ITokenProvider

### Archivo: `SIGAD.BlazorApp/Services/AuthService.cs`

**Antes:**
```csharp
public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage; // ❌ Dependencia directa
    
    public AuthService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }
    
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var response = await _http.PostAsJsonAsync("api/Auth/login", request);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            await _localStorage.SetItemAsStringAsync("authToken", result.Token); // ❌
            return result;
        }
        return null;
    }
}
```

**Después:**
```csharp
public class AuthService
{
    private readonly IAuthApiClient _authClient; // ✅ Cliente tipado
    private readonly ITokenProvider _tokenProvider; // ✅ Abstracción
    
    public AuthService(IAuthApiClient authClient, ITokenProvider tokenProvider)
    {
        _authClient = authClient;
        _tokenProvider = tokenProvider;
    }
    
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var result = await _authClient.LoginAsync(request);
        if (result != null)
        {
            await _tokenProvider.SetTokenAsync(result.Token); // ✅
        }
        return result;
    }
}
```

---

## 3. Actualizar AuthorizationMessageHandler

### Archivo: `SIGAD.BlazorApp/Services/AuthorizationMessageHandler.cs`

**Antes:**
```csharp
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage; // ❌
    
    public AuthorizationMessageHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsStringAsync("authToken"); // ❌
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
```

**Después:**
```csharp
public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider; // ✅
    
    public AuthorizationMessageHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync(); // ✅
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
```

---

## 4. Actualizar Componentes Blazor

### Ejemplo: `Login.razor`

**Antes:**
```razor
@inject HttpClient Http
@inject ILocalStorageService LocalStorage

@code {
    private async Task HandleLogin()
    {
        var response = await Http.PostAsJsonAsync("api/Auth/login", loginRequest);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            await LocalStorage.SetItemAsStringAsync("authToken", result.Token);
            // ...
        }
    }
}
```

**Después:**
```razor
@inject IAuthApiClient AuthClient
@inject ITokenProvider TokenProvider

@code {
    private async Task HandleLogin()
    {
        var result = await AuthClient.LoginAsync(loginRequest);
        if (result != null)
        {
            await TokenProvider.SetTokenAsync(result.Token);
            // ...
        }
    }
}
```

### Ejemplo: Componente que muestra solicitudes

**Antes:**
```razor
@inject HttpClient Http

@code {
    private List<SolicitudDetalleDto> solicitudes = new();
    
    protected override async Task OnInitializedAsync()
    {
        solicitudes = await Http.GetFromJsonAsync<List<SolicitudDetalleDto>>("api/Ascensos") ?? new();
    }
}
```

**Después:**
```razor
@inject ISolicitudesQueryApiClient SolicitudesQuery

@code {
    private List<SolicitudDetalleDto> solicitudes = new();
    
    protected override async Task OnInitializedAsync()
    {
        solicitudes = (await SolicitudesQuery.GetAllAsync()).ToList();
    }
}
```

---

## 5. Actualizar Controladores para Usar Políticas

### Archivo: `SIGAD.WebAPI/Controllers/AscensosController.cs`

**Antes:**
```csharp
[HttpPost("{id}/aprobar-comision")]
[Authorize(Roles = "Admin")] // ❌ Hardcoded
public async Task<IActionResult> AprobarPorComision(Guid id)
{
    // ...
}
```

**Después:**
```csharp
[HttpPost("{id}/aprobar-comision")]
[Authorize(Policy = "CanManageSolicitudes")] // ✅ Política
public async Task<IActionResult> AprobarPorComision(Guid id)
{
    // ...
}
```

### Tabla de Mapeo de Roles → Políticas

| Antes (Roles Hardcoded) | Después (Políticas) | Uso |
|-------------------------|---------------------|-----|
| `[Authorize(Roles = "Admin")]` | `[Authorize(Policy = "RequireAdminRole")]` | Acciones administrativas generales |
| `[Authorize(Roles = "Docente")]` | `[Authorize(Policy = "RequireDocenteRole")]` | Acciones de docentes |
| `[Authorize(Roles = "Admin,Docente")]` | `[Authorize(Policy = "RequireAdminOrDocente")]` | Acciones compartidas |
| - | `[Authorize(Policy = "CanManageSolicitudes")]` | Aprobar/rechazar solicitudes (Admin) |
| - | `[Authorize(Policy = "CanCreateSolicitud")]` | Crear solicitudes (Docente) |
| `[Authorize]` | `[Authorize(Policy = "CanViewOwnSolicitud")]` | Ver solicitudes propias |

---

## 6. Implementar Servicios Segregados (Ejemplo)

### Crear `SolicitudQueryService`

**Archivo:** `SIGAD.Application/Services/SolicitudQueryService.cs`

```csharp
using SIGAD.Application.Contracts.Services;
using SIGAD.Application.DTOs;
using SIGAD.Domain.Enums;
using SIGAD.Domain.Interfaces;

namespace SIGAD.Application.Services
{
    public class SolicitudQueryService : ISolicitudQueryService
    {
        private readonly ISolicitudAscensoRepository _repository;

        public SolicitudQueryService(ISolicitudAscensoRepository repository)
        {
            _repository = repository;
        }

        public async Task<SolicitudDetalleDto?> GetByIdAsync(Guid id)
        {
            var solicitud = await _repository.GetByIdWithDetailsAsync(id);
            if (solicitud == null) return null;
            
            // Mapear a DTO (considerar usar AutoMapper)
            return MapToDto(solicitud);
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetAllAsync()
        {
            var solicitudes = await _repository.GetAllWithDetailsAsync();
            return solicitudes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetHistorialByDocenteAsync(string docenteCedula)
        {
            var solicitudes = await _repository.GetHistorialByDocenteAsync(docenteCedula);
            return solicitudes.Select(MapToDto);
        }

        public async Task<bool> HasActiveSolicitudAsync(string docenteCedula)
        {
            return await _repository.HasActiveSolicitudAsync(docenteCedula);
        }

        public async Task<SolicitudDetalleDto?> GetActiveSolicitudByDocenteAsync(string docenteCedula)
        {
            var solicitud = await _repository.GetActiveSolicitudByDocenteAsync(docenteCedula);
            return solicitud != null ? MapToDto(solicitud) : null;
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetByEstadoAsync(EstadoSolicitud estado)
        {
            var solicitudes = await _repository.GetByEstadoAsync(estado);
            return solicitudes.Select(MapToDto);
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetPendientesRevisionAsync()
        {
            var solicitudes = await _repository.GetPendientesRevisionAsync();
            return solicitudes.Select(MapToDto);
        }

        public async Task<Dictionary<EstadoSolicitud, int>> GetEstadisticasByEstadoAsync()
        {
            var result = new Dictionary<EstadoSolicitud, int>();
            foreach (EstadoSolicitud estado in Enum.GetValues<EstadoSolicitud>())
            {
                var count = await _repository.GetCantidadSolicitudesByEstadoAsync(estado);
                result[estado] = count;
            }
            return result;
        }

        public async Task<IEnumerable<SolicitudDetalleDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var solicitudes = await _repository.GetSolicitudesByFechaAsync(fechaInicio, fechaFin);
            return solicitudes.Select(MapToDto);
        }

        private SolicitudDetalleDto MapToDto(SolicitudAscenso solicitud)
        {
            // TODO: Implementar mapeo completo o usar AutoMapper
            return new SolicitudDetalleDto
            {
                Id = solicitud.Id,
                Estado = solicitud.Estado.ToString(),
                FechaCreacion = solicitud.FechaCreacion,
                // ... resto de propiedades
            };
        }
    }
}
```

### Registrar en DI

**Archivo:** `SIGAD.WebAPI/Program.cs`

```csharp
// Servicios segregados (CQRS)
builder.Services.AddScoped<ISolicitudQueryService, SolicitudQueryService>();
builder.Services.AddScoped<ISolicitudCommandService, SolicitudCommandService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
```

---

## 7. Testing

### Ejemplo de Test con Interfaces Segregadas

```csharp
public class SolicitudQueryServiceTests
{
    private readonly Mock<ISolicitudAscensoRepository> _mockRepository;
    private readonly ISolicitudQueryService _service;

    public SolicitudQueryServiceTests()
    {
        _mockRepository = new Mock<ISolicitudAscensoRepository>();
        _service = new SolicitudQueryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSolicitudes()
    {
        // Arrange
        var solicitudes = new List<SolicitudAscenso> { /* ... */ };
        _mockRepository.Setup(r => r.GetAllWithDetailsAsync()).ReturnsAsync(solicitudes);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(solicitudes.Count, result.Count());
    }
}
```

---

## 8. Checklist de Verificación

- [ ] Todos los clientes tipados están registrados en Blazor Program.cs
- [ ] `ITokenProvider` está registrado y usado en AuthService
- [ ] AuthorizationMessageHandler usa `ITokenProvider`
- [ ] Al menos 3 componentes .razor actualizados para usar clientes tipados
- [ ] Al menos 5 endpoints actualizados con políticas de autorización
- [ ] Servicios concretos implementan las interfaces segregadas
- [ ] Todos los servicios segregados están registrados en DI
- [ ] La solución compila sin errores
- [ ] Pruebas manuales de login y operaciones CRUD funcionan
- [ ] Pruebas unitarias actualizadas para nuevas interfaces

---

## 9. Beneficios Esperados

### Testabilidad
- Fácil de mockear interfaces pequeñas y específicas
- Tests más rápidos (solo mockeas lo que necesitas)

### Mantenibilidad
- Cambios localizados (modificar una política no afecta controladores)
- Interfaces pequeñas = menos acoplamiento

### Escalabilidad
- Agregar nuevos clientes de API sin modificar existentes
- Cambiar proveedores de almacenamiento (LocalStorage → SessionStorage) sin tocar servicios

### Seguridad
- Políticas centralizadas = menos errores de permisos
- Fácil de auditar quién puede hacer qué

---

## 10. Soporte

Si encuentras problemas durante la integración:

1. Verifica que todas las interfaces están en `SIGAD.Application/Contracts/`
2. Asegúrate de que los namespaces `using` son correctos
3. Confirma que los servicios están registrados en DI
4. Revisa que los clientes tipados usan el BaseAddress correcto

**Documentación adicional:**
- `REFACTORIZACION-SOLID.md` - Resumen completo de cambios
- `README.Arquitectura-SOLID.md` - Guía de arquitectura y principios SOLID
