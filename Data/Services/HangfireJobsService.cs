using iTextSharp.text.pdf;
using iTextSharp.text;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniSportUAQ_API.Data.Services
{
    public class HangfireJobsService : IHangfireJobsService
    {

        private readonly IInscriptionsService _inscriptionsService;
        private readonly ICartasLiberacionService _cartasLiberacionService;
        private readonly ICoursesService _coursesService;
        public HangfireJobsService(IInscriptionsService inscriptionsService, ICartasLiberacionService cartasLiberacionService, ICoursesService coursesService)
        {

            _inscriptionsService = inscriptionsService;
            _cartasLiberacionService = cartasLiberacionService;
            _coursesService = coursesService;
        }

        public async Task GenerateAllCartasAsync(string courseId)
        {


            //expedientes carta that could not generate
            List<string> FailedExpedientes = new List<string>();

            //check cours exist
            var course = await _coursesService.GetByIdAsync(courseId);

            //check if ended
            if (course != null)
            {
                if (course.IsActive == false)
                {
                    var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                    i.CourseId == courseId &&
                    i.UnEnrolled == false &&
                    i.Accredit == true,
                    i => i.Course!,
                    i => i.Student!,
                    i => i.Course!.Instructor!);

                    //return not found inscriptions related to this course
                    if (inscriptions.Any())
                    {
                        foreach (var inscription in inscriptions)
                        {

                            if (inscription.CartaId == null)
                            {

                                try
                                {
                                    var data = new CartaModel
                                    {
                                        Expediente = inscription.Student!.Expediente,
                                        StudentName = inscription.Student!.FullName,
                                        Grupo = inscription.Student!.Group,
                                        StudyPlan = inscription.Student!.StudyPlan,
                                        CourseName = inscription.Course!.CourseName,
                                        InstructorName = inscription.Course!.Instructor!.FullName
                                    };

                                    //Generate byteArray
                                    byte[] streamBytes = GeneratePDf(data);

                                    if (streamBytes.Length > 0)
                                    {

                                        //convert to memory stream
                                        MemoryStream stream = new MemoryStream(streamBytes);

                                        //generate fileaname
                                        string filename = inscription.Student!.Expediente! + "_" + course!.CourseName + "_" + course.Id + ".pdf";

                                        //get file route
                                        string projectPath = Directory.GetCurrentDirectory();
                                        string folderPath = Path.Combine(projectPath, "CartasLiberacion");

                                        // Crear la carpeta si no existe
                                        if (!Directory.Exists(folderPath))
                                        {
                                            Directory.CreateDirectory(folderPath);
                                        }

                                        string localPath = Path.Combine(folderPath, filename);
                                        await System.IO.File.WriteAllBytesAsync(localPath, streamBytes);


                                        //save in hangfire
                                        //string? url = await _cartasLiberacionService.UploadLetterAsync(stream, filename);


                                        //create bew object carta
                                        var carta = new CartaLiberacion
                                        {

                                            Id = Guid.NewGuid().ToString(),
                                            CourseId = inscription.CourseId!,
                                            StudentId = inscription.StudentId!,
                                            Url = localPath,
                                            InscriptionId = inscription.Id,

                                        };

                                        inscription.CartaId = carta.Id;

                                        var inscriptUpdt = await _inscriptionsService.UpdateAsync(inscription);

                                        //add to lits not poisbble generate carta
                                        if (inscriptUpdt == null) FailedExpedientes.Add(data.Expediente! + ": not posible to conect to carta to inscription");


                                        var cartaRegister = await _cartasLiberacionService.AddAsync(carta);

                                        //add to lits not poisbble generate carta
                                        if (cartaRegister == null) FailedExpedientes.Add(data.Expediente! + ": not posibble to save carta");

                                    }
                                    else {
                                        Console.WriteLine($"ERROR GENERATING CARTA FOR {inscription.StudentId} IS NOT POSSIBLE TO GENERATE CARTA");
                                    }

                                    

                                }
                                catch
                                {
                                    FailedExpedientes.Add(inscription.Student!.Expediente! + ": stream error");
                                }
                            }
                            else
                            {
                                FailedExpedientes.Add(inscription.Student!.Expediente! + ": already has a carta liberacion");
                            }

                        }



                    }
                    else
                    {
                        Console.WriteLine($"THIS COURSE {courseId} HAS NOT INSCRIPTIONS TO GENERATE CARTA");

                    }
                    if (FailedExpedientes.Any())
                    {
                        foreach (var data in FailedExpedientes)
                        {
                            Console.WriteLine(data + "\n");
                        }
                    }
                    else
                    {

                        Console.WriteLine($"ENDED CARTAS FOR COURSE {courseId} CORRECTLY");

                    }


                }
                else {

                    Console.WriteLine($"COURSE WITH ID: {courseId} STILL ACTIVE END COURSE AND THEN GENERATE CARTAS");

                }

            }
            else {

                Console.WriteLine($"COURSE WITH ID: {courseId} DOES NOT EXIST");

            }

            
        }

        private byte[] GeneratePDf(CartaModel data)
        {


            using (MemoryStream outputStream = new MemoryStream())
            {


                try
                {
                    // Ruta del archivo PDF de salida
                    //string outputPath = @"carta_" + student.Expediente + "_" + course.CourseName + ".pdf";

                    // Crear un documento PDF
                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, outputStream);
                    document.Open();

                    // Fuentes y estilos
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    Font subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                    Font bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                    // Tabla principal
                    PdfPTable mainTable = new PdfPTable(3);
                    mainTable.WidthPercentage = 100;
                    mainTable.SetWidths(new float[] { 30, 40, 30 });
                    mainTable.SpacingAfter = 20;

                    // Celda de logo
                    PdfPCell logoCell = new PdfPCell();
                    logoCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    logoCell.Border = Rectangle.NO_BORDER;
                    try
                    {
                        Image logo = Image.GetInstance("logouaq.svg"); // Cambiar ruta si es necesario
                        logo.ScalePercent(35f);
                        logoCell.AddElement(logo);
                    }
                    catch
                    {
                        logoCell.AddElement(new Phrase("Logo Here", bodyFont)); // Fallback si la imagen no se encuentra
                    }
                    mainTable.AddCell(logoCell);

                    // Celda de información central
                    PdfPTable infoTable = new PdfPTable(1);
                    infoTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    infoTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    infoTable.AddCell(new Phrase("UNIVERSIDAD AUTONOMA DE QUERETARO", headerFont));
                    infoTable.AddCell(new Phrase("FACULTAD DE INFORMATICA", subHeaderFont));
                    infoTable.AddCell(new Phrase("Av. de las Ciencias S/N, 76230 Juriquilla, Qro.", bodyFont));
                    infoTable.AddCell(new Phrase(DateTime.Now.Date.ToString(), bodyFont)); // Reemplazar con la fecha actual
                    PdfPCell infoCell = new PdfPCell(infoTable);
                    infoCell.Border = Rectangle.NO_BORDER;
                    mainTable.AddCell(infoCell);

                    // Celda de información lateral
                    PdfPTable sideTable = new PdfPTable(1);
                    sideTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    sideTable.AddCell(new Phrase("Expediente: " + data.Expediente, bodyFont)); // Reemplazar con el expediente real
                    sideTable.AddCell(new Phrase("Grupo: " + data.Grupo, bodyFont)); // Reemplazar con el grupo real
                    sideTable.AddCell(new Phrase("Plan de Estudios: " + data.StudyPlan, bodyFont)); // Reemplazar con el plan de estudios real
                    PdfPCell sideCell = new PdfPCell(sideTable);
                    sideCell.Border = Rectangle.NO_BORDER;
                    mainTable.AddCell(sideCell);

                    // Añadir la tabla principal al documento
                    document.Add(mainTable);

                    // Cuerpo del documento
                    Paragraph body = new Paragraph
            {
                new Phrase("Por el presente medio:\n\n", bodyFont),
                new Phrase("Se informa de la liberación en relación con la participación del alumno "+data.StudentName+" con expediente "+data.Expediente+" en el taller "+data.CourseName+". En reconocimiento del cumplimiento satisfactorio con los requisitos establecidos y ha finalizado exitosamente su participación en el taller nos complace emitir este documento de liberación.\n\n", bodyFont),
                new Phrase("Por medio de esta carta, "+data.InstructorName+" declara que el alumno "+data.StudentName+" ha completado el curso correspondiente al taller deportivo con éxito y ha cumplido con todas las obligaciones y responsabilidades requeridas durante su participación en el mismo.\n\n", bodyFont),
                new Phrase("Le agradecemos sinceramente su interés y participación en nuestro taller deportivo.\n\n", bodyFont),
                new Phrase("Esperamos haber contribuido positivamente a su desarrollo y crecimiento en el ámbito deportivo, y le deseamos éxito continuo en sus futuras actividades y metas deportivas.\n", bodyFont)
        };
                    body.SpacingAfter = 20;
                    document.Add(body);

                    // Firma
                    Paragraph firma = new Paragraph
            {
                new Phrase("ATENTAMENTE,\n\n", bodyFont),
                new Phrase(data.InstructorName+"\n", bodyFont), // Reemplazar con el nombre del instructor real
                new Phrase(data.CourseName+"\n", bodyFont) // Reemplazar con el nombre del taller real
        };
                    firma.Alignment = Element.ALIGN_CENTER;
                    firma.SpacingAfter = 50;
                    document.Add(firma);

                    // Tabla de firmas
                    PdfPTable signatureTable = new PdfPTable(2);
                    signatureTable.WidthPercentage = 100;
                    signatureTable.SetWidths(new float[] { 50, 50 });

                    PdfPCell studentSignatureCell = new PdfPCell(new Phrase("Firma del alumno", bodyFont));
                    studentSignatureCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    studentSignatureCell.Border = Rectangle.TOP_BORDER;
                    signatureTable.AddCell(studentSignatureCell);

                    PdfPCell instructorSignatureCell = new PdfPCell(new Phrase("Firma del Instructor", bodyFont));
                    instructorSignatureCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    instructorSignatureCell.Border = Rectangle.TOP_BORDER;
                    signatureTable.AddCell(instructorSignatureCell);

                    // Añadir la tabla de firmas al documento
                    document.Add(signatureTable);

                    // Cerrar el documento
                    document.Close();

                    //convertir a bytes[]
                    byte[] outbyte = outputStream.ToArray();


                    // Devolver el MemoryStream
                    return outbyte;

                }
                catch
                {
                    byte[] outbyte = new byte[0];
                    return outbyte;
                }
            }


        }


    }
}
