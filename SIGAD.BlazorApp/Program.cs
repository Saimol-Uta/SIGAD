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
    var baseAddress = "https://super-space-spoon-pj99gqvv95vwf6wrp-5217.app.github.dev";
    
    return new HttpClient
    {
        BaseAddress = new Uri(baseAddress),
        Timeout = TimeSpan.FromMinutes(10) // 10 minutos para operaciones críticas como apelaciones
    };
});

builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services.AddHttpClient("SIGAD.WebApi", client =>
{
    client.BaseAddress = new Uri("https://super-space-spoon-pj99gqvv95vwf6wrp-5217.app.github.dev"); // La dirección de tu API
    client.Timeout = TimeSpan.FromMinutes(10);
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SIGAD.WebApi"));

await builder.Build().RunAsync();