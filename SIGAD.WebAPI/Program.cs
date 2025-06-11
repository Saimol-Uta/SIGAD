// SIGAD.WebAPI/Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SIGAD.Application.Services;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.Persistence;
using SIGAD.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE CONFIGURACIÓN DE SERVICIOS ---

// 1. Configurar DbContext para Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SigadDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configurar JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 3. Registrar servicios para Inyección de Dependencias (DI)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRangoRepository, EfRangoRepository>();
builder.Services.AddScoped<ICuentaRepository, EfCuentaRepository>();
builder.Services.AddScoped<ISolicitudAscensoRepository, EfSolicitudAscensoRepository>();
builder.Services.AddScoped<IArticuloRepository, EfArticuloRepository>();
builder.Services.AddScoped<IInvestigacionRepository, EfInvestigacionRepository>();

// Servicios de aplicación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<GestionArticulosAppService>();
builder.Services.AddScoped<GestionInvestigacionesAppService>();
builder.Services.AddScoped<ConsultaRangoAppService>();
builder.Services.AddScoped<GestionRangoAppService>();
builder.Services.AddScoped<ActualizarRangoService>();
builder.Services.AddScoped<GestionSolicitudesAppService>();

// 4. Agregar servicios para controladores de API
builder.Services.AddControllers();

// 5. Configurar Swagger/OpenAPI con soporte para JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIGAD API",
        Version = "v1",
        Description = "API para el Sistema de Gestión Académica Docente (SIGAD)"
    });

    // Configurar autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 6. Configurar CORS
builder.Services.AddCors();

// --- CONSTRUCCIÓN DE LA APLICACIÓN Y PIPELINE ---

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGAD API v1");
        c.DocumentTitle = "SIGAD API - Documentación";
    });
}

app.UseHttpsRedirection();

// Usar la política de CORS (recuerda ajustar los puertos si son diferentes)
app.UseCors(policy =>
    policy.WithOrigins("https://localhost:7087", "http://localhost:5250")
    .AllowAnyMethod()
    .AllowAnyHeader());

// IMPORTANTE: El orden de estos middlewares es crucial
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();