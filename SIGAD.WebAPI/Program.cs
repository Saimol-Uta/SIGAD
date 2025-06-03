// SIGAD.WebAPI/Program.cs
using Microsoft.EntityFrameworkCore;
using SIGAD.Application.Services;    // Para ConsultaRangoAppService
using SIGAD.Domain.Interfaces;       // Para IRangoRepository
using SIGAD.Infrastructure.Persistence; // Para SigadDbContext
using SIGAD.Infrastructure.Repositories; // Para EfRangoRepository

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DbContext para Entity Framework Core
builder.Services.AddDbContext<SigadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Registrar servicios para Inyección de Dependencias (DI)
// Cuando una clase pida IRangoRepository, se le dará una instancia de EfRangoRepository.
// AddScoped significa que se creará una instancia por cada solicitud HTTP.
builder.Services.AddScoped<IRangoRepository, EfRangoRepository>();

// Registrar el servicio de aplicación
builder.Services.AddScoped<ConsultaRangoAppService>();
// Si hubieras usado una interfaz para el servicio de aplicación:
// builder.Services.AddScoped<IConsultaRangoAppService, ConsultaRangoAppService>();


// 3. Agregar servicios para controladores (necesario para que funcionen las APIs)
builder.Services.AddControllers();

// 4. Configurar Swagger/OpenAPI (útil para probar tu API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Esto es importante para que Blazor pueda llamar a la API desde un origen diferente (tu app Blazor corriendo en otro puerto)
// En producción, deberías configurar esto de forma más restrictiva.
app.UseCors(policy =>
    policy.WithOrigins("https://localhost:PORT_BLAZOR_APP", "http://localhost:PORT_BLAZOR_APP_HTTP") // Reemplaza con los puertos de tu app Blazor
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization(); // Si añades autenticación más adelante

app.MapControllers(); // Mapea las rutas a tus controladores

app.Run();