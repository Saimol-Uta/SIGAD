
## Guía de Desarrollo del Proyecto SIGAD con Arquitectura Onion (Tutorial de Inicio)

**Versión:** 1.1
**Fecha:** 3 de junio de 2025

### Introducción

Bienvenido al equipo de desarrollo del proyecto **SIGAD (Sistema de Gestión de Ascensos Docentes)**. Este manual te guiará a través de nuestra arquitectura de software (Arquitectura Onion) y te proporcionará un tutorial paso a paso para que puedas comenzar a desarrollar nuevas funcionalidades.

**Nuestro Stack Tecnológico:**
* **Backend:** .NET 9, ASP.NET Core Web API, Entity Framework Core 9.
* **Frontend:** Blazor WebAssembly.
* **Base de Datos:** SQL Server.

La Arquitectura Onion nos ayuda a construir un sistema robusto, desacoplado y fácil de mantener. ¡Vamos a aprender cómo!

### Para un Nuevo Compañero: Tu Primera Tarea (Tutorial: Gestionar Tipos de Documento)

Después de clonar el proyecto y familiarizarte con la estructura (Fase 0), tu primera tarea será implementar una nueva funcionalidad: **"Gestionar Tipos de Documento"**. Un "Tipo de Documento" podría ser, por ejemplo, "Certificado de Publicación", "Diploma Académico", "Constancia de Experiencia Laboral", etc. Estos tipos se usarán más adelante en el sistema.

Este tutorial te guiará para:
1.  Crear la entidad `TipoDocumento`.
2.  Implementar la lógica para listar y agregar nuevos Tipos de Documento.
3.  Exponer esta funcionalidad a través de la API.
4.  Crear una página en Blazor para interactuar con ella.

Sigue las fases descritas a continuación. Cada fase te indicará qué archivos crear o modificar y te dará ejemplos de código.

---
### Fase 0: Entendiendo la Estructura del Proyecto Clonado

**Objetivo:** Asegurarte de que puedes ejecutar el proyecto existente y entiendes su organización.

1.  **Clona el Repositorio:** Obtén la última versión del proyecto desde GitHub.
2.  **Abre la Solución (`SIGAD.sln`) en Visual Studio.**
3.  **Revisa la Estructura de Proyectos:**
    * `SIGAD.Domain`: Contiene entidades, interfaces de repositorio (el núcleo).
    * `SIGAD.Application`: Lógica de casos de uso, DTOs, interfaces de servicios de aplicación.
    * `SIGAD.Infrastructure`: Implementaciones de repositorios (EF Core), DbContext, migraciones.
    * `SIGAD.WebAPI`: Controladores API RESTful.
    * `SIGAD.BlazorApp`: Componentes y páginas de la interfaz de usuario.
4.  **Configura Múltiples Proyectos de Inicio:**
    * Clic derecho en la Solución -> "Establecer proyectos de inicio..." -> "Varios proyectos de inicio".
    * Establece `SIGAD.WebAPI` y `SIGAD.BlazorApp` con la Acción "Iniciar".
5.  **Ejecuta el Proyecto (F5):**
    * Deberías poder ver la funcionalidad existente (como la lista de "Rangos Académicos") funcionando. Esto confirma que tu entorno está configurado correctamente.
    * Revisa la cadena de conexión en `SIGAD.WebAPI/appsettings.json` y asegúrate que apunta a tu instancia local de SQL Server. Si la base de datos no existe, los comandos de migración (que verás más adelante) la crearán.

---
### Desarrollando la Funcionalidad "Gestionar Tipos de Documento" (Paso a Paso)

Ahora, implementaremos la nueva funcionalidad.

#### Fase 1 (Dominio): Creando la Entidad `TipoDocumento` y su Repositorio

**Objetivo:** Definir el objeto de negocio `TipoDocumento` y su contrato de persistencia.

1.  **Crear la Entidad `TipoDocumento.cs`:**
    * En el proyecto `SIGAD.Domain`, dentro de la carpeta `Entities`, crea una nueva clase `TipoDocumento.cs`:
      ```csharp
      // SIGAD.Domain/Entities/TipoDocumento.cs
      namespace SIGAD.Domain.Entities
      {
          public class TipoDocumento
          {
              public Guid Id { get; private set; }
              public string Nombre { get; private set; }
              public string? Descripcion { get; private set; } // Puede ser nulo
              public bool Activo { get; private set; }

              private TipoDocumento() { /* Constructor para EF Core */ }

              public TipoDocumento(Guid id, string nombre, string? descripcion)
              {
                  if (string.IsNullOrWhiteSpace(nombre))
                      throw new ArgumentException("El nombre del tipo de documento es requerido.", nameof(nombre));
                  if (nombre.Length > 100)
                      throw new ArgumentOutOfRangeException(nameof(nombre), "El nombre no puede exceder los 100 caracteres.");

                  Id = id == Guid.Empty ? Guid.NewGuid() : id;
                  Nombre = nombre;
                  Descripcion = descripcion;
                  Activo = true; // Por defecto, un nuevo tipo de documento está activo
              }

              public void ActualizarInformacion(string nombre, string? descripcion)
              {
                  if (string.IsNullOrWhiteSpace(nombre))
                      throw new ArgumentException("El nombre del tipo de documento es requerido.", nameof(nombre));
                  if (nombre.Length > 100)
                      throw new ArgumentOutOfRangeException(nameof(nombre), "El nombre no puede exceder los 100 caracteres.");
                  
                  Nombre = nombre;
                  Descripcion = descripcion;
              }

              public void Desactivar() => Activo = false;
              public void Activar() => Activo = true;
          }
      }
      ```

2.  **Crear la Interfaz `ITipoDocumentoRepository.cs`:**
    * En `SIGAD.Domain`, dentro de la carpeta `Interfaces`, crea una nueva interfaz `ITipoDocumentoRepository.cs`:
      ```csharp
      // SIGAD.Domain/Interfaces/ITipoDocumentoRepository.cs
      using SIGAD.Domain.Entities;
      using System.Collections.Generic;
      using System.Threading.Tasks;

      namespace SIGAD.Domain.Interfaces
      {
          public interface ITipoDocumentoRepository
          {
              Task<TipoDocumento?> GetByIdAsync(Guid id);
              Task<IEnumerable<TipoDocumento>> GetAllAsync();
              Task AddAsync(TipoDocumento tipoDocumento);
              Task UpdateAsync(TipoDocumento tipoDocumento);
              // No incluiremos DeleteAsync por ahora, usaremos el estado Activo/Inactivo
          }
      }
      ```

#### Fase 2 (Aplicación): Creando Servicios para `TipoDocumento`

**Objetivo:** Definir DTOs y la lógica de aplicación para gestionar Tipos de Documento.

1.  **Crear DTOs en `SIGAD.Application/DTOs/`:**
    * `TipoDocumentoDto.cs`:
      ```csharp
      namespace SIGAD.Application.DTOs
      {
          public class TipoDocumentoDto
          {
              public Guid Id { get; set; }
              public string Nombre { get; set; }
              public string? Descripcion { get; set; }
              public bool Activo { get; set; }
          }
      }
      ```
    * `CrearTipoDocumentoDto.cs`:
      ```csharp
      namespace SIGAD.Application.DTOs
      {
          public class CrearTipoDocumentoDto
          {
              public string Nombre { get; set; }
              public string? Descripcion { get; set; }
          }
      }
      ```
    * `ActualizarTipoDocumentoDto.cs`:
      ```csharp
      namespace SIGAD.Application.DTOs
      {
          public class ActualizarTipoDocumentoDto
          {
              public string Nombre { get; set; }
              public string? Descripcion { get; set; }
              public bool Activo { get; set; }
          }
      }
      ```

2.  **Crear `GestionTipoDocumentoAppService.cs`:**
    * En `SIGAD.Application`, dentro de la carpeta `Services`, crea una nueva clase `GestionTipoDocumentoAppService.cs`:
      ```csharp
      // SIGAD.Application/Services/GestionTipoDocumentoAppService.cs
      using SIGAD.Application.DTOs;
      using SIGAD.Domain.Entities;
      using SIGAD.Domain.Interfaces; // Para ITipoDocumentoRepository
      using System.Collections.Generic;
      using System.Linq;
      using System.Threading.Tasks;
      // Asumiremos que tienes una interfaz IUnitOfWork para SaveChangesAsync()
      // Si no, necesitarías inyectar SigadDbContext aquí para llamar a SaveChangesAsync,
      // lo cual es menos ideal pero funcional para empezar.

      namespace SIGAD.Application.Services
      {
          public class GestionTipoDocumentoAppService
          {
              private readonly ITipoDocumentoRepository _tipoDocumentoRepository;
              // private readonly IUnitOfWork _unitOfWork; // Idealmente

              public GestionTipoDocumentoAppService(ITipoDocumentoRepository tipoDocumentoRepository /*, IUnitOfWork unitOfWork*/)
              {
                  _tipoDocumentoRepository = tipoDocumentoRepository;
                  // _unitOfWork = unitOfWork;
              }

              public async Task<IEnumerable<TipoDocumentoDto>> GetAllTiposDocumentoAsync()
              {
                  var tiposDocumento = await _tipoDocumentoRepository.GetAllAsync();
                  return tiposDocumento.Select(td => new TipoDocumentoDto
                  {
                      Id = td.Id,
                      Nombre = td.Nombre,
                      Descripcion = td.Descripcion,
                      Activo = td.Activo
                  }).ToList();
              }

              public async Task<Guid> CrearTipoDocumentoAsync(CrearTipoDocumentoDto dto)
              {
                  var tipoDocumento = new TipoDocumento(Guid.NewGuid(), dto.Nombre, dto.Descripcion);
                  await _tipoDocumentoRepository.AddAsync(tipoDocumento);
                  // await _unitOfWork.SaveChangesAsync(); // ¡IMPORTANTE! Esto guarda los cambios en la BD.
                  // Si no tienes IUnitOfWork, y EfTipoDocumentoRepository no llama a SaveChanges,
                  // tendrás que añadir lógica para guardar aquí (ej. inyectando DbContext y llamando SaveChanges).
                  // Por ahora, asumiremos que la llamada a SaveChanges() se gestionará.
                  return tipoDocumento.Id;
              }

              public async Task ActualizarTipoDocumentoAsync(Guid id, ActualizarTipoDocumentoDto dto)
              {
                  var tipoDocumento = await _tipoDocumentoRepository.GetByIdAsync(id);
                  if (tipoDocumento == null) throw new KeyNotFoundException("Tipo de Documento no encontrado.");

                  tipoDocumento.ActualizarInformacion(dto.Nombre, dto.Descripcion);
                  if (dto.Activo) tipoDocumento.Activar(); else tipoDocumento.Desactivar();
                  
                  await _tipoDocumentoRepository.UpdateAsync(tipoDocumento);
                  // await _unitOfWork.SaveChangesAsync(); // ¡IMPORTANTE!
              }
          }
      }
      ```
    * **Nota sobre `SaveChangesAsync()`:** Para que `AddAsync` y `UpdateAsync` realmente persistan los datos, se debe llamar a `SaveChangesAsync()` del `DbContext`. Idealmente, esto se maneja a través de un patrón "Unit of Work" inyectado en el servicio de aplicación. Si aún no lo tienes, el servicio de aplicación podría necesitar acceso al `DbContext` para llamar a `SaveChangesAsync()` directamente (menos ideal, pero más simple para este tutorial inicial) o la implementación del repositorio podría llamarlo (no recomendado para operaciones de escritura si quieres agrupar cambios). Para este tutorial, asegúrate que `SaveChangesAsync` se llame después de `AddAsync` o `UpdateAsync` en algún punto antes de que el método del servicio de aplicación termine.

#### Fase 3 (Infraestructura): Implementando la Persistencia para `TipoDocumento`

**Objetivo:** Conectar `TipoDocumento` a la base de datos.

1.  **Actualizar `SigadDbContext.cs`:**
    * En `SIGAD.Infrastructure/Persistence/SigadDbContext.cs`, añade un `DbSet` para `TipoDocumento` y configura la entidad en `OnModelCreating`:
      ```csharp
      // ...
      public DbSet<TipoDocumento> TiposDocumento { get; set; }
      // ...
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          base.OnModelCreating(modelBuilder);

          modelBuilder.Entity<Rango>(entity => {/*...*/}); // Configuración de Rango
          modelBuilder.Entity<Rango>().HasData(/*...*/);   // Siembra de Rango

          // Nueva configuración para TipoDocumento
          modelBuilder.Entity<TipoDocumento>(entity =>
          {
              entity.HasKey(td => td.Id);
              entity.Property(td => td.Nombre).IsRequired().HasMaxLength(100);
              entity.Property(td => td.Descripcion).HasMaxLength(500);
              entity.Property(td => td.Activo).IsRequired();
          });

          // (Opcional) Sembrar algunos Tipos de Documento iniciales (¡USA GUIDs ESTÁTICOS!)
          modelBuilder.Entity<TipoDocumento>().HasData(
              new TipoDocumento(new Guid("TU_GUID_ESTATICO_1"), "Certificado de Publicación", "Artículo, libro, etc."),
              new TipoDocumento(new Guid("TU_GUID_ESTATICO_2"), "Diploma Académico", "Pregrado, Maestría, Doctorado")
          );
      }
      ```

2.  **Implementar `EfTipoDocumentoRepository.cs`:**
    * En `SIGAD.Infrastructure/Repositories/`, crea `EfTipoDocumentoRepository.cs`:
      ```csharp
      // SIGAD.Infrastructure/Repositories/EfTipoDocumentoRepository.cs
      using Microsoft.EntityFrameworkCore;
      using SIGAD.Domain.Entities;
      using SIGAD.Domain.Interfaces;
      using SIGAD.Infrastructure.Persistence;
      using System.Collections.Generic;
      using System.Threading.Tasks;

      namespace SIGAD.Infrastructure.Repositories
      {
          public class EfTipoDocumentoRepository : ITipoDocumentoRepository
          {
              private readonly SigadDbContext _context;

              public EfTipoDocumentoRepository(SigadDbContext context)
              {
                  _context = context;
              }

              public async Task<TipoDocumento?> GetByIdAsync(Guid id) =>
                  await _context.TiposDocumento.FindAsync(id);

              public async Task<IEnumerable<TipoDocumento>> GetAllAsync() =>
                  await _context.TiposDocumento.Where(td => td.Activo).ToListAsync(); // Solo activos por defecto

              public async Task AddAsync(TipoDocumento tipoDocumento) =>
                  await _context.TiposDocumento.AddAsync(tipoDocumento);

              public Task UpdateAsync(TipoDocumento tipoDocumento)
              {
                  _context.TiposDocumento.Update(tipoDocumento); // EF Core rastrea y marca como modificado
                  return Task.CompletedTask;
              }
          }
      }
      ```

3.  **Añadir y Aplicar Migración:**
    * Abre la Consola del Administrador de Paquetes (PMC). Proyecto predeterminado: `SIGAD.Infrastructure`.
    * `Add-Migration AddTipoDocumentoEntityAndSeed -StartupProject SIGAD.WebAPI`
    * `Update-Database -StartupProject SIGAD.WebAPI`
    * **Revisión Importante para `SaveChangesAsync()`:** Si tu `AddAsync` o `UpdateAsync` en el repositorio NO llaman a `_context.SaveChangesAsync()`, entonces tu `GestionTipoDocumentoAppService` DEBE hacerlo. La forma más simple (pero menos ideal para transacciones complejas) es inyectar `SigadDbContext` también en el servicio de aplicación y llamar `await _context.SaveChangesAsync()` después de las operaciones de repositorio. Una mejor solución a largo plazo es el patrón Unit of Work.
        * Ejemplo de cómo hacerlo en el App Service si no tienes Unit of Work:
          ```csharp
          // En GestionTipoDocumentoAppService.cs
          // private readonly SigadDbContext _context; // Inyectar también el DbContext
          // public GestionTipoDocumentoAppService(ITipoDocumentoRepository repo, SigadDbContext context)
          // { _tipoDocumentoRepository = repo; _context = context; }
          //
          // public async Task<Guid> CrearTipoDocumentoAsync(CrearTipoDocumentoDto dto)
          // {
          //      var tipoDocumento = new TipoDocumento(Guid.NewGuid(), dto.Nombre, dto.Descripcion);
          //      await _tipoDocumentoRepository.AddAsync(tipoDocumento);
          //      await _context.SaveChangesAsync(); // <--- LLAMADA EXPLÍCITA
          //      return tipoDocumento.Id;
          // }
          ```

#### Fase 4 (Web API): Exponiendo Endpoints para `TipoDocumento`

**Objetivo:** Crear endpoints HTTP para `TipoDocumento`.

1.  **Actualizar `Program.cs` en `SIGAD.WebAPI` para Inyección de Dependencias:**
    ```csharp
    // ...
    builder.Services.AddScoped<ITipoDocumentoRepository, EfTipoDocumentoRepository>();
    builder.Services.AddScoped<GestionTipoDocumentoAppService>();
    // ...
    ```

2.  **Crear `TiposDocumentoController.cs`:**
    * En `SIGAD.WebAPI/Controllers/`, crea `TiposDocumentoController.cs`:
      ```csharp
      // SIGAD.WebAPI/Controllers/TiposDocumentoController.cs
      using Microsoft.AspNetCore.Mvc;
      using SIGAD.Application.DTOs;
      using SIGAD.Application.Services;
      using System;
      using System.Collections.Generic;
      using System.Threading.Tasks;

      namespace SIGAD.WebAPI.Controllers
      {
          [ApiController]
          [Route("api/[controller]")]
          public class TiposDocumentoController : ControllerBase
          {
              private readonly GestionTipoDocumentoAppService _appService;

              public TiposDocumentoController(GestionTipoDocumentoAppService appService)
              {
                  _appService = appService;
              }

              [HttpGet]
              public async Task<ActionResult<IEnumerable<TipoDocumentoDto>>> GetAll()
              {
                  return Ok(await _appService.GetAllTiposDocumentoAsync());
              }

              [HttpPost]
              public async Task<ActionResult<Guid>> Create([FromBody] CrearTipoDocumentoDto dto)
              {
                  try
                  {
                      var id = await _appService.CrearTipoDocumentoAsync(dto);
                      // Devolver un 201 Created con la ubicación del nuevo recurso (GetById no implementado aún)
                      return CreatedAtAction(nameof(GetAll), new { id }, id); // Temporalmente apunta a GetAll
                  }
                  catch (ArgumentException ex)
                  {
                      return BadRequest(ex.Message);
                  }
              }

              [HttpPut("{id}")]
              public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarTipoDocumentoDto dto)
              {
                  try
                  {
                      await _appService.ActualizarTipoDocumentoAsync(id, dto);
                      return NoContent(); // 204 No Content indica éxito sin cuerpo de respuesta
                  }
                  catch (KeyNotFoundException ex)
                  {
                      return NotFound(ex.Message);
                  }
                  catch (ArgumentException ex)
                  {
                      return BadRequest(ex.Message);
                  }
              }
          }
      }
      ```

3.  **Probar con Swagger:** Ejecuta `SIGAD.WebAPI` y usa Swagger UI para probar los nuevos endpoints de `api/tiposdocumento`.

#### Fase 5 (Blazor UI): Mostrando y Creando `TipoDocumento`

**Objetivo:** Crear una página para ver y agregar Tipos de Documento.

1.  **Añadir DTOs a `SIGAD.BlazorApp/Models/`:**
    * Crea `TipoDocumentoDto.cs` y `CrearTipoDocumentoDto.cs` en el proyecto Blazor (pueden ser copias de los de la capa de Aplicación, o idealmente de una librería compartida).

2.  **Crear `GestionTiposDocumento.razor`:**
    * En `SIGAD.BlazorApp/Pages/`, crea `GestionTiposDocumento.razor`:
      ```html
      @page "/gestionar-tipos-documento"
      @using SIGAD.BlazorApp.Models // O el namespace de tus DTOs en Blazor
      @inject HttpClient Http
      @inject NavigationManager NavigationManager

      <PageTitle>Gestionar Tipos de Documento</PageTitle>

      <h3>Gestionar Tipos de Documento</h3>

      <h4>Crear Nuevo Tipo de Documento</h4>
      <EditForm Model="@nuevoTipoDocumento" OnValidSubmit="@HandleCrearSubmit">
          <DataAnnotationsValidator />
          <ValidationSummary />

          <div class="mb-3">
              <label for="nombre" class="form-label">Nombre:</label>
              <InputText id="nombre" class="form-control" @bind-Value="nuevoTipoDocumento.Nombre" />
          </div>
          <div class="mb-3">
              <label for="descripcion" class="form-label">Descripción (Opcional):</label>
              <InputTextArea id="descripcion" class="form-control" @bind-Value="nuevoTipoDocumento.Descripcion" />
          </div>
          <button type="submit" class="btn btn-primary">Guardar Nuevo</button>
      </EditForm>
      @if (!string.IsNullOrWhiteSpace(crearError)) { <p class="text-danger">@crearError</p> }
      @if (crearExitoso) { <p class="text-success">¡Tipo de documento creado exitosamente!</p> }


      <h4 class="mt-4">Lista de Tipos de Documento Existentes</h4>
      @if (tiposDocumento == null)
      {
          <p><em>Cargando...</em></p>
      }
      else
      {
          <table class="table">
              <thead><tr><th>Nombre</th><th>Descripción</th><th>Activo</th></tr></thead>
              <tbody>
                  @foreach (var tipo in tiposDocumento)
                  {
                      <tr><td>@tipo.Nombre</td><td>@tipo.Descripcion</td><td>@(tipo.Activo ? "Sí" : "No")</td></tr>
                  }
              </tbody>
          </table>
      }

      @code {
          private List<TipoDocumentoDto>? tiposDocumento;
          private CrearTipoDocumentoDto nuevoTipoDocumento = new();
          private string? crearError;
          private bool crearExitoso = false;

          protected override async Task OnInitializedAsync()
          {
              await CargarTiposDocumento();
          }

          private async Task CargarTiposDocumento()
          {
              try
              {
                  tiposDocumento = await Http.GetFromJsonAsync<List<TipoDocumentoDto>>("api/tiposdocumento");
              }
              catch (Exception ex)
              {
                  Console.WriteLine($"Error cargando tipos de documento: {ex.Message}");
                  // Manejar error en UI si es necesario
              }
          }

          private async Task HandleCrearSubmit()
          {
              crearError = null;
              crearExitoso = false;
              try
              {
                  var response = await Http.PostAsJsonAsync("api/tiposdocumento", nuevoTipoDocumento);
                  if (response.IsSuccessStatusCode)
                  {
                      crearExitoso = true;
                      nuevoTipoDocumento = new CrearTipoDocumentoDto(); // Limpiar formulario
                      await CargarTiposDocumento(); // Recargar lista
                  }
                  else
                  {
                      crearError = $"Error al crear: {await response.Content.ReadAsStringAsync()}";
                  }
              }
              catch (Exception ex)
              {
                  crearError = $"Excepción al crear: {ex.Message}";
              }
          }
      }
      ```

3.  **Añadir Enlace en `NavMenu.razor`:**
    ```html
    <div class="nav-item px-3">
        <NavLink class="nav-link" href="gestionar-tipos-documento">
            <span class="oi oi-document" aria-hidden="true"></span> Tipos de Documento
        </NavLink>
    </div>
    ```

4.  **Ejecutar y Probar:** Inicia ambos proyectos (`SIGAD.WebAPI` y `SIGAD.BlazorApp`) y prueba la nueva página.

---
### Revisión del Flujo y Principios Arquitectónicos

Al implementar esta funcionalidad, habrás practicado:
* Crear entidades y definir contratos en el **Dominio**.
* Orquestar lógica y transformar datos en la capa de **Aplicación**.
* Implementar acceso a datos y manejar migraciones en **Infraestructura**.
* Exponer funcionalidades de forma segura y estándar en la **Web API**.
* Consumir la API y presentar una interfaz interactiva en **Blazor UI**.

Este flujo es el que seguirán para la mayoría de las funcionalidades del proyecto.

---
### Buenas Prácticas y Siguientes Pasos
* **Unit of Work:** Consideren implementar formalmente el patrón Unit of Work para gestionar `SaveChangesAsync()` de forma centralizada en la capa de Aplicación.
* **Validación:** Añadan validación más robusta (ej. FluentValidation).
* **Manejo de Errores:** Mejoren el manejo de errores y feedback al usuario.
* **Pruebas:** Comiencen a pensar en pruebas unitarias y de integración.

---