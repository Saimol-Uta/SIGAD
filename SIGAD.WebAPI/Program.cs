// SIGAD.WebAPI/Program.cs
using Microsoft.EntityFrameworkCore;
using SIGAD.Application.Services;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using SIGAD.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE CONFIGURACIÓN DE SERVICIOS ---

// 1. Configurar DbContext para Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SigadDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Registrar servicios para Inyección de Dependencias (DI)
// Aquí es donde hacemos la corrección.
// Le decimos: "Cuando se necesite un IRangoRepository, usa la clase EfRangoRepository".
builder.Services.AddScoped<IRangoRepository, EfRangoRepository>();

// Registramos el servicio de aplicación que usa el repositorio.
builder.Services.AddScoped<ConsultaRangoAppService>();

// NOTA PARA EL FUTURO: A medida que crees más repositorios y servicios
// (como para TipoDocumento), los añadirás aquí de la misma forma.


// 3. Agregar servicios para controladores de API
builder.Services.AddControllers();

// 4. Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Configurar CORS
builder.Services.AddCors();

// --- CONSTRUCCIÓN DE LA APLICACIÓN Y PIPELINE ---

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar la política de CORS (recuerda ajustar los puertos si son diferentes)
app.UseCors(policy =>
    policy.WithOrigins("https://localhost:7087", "http://localhost:5250")
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthorization();

app.MapControllers();

app.Run();