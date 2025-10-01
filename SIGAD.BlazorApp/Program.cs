using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SIGAD.BlazorApp;
using SIGAD.BlazorApp.Services;
using SIGAD.BlazorApp.Abstractions;
using SIGAD.BlazorApp.ApiClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuración de base URL para la API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddBlazoredLocalStorage(); // Registrar el servicio de local storage
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// ========== FASE 1: NUEVOS SERVICIOS SOLID ==========

// 1. Registrar abstracción de token (DIP)
builder.Services.AddScoped<ITokenProvider, LocalStorageTokenProvider>();

// 2. Registrar AuthorizationMessageHandler (necesario para clientes tipados)
builder.Services.AddScoped<AuthorizationMessageHandler>();

// 3. Registrar clientes tipados de API con HttpClient configurado
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

// ========== SERVICIOS EXISTENTES (temporalmente mantener compatibilidad) ==========

builder.Services.AddScoped<IAuthService, AuthService>();
//REVISAR21
//builder.Services.AddScoped<ISolicitudService, SolicitudService>();
// Agregar el servicio ISolicitudesService que también se necesita
builder.Services.AddScoped<ISolicitudesService, SolicitudesService>();
builder.Services.AddScoped<SolicitudesService>();
builder.Services.AddScoped<INotificacionClienteService, NotificacionClienteService>();
builder.Services.AddScoped<SIGAD.BlazorApp.Services.ISolicitudService, SIGAD.BlazorApp.Services.SolicitudService>();
builder.Services.AddScoped<SIGAD.BlazorApp.Services.ISolicitudesService, SIGAD.BlazorApp.Services.SolicitudesService>();

builder.Services.AddScoped<ReporteService>();

// ========== HTTPCLIENT LEGACY (mantener para servicios que aún no se migraron) ==========

builder.Services.AddScoped(sp =>
{
    // Leer la base URL desde configuración (wwwroot/appsettings.json)
    var baseAddress = builder.Configuration["ApiSettings:BaseUrl"]
        ?? builder.HostEnvironment.BaseAddress; // fallback razonable
    return new HttpClient
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromMinutes(10)
    };
});

builder.Services.AddHttpClient("SIGAD.WebApi", client =>
{
    var baseAddress = builder.Configuration["ApiSettings:BaseUrl"]
        ?? builder.HostEnvironment.BaseAddress;
    client.BaseAddress = new Uri(baseAddress);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SIGAD.WebApi"));

await builder.Build().RunAsync();