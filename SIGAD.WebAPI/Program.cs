// SIGAD.WebAPI/Program.cs

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Application.Services;
using SIGAD.Application.Services.ExternalServices;
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.ExternalServices;
using SIGAD.Infrastructure.Persistence;
using SIGAD.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- SECCIÓN DE CONFIGURACIÓN DE SERVICIOS ---

// 1. Configurar DbContext para Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SigadDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName); // 👈 Solución alternativa
});

var configuration = builder.Configuration;

builder.Services.AddScoped<ISgthSyncService>(_ =>
    new SgthSyncService(configuration.GetConnectionString("SGTH")!));

builder.Services.AddScoped<ISutSyncService>(_ =>
    new SutSyncService(configuration.GetConnectionString("SUT")!));

builder.Services.AddScoped<IDiticSyncService>(_ =>
    new DiticSyncService(configuration.GetConnectionString("DITIC")!));


builder.Services.AddScoped<DocenteSyncCoordinator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDocenteSyncCoordinator, DocenteSyncCoordinator>();
builder.Services.AddScoped<HistorialDocenteImporter>();



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
builder.Services.AddScoped<IDocenteRepository, EfDocenteRepository>();
builder.Services.AddScoped<ISolicitudAscensoRepository, EfSolicitudAscensoRepository>();
builder.Services.AddScoped<IArticuloRepository, EfArticuloRepository>();
builder.Services.AddScoped<ICursoRepository, EfCursoRepository>();
builder.Services.AddScoped<IInvestigacionRepository, EfInvestigacionRepository>();
builder.Services.AddScoped<IEvaluacionDocenteRepository, EfEvaluacionDocenteRepository>();
builder.Services.AddScoped<IExperienciaLaboralRepository, ExperienciaLaboralRepository>();
builder.Services.AddScoped<IOrganizacionRepository, EfOrganizacionRepository>();

// Servicios de aplicación
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEvaluacionDocenteService, EvaluacionDocenteService>();
builder.Services.AddScoped<IArticuloService, ArticuloService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IInvestigacionService, InvestigacionService>();
builder.Services.AddScoped<IExperienciaLaboralService, ExperienciaLaboralService>();
// builder.Services.AddScoped<GestionArticulosAppService>();
// builder.Services.AddScoped<GestionInvestigacionesAppService>();
// builder.Services.AddScoped<ConsultaRangoAppService>();
// builder.Services.AddScoped<GestionRangoAppService>();
// builder.Services.AddScoped<ActualizarRangoService>();
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

    // Configurar Swagger para usar JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
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
            Array.Empty<string>()
        }
    });
});

// 6. Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- SECCIÓN DE CONFIGURACIÓN DE MIDDLEWARE ---
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"🔥 ERROR: {ex.Message}\n{ex.StackTrace}");
        throw;
    }
});

// 1. Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();