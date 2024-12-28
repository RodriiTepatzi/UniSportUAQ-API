using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.DTO;
using System.Text;



namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/v1/cartasLiberacion")]
    public class CartasLiberacionController : Controller
    {

        private readonly ICartasLiberacionService _cartasLiberacionService;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;
        private readonly IInscriptionsService _inscriptionsService;
        private readonly IInstructorsService _instructorsService;

        public CartasLiberacionController(ICartasLiberacionService cartasLiberacionService, ICoursesService coursesService, IStudentsService studentsService, IInscriptionsService inscriptionsService, IInstructorsService instructorsService)
        {

            _cartasLiberacionService = cartasLiberacionService;
            _coursesService = coursesService;
            _studentsService = studentsService;
            _inscriptionsService = inscriptionsService;
            _instructorsService = instructorsService;


        }

        [HttpGet]
        [Route("download-pdf/{id}")]
        [Authorize]
        public async Task<IActionResult> DownloadPdf(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var carta = await _cartasLiberacionService.GetByIdAsync(id);

            if (carta != null)
            {

                try
                {
                    var filePath = carta.Url;
                    var fileBytes = System.IO.File.ReadAllBytes(filePath!);
                    //clean file name
                    var fileName = filePath!.Split('\\').Last();
                    return Ok(File(fileBytes, "aplication/pdf", fileName));
                }
                catch
                {
                    return Ok(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound });
                }

            }

            return Ok(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound });
        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByIdAsync(string id)
        {

            if (!Guid.TryParse(id, out _)) return BadRequest(new BaseResponse<CartaLiberacionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _cartasLiberacionService.GetByIdAsync(id, c => c.Course!, c => c.Student!);

            if (result is not null)
            {
                var cartaDTO = new CartaLiberacionDTO
                {
                    Id = result.Id,
                    InstructorId = result.Course!.InstructorId,
                    StudentId = result.StudentId,
                    Name = result.Student!.Name,
                    CourseId = result.CourseId,
                    CourseName = result.Course.CourseName,
                    EndDate = result.Course?.EndDate.Date.ToString(),
                    Url = result.Url
                };

                return Ok(new BaseResponse<CartaLiberacionDTO> { Data = cartaDTO, Error = null });
            }

            return Ok(new BaseResponse<CartaLiberacionDTO> { Data = null, Error = ResponseErrors.DataNotFound });

        }


        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByCourseIdAsync(string courseId)
        {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new BaseResponse<CartaLiberacionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _cartasLiberacionService.GetAllAsync(c => c.CourseId == courseId, c => c.Course!, c => c.Student!);

            var data = new List<CartaLiberacionDTO>();

            foreach (var item in result)
            {

                var cartaDTO = new CartaLiberacionDTO
                {
                    Id = item.Id,
                    InstructorId = item.Course?.InstructorId,
                    StudentId = item.StudentId,
                    Name = item.Student?.Name,
                    CourseId = item.CourseId,
                    CourseName = item.Course?.CourseName,
                    EndDate = item.Course?.EndDate.Date.ToString(),
                    Url = item.Url
                };

                data.Add(cartaDTO);
            }

            return Ok(new BaseResponse<List<CartaLiberacionDTO>> { Data = data, Error = null });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetCartaByStudentIdAsync(string studentid)
        {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new BaseResponse<CartaLiberacionDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _cartasLiberacionService.GetAllAsync(c => c.StudentId == studentid, c => c.Course!, c => c.Student!);

            var Data = new List<CartaLiberacionDTO>();

            foreach (var item in result)

            {
                var cartaDTO = new CartaLiberacionDTO
                {
                    Id = item.Id,
                    InstructorId = item.Course?.InstructorId,
                    StudentId = item.StudentId,
                    Name = item.Student?.Name,
                    CourseId = item.CourseId,
                    CourseName = item.Course?.CourseName,
                    EndDate = item.Course?.EndDate.Date.ToString(),
                    Url = item.Url
                };

                Data.Add(cartaDTO);
            }

            return Ok(new BaseResponse<List<CartaLiberacionDTO>> { Data = Data, Error = null });
        }

        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateCartaAsync([FromBody] CartaLiberacionSchema schema)
        {



            //check if student and course exist
            if (await _studentsService.GetByIdAsync(schema.StudentId!) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });
            if (await _coursesService.GetByIdAsync(schema.CourseId!) is null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFound });

            //check if student is inscribed
            var isInscribed = await _inscriptionsService.GetAllAsync(i => i.CourseId == schema.CourseId! && i.StudentId == schema.StudentId!,
                i => i.Student!,
                i => i.Course!
            );


            if (!isInscribed.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFoundInscription });

            //check if ulid exist
            string id = Ulid.NewUlid().ToString().Substring(0, 7);
            var carta1 = await _cartasLiberacionService.GetByIdAsync(id);

            while (carta1 != null && carta1.Id == id) 
            {
                id = Ulid.NewUlid().ToString().Substring(0, 7);
                carta1 = await _cartasLiberacionService.GetByIdAsync(id);
            }

            //check existance and limit of liberation
            var result = await _cartasLiberacionService.GetAllAsync(c => c.StudentId == schema.StudentId!, c => c.Course!, c => c.Student!);

            if (result is not null && result.Count() >= 3) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CartasAlreadyExist });

            foreach (CartaLiberacion carta in result!)
            {
                if (carta.CourseId == schema.CourseId && carta.StudentId == schema.StudentId) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CartasAlreadyExist });
            }

            //get student course and instructor
            var course = await _coursesService.GetByIdAsync(schema.CourseId!);
            if (course == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFound });

            var student = await _studentsService.GetByIdAsync(schema.StudentId!);
            if (student == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.AuthUserNotFound });

            var instructor = await _instructorsService.GetByIdAsync(course.InstructorId!);
            if (instructor == null || instructor.IsInstructor == false) return Ok(new BaseResponse<bool> { Error = ResponseErrors.UserNotAnInstructor });

            var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                i.StudentId == schema.StudentId! &&
                i.CourseId == schema.CourseId!,
                i => i.Student!,
                i => i.Course!
            );
            if (course == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.InscriptionNotAccredit });

            var inscript = inscriptions.FirstOrDefault();

            //check if concluded and aprobed course by inscription

            if (inscript != null)
            {
                if (inscript!.IsFinished is false) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CourseHasNotEnded });
                if (inscript!.Accredit is false) return Ok(new BaseResponse<bool> { Error = ResponseErrors.InscriptionNotAccredit });

                

                try
                {
                    var data = new CartaModel
                    {
                        Id = id,
                        Expediente = student!.Expediente,
                        StudentName = student.FullName,
                        Grupo = student.Group,
                        StudyPlan = student.StudyPlan,
                        CourseName = course!.CourseName,
                        InstructorName = instructor!.FullName
                    };


                    //Generate byteArray
                    byte[] streamBytes = GeneratePDf(data);

                    if (streamBytes.Length < 1) return Ok(new BaseResponse<bool> {  Error = ResponseErrors.CartasErrorGenerating });

                    //convert to memory stream
                    MemoryStream stream = new MemoryStream(streamBytes);

                    //generate fileaname
                    string filename = student!.Expediente + "_" + course!.CourseName + "_" + course.Id + ".pdf";

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

                    stream.Close();


                    //save in hangfire
                    //string? url = await _cartasLiberacionService.UploadLetterAsync(stream, filename);


                    //create bew object carta
                    var carta = new CartaLiberacion
                    {

                        Id = id,
                        CourseId = schema.CourseId!,
                        StudentId = schema.StudentId!,
                        Url = localPath,
                        InscriptionId = inscript.Id,

                    };

                    inscript.CartaId = carta.Id;

                    var inscriptUpdt = await _inscriptionsService.UpdateAsync(inscript);
                    if (inscriptUpdt == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });


                    var cartaRegister = await _cartasLiberacionService.AddAsync(carta);

                    if (cartaRegister != null) return Ok(new BaseResponse<bool> { Data = true });

                    return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

                }
                catch
                {
                    return BadRequest(new BaseResponse<bool> {  Error = ResponseErrors.CartasErrorGenerating });
                }
            }
            return Ok(new BaseResponse<bool> {  Error = ResponseErrors.CartasErrorGenerating });
        }


        //generate all the cartas liberacion

        [HttpPost]
        [Route("generateAllCartas/course/{courseId}")]
        [Authorize]

        public async Task<IActionResult> GenerateAllCartasByCourseId(string courseId)
        {

            //expedientes carta that could not generate
            List<string> FailedExpedientes = new List<string>();

            //check course exist
            var course = await _coursesService.GetByIdAsync(courseId);

            //check if ended
            if (course == null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFound });

            //return course is not over yet
            if (course.IsActive == true) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHasNotEnded });

            //get all ins for that course

            var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                i.CourseId == courseId &&
                i.UnEnrolled == false &&
                i.Accredit == true,
                i => i.Course!,
                i => i.Student!,
                i => i.Course!.Instructor!);

            //return not found inscriptions related to this course
            if (!inscriptions.Any()) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.CourseNoneInscription });

            foreach (var inscription in inscriptions)
            {

                string id = Ulid.NewUlid().ToString().Substring(0, 7);
                var carta1 = await _cartasLiberacionService.GetByIdAsync(id);

                while (carta1 != null && carta1.Id == id)
                {
                    id = Ulid.NewUlid().ToString().Substring(0, 7);
                    carta1 = await _cartasLiberacionService.GetByIdAsync(id);
                }

                if (inscription.CartaId == null)
                {

                    try
                    {
                       
                        var data = new CartaModel
                        {
                            Id = id,
                            Expediente = inscription.Student!.Expediente,
                            StudentName = inscription.Student!.FullName,
                            Grupo = inscription.Student!.Group,
                            StudyPlan = inscription.Student!.StudyPlan,
                            CourseName = inscription.Course!.CourseName,
                            InstructorName = inscription.Course!.Instructor!.FullName
                        };


                        //Generate byteArray
                        byte[] streamBytes = GeneratePDf(data);

                        if (streamBytes.Length < 1) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Error generating carta" });

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

                            Id = id,
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
            if (FailedExpedientes.Any()) return Ok(new BaseResponse<List<string>> { Data = FailedExpedientes, Error = ResponseErrors.CartasAlreadyExist });

            return Ok(new BaseResponse<bool> { Data = true });


        }




        //create carta local

        private byte[] GeneratePDf(CartaModel data)
        {


            using (MemoryStream outputStream = new MemoryStream())
            {
                byte[] outbyte = new byte[0];


                try
                {
                    // Ruta del archivo PDF de salida
                    //string outputPath = @"carta_" + student.Expediente + "_" + course.CourseName + ".pdf";

                    // Crear un documento PDF
                    Document document = new Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, outputStream);

                    writer.SetEncryption(
                        null, // Contraseña de usuario (null permite abrir el PDF sin contraseña)
                        Encoding.UTF8.GetBytes("deportestroyanos#1"), // Contraseña del propietario
                        PdfWriter.ALLOW_PRINTING, // Permisos permitidos (solo permitir impresión)
                        PdfWriter.ENCRYPTION_AES_128 // Tipo de cifrado
                    );
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

                    Paragraph id = new Paragraph 
                    {
                        new Phrase("Numero de serie: " + data.Id, bodyFont)
                    };
                    id.SpacingBefore = 50;
                    document.Add(id);
                    // Cerrar el documento
                    document.Close();

                    //convertir a bytes[]
                    outbyte = outputStream.ToArray();


                    // Devolver el MemoryStream
                    return outbyte;

                }
                catch (Exception EX)
                {
                    Console.WriteLine("EXCEPTION:" + EX);

                    return outbyte;
                }
            }


        }


    }
}
