using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;
using SIGAD.Application.Interfaces;
using SIGAD.Application.Interfaces.Integraciones;
using SIGAD.Application.Services;
using SIGAD.Application.Services.ExternalServices;
using SIGAD.Application.Contracts.Services; // NUEVO: interfaces segregadas
using SIGAD.Domain.Interfaces;
using SIGAD.Infrastructure.ExternalServices;
using SIGAD.Infrastructure.Persistence;
using SIGAD.Infrastructure.Repositories;
using SIGAD.Infrastructure.Services;
using SIGAD.WebAPI.Services;
using SIGAD.WebAPI.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar licencia de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// --- SECCIÓN DE CONFIGURACIÓN DE SERVICIOS ---

// 1. Configurar DbContext para Entity Framework Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SigadDbContext>(options =>
    options.UseSqlServer(connectionString));

var configuration = builder.Configuration;

builder.Services.AddScoped<ISgthSyncService>(_ =>
    new SgthSyncService(configuration.GetConnectionString("SGTH")!));

builder.Services.AddScoped<ISutSyncService>(_ =>
    new SutSyncService(configuration.GetConnectionString("SUT")!));

builder.Services.AddScoped<IDiticSyncService>(_ =>
    new DiticSyncService(configuration.GetConnectionString("DITIC")!));

// Registrar servicio de procesamiento de archivos para importación
builder.Services.AddScoped<IArchivoImportacionService, ArchivoImportacionService>();


builder.Services.AddScoped<DocenteSyncCoordinator>();
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

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    // Política para administradores
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("ADMINISTRADOR")); // Fase 4: Corregido para coincidir con enum Rol

    // Política para docentes
    options.AddPolicy("RequireDocenteRole", policy =>
        policy.RequireRole("DOCENTE")); // Fase 4: Corregido para coincidir con enum Rol

    // Política para administradores o docentes
    options.AddPolicy("RequireAdminOrDocente", policy =>
        policy.RequireRole("ADMINISTRADOR", "DOCENTE")); // Fase 4: Corregido para coincidir con enum Rol

    // Política para gestionar solicitudes (solo admin)
    options.AddPolicy("CanManageSolicitudes", policy =>
        policy.RequireRole("ADMINISTRADOR")); // Fase 4: Corregido para coincidir con enum Rol

    // Política para crear solicitudes (solo docentes)
    options.AddPolicy("CanCreateSolicitud", policy =>
        policy.RequireRole("DOCENTE")); // Fase 4: Corregido para coincidir con enum Rol

    // Política para ver solicitudes propias
    options.AddPolicy("CanViewOwnSolicitud", policy =>
        policy.RequireAuthenticatedUser());
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<SmtpEmailService>();
builder.Services.AddScoped<IApiEmailService, ApiEmailService>();
builder.Services.AddScoped<IEmailService, ResilientEmailService>();
builder.Services.AddScoped<INotificacionService, NotificacionService>();

// 3. Registrar servicios para Inyección de Dependencias (DI)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRangoRepository, EfRangoRepository>();
builder.Services.AddScoped<ICuentaRepository, EfCuentaRepository>();
builder.Services.AddScoped<IDocenteRepository, EfDocenteRepository>();
builder.Services.AddScoped<ISolicitudAscensoRepository, EfSolicitudAscensoRepository>();
builder.Services.AddScoped<IApelacionRepository, EfApelacionRepository>();
builder.Services.AddScoped<IArticuloRepository, EfArticuloRepository>();
builder.Services.AddScoped<ICursoRepository, EfCursoRepository>();
builder.Services.AddScoped<IInvestigacionRepository, EfInvestigacionRepository>();
builder.Services.AddScoped<IEvaluacionDocenteRepository, EfEvaluacionDocenteRepository>();
builder.Services.AddScoped<IExperienciaLaboralRepository, ExperienciaLaboralRepository>();
builder.Services.AddScoped<ITesisDirigidaRepository, EfTesisDirigidaRepository>();
builder.Services.AddScoped<IOrganizacionRepository, EfOrganizacionRepository>();

// ✅ Servicios segregados de autenticación (SOLID - SRP)
// AuthService monolítico ELIMINADO - ahora usamos servicios especializados
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();

builder.Services.AddScoped<IEvaluacionDocenteService, EvaluacionDocenteService>();
builder.Services.AddScoped<IArticuloService, ArticuloService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IInvestigacionService, InvestigacionService>();
builder.Services.AddScoped<IExperienciaLaboralService, ExperienciaLaboralService>();
builder.Services.AddScoped<ITesisDirigidaService, TesisDirigidaService>();

// Servicios de aplicación específicos para SIGAD
builder.Services.AddScoped<ConsultaRangoAppService>();
builder.Services.AddScoped<GestionRangoAppService>();
builder.Services.AddScoped<ActualizarRangoService>();
builder.Services.AddScoped<GestionSolicitudesAppService>();
builder.Services.AddScoped<ValidacionRequisitosService>();
builder.Services.AddScoped<IValidacionRequisitosService, ValidacionRequisitosService>();
//builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Servicio para generación de certificados de acción de personal
builder.Services.AddScoped<IAccionPersonalService, AccionPersonalService>();

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SigadDbContext>());
builder.Services.AddScoped<ReporteBackendService>();
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

    // Resolver conflictos de nombres de esquemas
    c.CustomSchemaIds(type =>
    {
        if (type.FullName != null)
        {
            // Si el tipo está en el namespace de IntegracionesExternas, agregar prefijo
            if (type.FullName.Contains("IntegracionesExternas"))
            {
                return $"External{type.Name}";
            }
            // Para otros tipos duplicados, usar el namespace completo
            if (type.FullName.Contains("SIGAD.Application.DTOs."))
            {
                return type.FullName.Replace("SIGAD.Application.DTOs.", "").Replace(".", "");
            }
        }
        return type.Name;
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


// Agregar servicios faltantes para archivos
builder.Services.AddScoped<SIGAD.Infrastructure.Services.CloudinaryService>();
builder.Services.AddScoped<SIGAD.Application.Interfaces.ICloudinaryService>(provider =>
    provider.GetRequiredService<SIGAD.Infrastructure.Services.CloudinaryService>());
builder.Services.AddScoped<SIGAD.Application.Interfaces.IFileStorageService, SIGAD.Infrastructure.Services.FileStorageService>();

var app = builder.Build();

// Crear directorio uploads si no existe
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
    // Crear subdirectorios para cada tipo de documento
    Directory.CreateDirectory(Path.Combine(uploadsPath, "investigaciones"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "articulos"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "cursos"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "experiencias"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "evaluaciones"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "tesis"));
    Directory.CreateDirectory(Path.Combine(uploadsPath, "acciones_personal"));
}

app.UseStaticFiles(); // Esto sirve wwwroot por defecto

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// --- SECCIÓN DE CONFIGURACIÓN DE MIDDLEWARE ---

// 1. Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// 2. Middleware personalizado de validación y manejo de errores
app.UseValidationMiddleware();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();