using System;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SIGAD.Application.DTOs;
using SIGAD.Application.Interfaces;
using SIGAD.Domain.Entities;

namespace SIGAD.Application.Services
{
    public class AccionPersonalService : IAccionPersonalService
    {
        private readonly IApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public AccionPersonalService(IApplicationDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Genera un documento PDF de acción de personal para un docente promovido
        /// </summary>
        public async Task<byte[]> GenerarAccionPersonalPdfAsync(AccionPersonalDto datos)
        {
            // Validar datos de entrada
            if (string.IsNullOrEmpty(datos.NombreCompleto))
                throw new ArgumentException("El nombre del docente es obligatorio");

            if (string.IsNullOrEmpty(datos.Cedula))
                throw new ArgumentException("La cédula del docente es obligatoria");

            if (string.IsNullOrEmpty(datos.RangoAnterior))
                throw new ArgumentException("El rango anterior es obligatorio");

            if (string.IsNullOrEmpty(datos.RangoNuevo))
                throw new ArgumentException("El rango nuevo es obligatorio");

            // Obtener la fecha actual para el documento
            var fechaActual = DateTime.Now;
            string dia = fechaActual.Day.ToString();
            string mes = ObtenerNombreMes(fechaActual.Month);
            string anio = datos.Anio; // Usamos el año proporcionado en los datos

            // Generar el documento PDF utilizando QuestPDF
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configuración de la página
                    page.Size(PageSizes.Letter);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontFamily("Times New Roman"));

                    // Contenido con decoración minimalista
                    page.Content().Layers(layers =>
                    {
                        // ✅ Marco dorado completo alrededor de toda la página
                        layers.Layer().Border(2).BorderColor("#DAA520");

                        // ✅ Contenido principal completamente centrado
                        layers.PrimaryLayer().AlignCenter().AlignMiddle().Padding(20).Column(column =>
                        {
                            column.Spacing(3);

                            // Encabezado
                            column.Item().AlignCenter().Column(c => 
                            {
                                c.Item().AlignCenter().Text("UNIVERSIDAD TÉCNICA DE AMBATO")
                                    .FontSize(14).Bold();
                                
                                c.Item().AlignCenter().Text("DIRECCIÓN DE TALENTO HUMANO")
                                    .FontSize(12).Bold();
                                    
                                c.Item().Height(6);
                                
                                c.Item().AlignCenter().Text($"ACCIÓN DE PERSONAL Nro. UTA-AP-{datos.Anio}:{datos.Consecutivo}")
                                    .FontSize(12).Bold();
                            });

                            // Espacio después del encabezado
                            column.Item().Height(6);

                            // Texto introductorio
                            column.Item().AlignCenter().Text("El Rector de la Universidad Técnica de Ambato, en uso de sus atribuciones legales y estatutarias,")
                                .FontSize(9);

                            column.Item().Height(4);

                            // Sección VISTOS
                            column.Item().AlignCenter().Text("VISTOS:")
                                .FontSize(10).Bold();

                            // Texto de solicitud
                            column.Item().AlignCenter().Text($"La solicitud de promoción al grado escalafonario inmediato superior, presentada por el/la docente {datos.NombreCompleto}, con cédula de ciudadanía Nro. {datos.Cedula}.")
                                .FontSize(9);

                            column.Item().Height(4);

                            // Sección CONSIDERANDO
                            column.Item().AlignCenter().Text("CONSIDERANDO:")
                                .FontSize(10).Bold();

                            // Lista de consideraciones compacta
                            column.Item().AlignLeft().Text("• Que, el Estatuto de la Universidad Técnica de Ambato confiere al Honorable Consejo Universitario la atribución de aprobar los procesos de promoción del personal académico titular.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text("• Que, el \"REGLAMENTO PARA LA PROMOCIÓN DEL PERSONAL ACADÉMICO TITULAR DE LA UNIVERSIDAD TÉCNICA DE AMBATO\", expedido mediante Resolución 0677-CU-P-2023, establece los requisitos y el procedimiento para el efecto.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text("• Que, la Comisión Académica de Escalafón y Promoción, tras el análisis de la documentación presentada por el/la solicitante, emitió el informe técnico favorable para la promoción.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text($"• Que, el Honorable Consejo Universitario, en sesión de {datos.FechaSesion}, conoció y aprobó el informe de promoción del personal académico titular correspondiente al período {datos.PeriodoConvocatoria}, en el cual consta el/la docente antes mencionado/a.")
                                .FontSize(8);

                            column.Item().Height(4);

                            // Sección RESUELVE
                            column.Item().AlignCenter().Text("RESUELVE:")
                                .FontSize(10).Bold();

                            // Artículos resolutivos compactos
                            column.Item().AlignLeft().Text($"Artículo 1.- PROMOVER, al/a la docente {datos.NombreCompleto}, de la categoría de {datos.RangoAnterior} a la categoría de {datos.RangoNuevo} dentro del escalafón del personal académico titular de la Universidad Técnica de Ambato.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text($"Artículo 2.- DISPONER, que la presente promoción rige a partir del {datos.FechaEfectivaPromocion}, de conformidad con lo establecido en el Artículo 8 del reglamento de la materia.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text($"Artículo 3.- FIJAR, la remuneración mensual unificada correspondiente a la categoría de {datos.RangoNuevo}, conforme a la escala salarial vigente en la Institución.")
                                .FontSize(8);

                            column.Item().AlignLeft().Text("Artículo 4.- ENCARGAR, la ejecución y registro de la presente Acción de Personal a la Dirección de Talento Humano y a la Dirección Financiera para los fines legales y económicos pertinentes.")
                                .FontSize(8);

                            column.Item().Height(4);

                            // Fecha y lugar
                            column.Item().AlignCenter().Text($"Dado y firmado en la ciudad de Ambato, a los {dia} días del mes de {mes} de {anio}.")
                                .FontSize(9);

                            column.Item().AlignCenter().Text("Comuníquese y cúmplase.")
                                .FontSize(9);

                            column.Item().Height(6);

                            // Firmas
                            column.Item().Row(row =>
                            {
                                // Firma del Presidente
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Height(25).Text("[FIRMA PRESIDENTE]").FontSize(7);
                                    c.Item().AlignCenter().Text("PRESIDENTE DEL H. CONSEJO")
                                        .FontSize(6).Bold();
                                    c.Item().AlignCenter().Text("UNIVERSITARIO TÉCNICO DE AMBATO")
                                        .FontSize(6).Bold();
                                });

                                // Espacio entre firmas
                                row.ConstantItem(15);

                                // Firma del Secretario
                                row.RelativeItem().AlignCenter().Column(c =>
                                {
                                    c.Item().AlignCenter().Height(25).Text("[FIRMA SECRETARIO]").FontSize(7);
                                    c.Item().AlignCenter().Text("SECRETARIO GENERAL")
                                        .FontSize(6).Bold();
                                });
                            });
                        });
                    });
                });
            });

            // Generar el PDF como array de bytes
            byte[] pdfBytes;
            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                pdfBytes = stream.ToArray();
            }

            // Guardar el archivo en el almacenamiento
            try
            {
                string nombreArchivo = $"accion_personal_{datos.Cedula}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                var resultado = await _fileStorage.GuardarArchivoDualAsync(
                    pdfBytes, 
                    "acciones_personal", 
                    datos.Cedula, 
                    ".pdf");
            }
            catch (Exception ex)
            {
                // Registrar error pero continuar, ya que el PDF ya se generó
                Console.WriteLine($"Error al guardar el archivo: {ex.Message}");
            }
            
            return pdfBytes;
        }

        /// <summary>
        /// Obtiene el nombre del mes en español a partir de su número
        /// </summary>
        private string ObtenerNombreMes(int numeroMes)
        {
            return numeroMes switch
            {
                1 => "enero",
                2 => "febrero",
                3 => "marzo",
                4 => "abril",
                5 => "mayo",
                6 => "junio",
                7 => "julio",
                8 => "agosto",
                9 => "septiembre",
                10 => "octubre",
                11 => "noviembre",
                12 => "diciembre",
                _ => throw new ArgumentOutOfRangeException(nameof(numeroMes), "El número de mes debe estar entre 1 y 12")
            };
        }
    }
}