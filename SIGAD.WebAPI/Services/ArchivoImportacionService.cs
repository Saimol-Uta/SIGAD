using iText.Kernel.Pdf;
using iText.IO.Source;

namespace SIGAD.WebAPI.Services
{
    /// <summary>
    /// Interfaz para el servicio de importación y procesamiento de archivos PDF.
    /// </summary>
    public interface IArchivoImportacionService
    {
        /// <summary>
        /// Procesa un PDF (compresión si excede 5MB) y lo guarda en el sistema de archivos.
        /// </summary>
        /// <param name="pdfBinario">Contenido binario del PDF</param>
        /// <param name="tipoDocumento">Categoría del documento (ej: "articulos", "cursos")</param>
        /// <param name="identificador">Identificador único del documento</param>
        /// <returns>Ruta relativa del archivo guardado</returns>
        Task<string> ProcesarYGuardarPdfAsync(byte[] pdfBinario, string tipoDocumento, string identificador);
    }

    /// <summary>
    /// Servicio para procesamiento de archivos PDF con compresión automática.
    /// 
    /// ⚠️ DEUDA TÉCNICA - UBICACIÓN EN WebAPI
    /// ========================================
    /// Este servicio está en WebAPI en lugar de Application debido a:
    /// 
    /// 1. DEPENDENCIA: IWebHostEnvironment (específica de ASP.NET Core WebAPI)
    ///    - Accede a WebRootPath para guardar archivos físicos
    ///    - No disponible en Application Layer
    /// 
    /// 2. DEPENDENCIA: iText7 (biblioteca de procesamiento PDF)
    ///    - Librería externa pesada (múltiples paquetes NuGet)
    ///    - Solo necesaria en este punto específico
    ///    - Agregar a Application contaminaría con dependencias innecesarias
    /// 
    /// SOLUCIÓN IDEAL (Refactorización Futura):
    /// - Crear IFileStorageService abstracto en Application
    /// - Implementar PhysicalFileStorageService en Infrastructure
    /// - Crear IPdfCompressionService abstracto en Application
    /// - Implementar IText7PdfCompressionService en Infrastructure
    /// - Mover lógica de orquestación a Application
    /// 
    /// POR AHORA: Aceptable mantener en WebAPI (capa de presentación)
    /// dado que maneja operaciones de I/O específicas de la infraestructura web.
    /// 
    /// Issue: https://github.com/Saimol-Uta/SIGAD/issues/TBD
    /// </summary>
    public class ArchivoImportacionService : IArchivoImportacionService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ArchivoImportacionService> _logger;
        private const int TAMANO_MAXIMO_SIN_COMPRESION = 5 * 1024 * 1024; // 5MB

        public ArchivoImportacionService(IWebHostEnvironment webHostEnvironment, ILogger<ArchivoImportacionService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string> ProcesarYGuardarPdfAsync(byte[] pdfBinario, string tipoDocumento, string identificador)
        {
            try
            {
                // 1. Verificar si necesita compresión
                var pdfFinal = pdfBinario.Length > TAMANO_MAXIMO_SIN_COMPRESION
                    ? ComprimirPdf(pdfBinario)
                    : pdfBinario;

                // 2. Generar nombre único para el archivo
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var nombreArchivo = $"{identificador}_{timestamp}.pdf";
                var rutaRelativa = $"{tipoDocumento}/{nombreArchivo}";

                // 3. Crear directorio si no existe
                var directorioCompleto = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", tipoDocumento);
                Directory.CreateDirectory(directorioCompleto);

                // 4. Guardar físicamente el archivo
                var rutaCompleta = Path.Combine(directorioCompleto, nombreArchivo);
                await File.WriteAllBytesAsync(rutaCompleta, pdfFinal);

                // 5. Log para debugging
                _logger.LogInformation($"Archivo guardado: {rutaRelativa} (Original: {pdfBinario.Length} bytes, Final: {pdfFinal.Length} bytes)");

                // 6. Retornar ruta relativa para guardar en BD
                return rutaRelativa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar archivo PDF para {identificador}");
                throw;
            }
        }

        private byte[] ComprimirPdf(byte[] pdfOriginal)
        {
            try
            {
                using var inputStream = new MemoryStream(pdfOriginal);
                using var outputStream = new MemoryStream();

                // Configurar writer con compresión máxima
                var writerProperties = new WriterProperties();
                writerProperties.SetCompressionLevel(CompressionConstants.BEST_COMPRESSION);
                writerProperties.SetFullCompressionMode(true);

                var writer = new PdfWriter(outputStream, writerProperties);
                var reader = new PdfReader(inputStream);

                // Crear el documento comprimido
                using var pdfDoc = new PdfDocument(reader, writer);
                pdfDoc.Close();

                var pdfComprimido = outputStream.ToArray();

                // Solo retornar comprimido si realmente es más pequeño
                var porcentajeReduccion = ((double)(pdfOriginal.Length - pdfComprimido.Length) / pdfOriginal.Length) * 100;

                _logger.LogInformation($"Compresión PDF: {pdfOriginal.Length} -> {pdfComprimido.Length} bytes ({porcentajeReduccion:F1}% reducción)");

                return pdfComprimido.Length < pdfOriginal.Length ? pdfComprimido : pdfOriginal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al comprimir PDF, usando archivo original");
                return pdfOriginal;
            }
        }
    }
}
