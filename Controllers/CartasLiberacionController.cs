using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;



namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/cartasLiberacion")]
    public class CartasLiberacionController: Controller
    {

        private readonly ICartasLiberacionService _cartasLiberacionService;
        private readonly ICoursesService _coursesService;
        private readonly IStudentsService _studentsService;
        private readonly IInscriptionsService _inscriptionsService;
        private readonly IInstructorsService _instructorsService;

        public CartasLiberacionController(ICartasLiberacionService cartasLiberacionService, ICoursesService coursesService, IStudentsService studentsService, IInscriptionsService inscriptionsService, IInstructorsService instructorsService) {

            _cartasLiberacionService = cartasLiberacionService;
            _coursesService = coursesService;
            _studentsService = studentsService;
            _inscriptionsService = inscriptionsService;
            _instructorsService = instructorsService;


        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByIdAsync(string id) {

            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByIdAsync(id);

            if (result is not null) return Ok(new DataResponse { Data = result.Dictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("course/{courseid}")]
        [Authorize]

        public async Task<IActionResult> GetCartaByCourseIdAsync(string courseId) {

            if (!Guid.TryParse(courseId, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByCourseIdAsync(courseId);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("student/{studentid}")]
        [Authorize]
        public async Task<IActionResult> GetCartaByStudentIdAsync(string studentid) {

            if (!Guid.TryParse(studentid, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _cartasLiberacionService.GetCartaByStudentIdAsync(studentid);

            var Data = new List<Dictionary<string, object>>();

            foreach (var item in result) Data.Add(item.Dictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = Data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateCartaAsync([FromBody] CartaLiberacionSchema schema) {

            

            //check if student and course exist
            if (await _studentsService.GetStudentByIdAsync(schema.StudentId!) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "student:"+ResponseMessages.OBJECT_NOT_FOUND });
            if (await _coursesService.GetCourseByIdAsync(schema.CourseId!) is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "course:" + ResponseMessages.OBJECT_NOT_FOUND });

            //check if student is inscribed
            if (!await _inscriptionsService.CheckInscriptionByCourseIdAndStudentIdAsync(schema.CourseId!, schema.StudentId!)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Inscription:" + ResponseMessages.NOT_FOUND_IN_COURSE});
            
            //check if carta exist


            //check existance and limit of liberation
            var result = await _cartasLiberacionService.GetCartaByStudentIdAsync(schema.StudentId!);

            if (result is not null && result.Count >= 6) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.LIBERATION_LIMIT});

            foreach (CartaLiberacion carta in result!) {

                if (carta.CourseId == schema.CourseId && carta.StudentId == schema.StudentId) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.CARTA_EXIST});


            }

            //get student course and instructor

            var  course = await _coursesService.GetCourseByIdAsync(schema.CourseId!);

            var student = await _studentsService.GetStudentByIdAsync(schema.StudentId!);

            var instructor = await _instructorsService.GetInstructorByIdAsync(schema.InstructorId!);

            var inscriptions = await _inscriptionsService.GetInscriptionByStudentIdAndCourseIdAsync(schema.StudentId!, schema.CourseId!);

            //check if concluded and aprobed course by inscription

            if (inscriptions!.IsFinished is false) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.STUDENT_NOT_ACCREDITED });
            if (inscriptions!.Accredit is false) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.STUDENT_NOT_ACCREDITED });

            //Generate byteArray
            byte[] streamBytes = GeneratePDf(student!, instructor!, course!);

            try { 

                //convert to memory stream
                MemoryStream stream = new MemoryStream(streamBytes);

                //generate fileaname
                string filename = student!.Expediente + "_" + course!.CourseName + ".pdf";

                //firebase upload and get url
                string? url = await _cartasLiberacionService.UploadLetterAsync(stream, filename);


                //create bew object carta
                var carta = new CartaLiberacion
                {

                    Id = Guid.NewGuid().ToString(),
                    CourseId = schema.CourseId!,
                    StudentId = schema.StudentId!,
                    Url = url,

                };

                var cartaRegister = await _cartasLiberacionService.CreateCartaAsync(carta);


                if(cartaRegister != null ) return Ok(new DataResponse { Data = cartaRegister.Dictionary, ErrorMessage = null });

                return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR});

            }
            catch (Exception ex)
            {
                return  BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.STREAM_ERROR });
            }



        }



        //create carta local

        private byte[] GeneratePDf(ApplicationUser student, ApplicationUser instructor, Course course) 
        {


            using (MemoryStream outputStream = new MemoryStream())
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
                catch (Exception ex)
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
                sideTable.AddCell(new Phrase("Expediente: " + student.Expediente, bodyFont)); // Reemplazar con el expediente real
                sideTable.AddCell(new Phrase("Grupo: " + student.Group, bodyFont)); // Reemplazar con el grupo real
                sideTable.AddCell(new Phrase("Plan de Estudios: " + student.StudyPlan, bodyFont)); // Reemplazar con el plan de estudios real
                PdfPCell sideCell = new PdfPCell(sideTable);
                sideCell.Border = Rectangle.NO_BORDER;
                mainTable.AddCell(sideCell);

                // Añadir la tabla principal al documento
                document.Add(mainTable);

                // Cuerpo del documento
                Paragraph body = new Paragraph
            {
                new Phrase("Por el presente medio:\n\n", bodyFont),
            new Phrase("Se informa de la liberación en relación con la participación del alumno "+student.FullName+" con expediente "+student.Expediente+" en el taller "+course.CourseName+". En reconocimiento del cumplimiento satisfactorio con los requisitos establecidos y ha finalizado exitosamente su participación en el taller nos complace emitir este documento de liberación.\n\n", bodyFont),
            new Phrase("Por medio de esta carta, "+instructor.FullName+" declara que el alumno "+student.FullName+" ha completado el curso correspondiente al taller deportivo con éxito y ha cumplido con todas las obligaciones y responsabilidades requeridas durante su participación en el mismo.\n\n", bodyFont),
                new Phrase("Le agradecemos sinceramente su interés y participación en nuestro taller deportivo.\n\n", bodyFont),
            new Phrase("Esperamos haber contribuido positivamente a su desarrollo y crecimiento en el ámbito deportivo, y le deseamos éxito continuo en sus futuras actividades y metas deportivas.\n", bodyFont)
        };
                body.SpacingAfter = 20;
                document.Add(body);

                // Firma
                Paragraph firma = new Paragraph
            {
                new Phrase("ATENTAMENTE,\n\n", bodyFont),
                new Phrase(instructor.FullName+"\n", bodyFont), // Reemplazar con el nombre del instructor real
            new Phrase(course.CourseName+"\n", bodyFont) // Reemplazar con el nombre del taller real
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

        }

       
    }
}
