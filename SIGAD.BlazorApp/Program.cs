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

builder.Services.AddScoped<SIGAD.BlazorApp.Services.ISolicitudService, SIGAD.BlazorApp.Services.SolicitudService>();
builder.Services.AddScoped<SIGAD.BlazorApp.Services.ISolicitudesService, SIGAD.BlazorApp.Services.SolicitudesService>();

builder.Services.AddScoped<ReporteService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7072") });

await builder.Build().RunAsync();
