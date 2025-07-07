using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SIGAD.BlazorApp;
using SIGAD.BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage(); // Registrar el servicio de local storage
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
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

builder.Services.AddScoped(sp => 
{
    // Para BlazorApp, usar la URL del navegador actual ya que el contenedor usa nginx
    var baseAddress = "http://localhost:5217";
    
    return new HttpClient
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromMinutes(10) // 10 minutos para operaciones críticas como apelaciones
    };
});

await builder.Build().RunAsync();
