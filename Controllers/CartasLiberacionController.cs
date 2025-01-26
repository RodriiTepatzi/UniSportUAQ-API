using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Base;
using Microsoft.EntityFrameworkCore.Query.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UniSportUAQ_API.Data.DTO;
using System.Text;
using System.Security.Policy;
using Microsoft.Extensions.Hosting.Internal;



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
		private readonly IWebHostEnvironment _hostingEnvironment;

		public CartasLiberacionController(ICartasLiberacionService cartasLiberacionService, ICoursesService coursesService, IStudentsService studentsService, IInscriptionsService inscriptionsService, IInstructorsService instructorsService, IWebHostEnvironment hostingEnvironment)
        {

            _cartasLiberacionService = cartasLiberacionService;
            _coursesService = coursesService;
            _studentsService = studentsService;
            _inscriptionsService = inscriptionsService;
            _instructorsService = instructorsService;
			_hostingEnvironment = hostingEnvironment;

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
                catch { 
                    return Ok(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound});
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
		[Route("verify/{code}")]
		[Authorize]
		public async Task<IActionResult> VerifyCarta(string code)
		{
			var exists = await _cartasLiberacionService.GetAllAsync(c => c.VerificationCode == code, c => c.Course!, c => c.Student!, c => c.Course!.Instructor!);

			if (!exists.Any()) return Ok(new BaseResponse<bool> { Error = ResponseErrors.DataNotFound });

			var data = exists.Select(item => new CartaVerificationDTO
			{
				Id = item.Id,
				VerificationCode = item.VerificationCode,
				Url = item.Url,
				Student = new UserDTO
				{
					Id = item.Student!.Id,
					Expediente = item.Student!.Expediente,
					PictureUrl = item.Student!.PictureUrl,
					Name = item.Student!.Name,
					LastName = item.Student!.LastName,
					IsAdmin = item.Student!.IsAdmin,
					IsInstructor = item.Student!.IsInstructor,
					IsStudent = item.Student!.IsStudent,
				},
				Instructor = new UserDTO
				{
					Id = item.Course!.Instructor!.Id,
					Expediente = item.Course!.Instructor!.Expediente,
					PictureUrl = item.Course!.Instructor!.PictureUrl,
					Name = item.Course!.Instructor!.Name,
					LastName = item.Course!.Instructor!.LastName,
					IsAdmin = item.Course!.Instructor!.IsAdmin,
					IsInstructor = item.Course!.Instructor!.IsInstructor,
					IsStudent = item.Course!.Instructor!.IsStudent,
				},
				Course = new CourseDTO
				{
					Id = item.Course!.Id,
					CourseName = item.Course!.CourseName,
					StartDate = item.Course!.StartDate,
					EndDate = item.Course!.EndDate,
				}

			}).First();

			return Ok(new BaseResponse<CartaVerificationDTO>
			{
				Data = data
			});
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
                    EndDate = item.Course?.EndDate.ToString("s"),
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

            //check if carta exist


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

				var verificationCode = string.Empty;

				bool isUnique = false;

				do
				{
					verificationCode = GenerateCode();

					var verificationCodeExists = await _cartasLiberacionService.GetAllAsync(c => c.VerificationCode == verificationCode);

					isUnique = !verificationCodeExists.Any();
				}
				while (!isUnique);

				try
                {
                    var data = new CartaModel
                    {
                        Expediente = student!.Expediente,
                        StudentName = student.FullName,
                        Grupo = student.Group,
                        StudyPlan = student.StudyPlan,
                        CourseName = course!.CourseName,
                        InstructorName = instructor!.FullName,
						VerificationCode = verificationCode,
                    };

                    //Generate byteArray
                    byte[] streamBytes = GeneratePDf(data);

                    if (streamBytes.Length < 1) return Ok(new BaseResponse<bool> { Error = ResponseErrors.CartasErrorGenerating });

                    //convert to memory stream
                    MemoryStream stream = new MemoryStream(streamBytes);

					// string filename = student!.Expediente! + "_" + course!.CourseName + "_" + course.Id + ".pdf";
					string filename = student!.Expediente! + "_" + course.Id + ".pdf";

					string baseDirectory = _hostingEnvironment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");

					string folderPath = Path.Combine(baseDirectory, "constancias");
					string guidName = Guid.NewGuid().ToString();

					if (!Directory.Exists(folderPath))
					{
						Directory.CreateDirectory(folderPath);
					}

					string filePath = Path.Combine(folderPath, student!.Expediente! + "_" + course.Id + ".pdf");

					using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
					{
						await fileStream.WriteAsync(streamBytes, 0, streamBytes.Length);
						await fileStream.FlushAsync();
					}

					// var url = $"/constancias/{student!.Expediente!}_{course!.CourseName}_{course.Id}.pdf";
					var url = $"/constancias/{student!.Expediente!}_{course.Id}.pdf";


					//save in hangfire
					//string? url = await _cartasLiberacionService.UploadLetterAsync(stream, filename);


					var carta = new CartaLiberacion
                    {

                        Id = Guid.NewGuid().ToString(),
                        CourseId = schema.CourseId!,
                        StudentId = schema.StudentId!,
                        Url = url,
                        InscriptionId = inscript.Id,
						VerificationCode = verificationCode
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
                    return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CartasErrorGenerating });
                }
            }
            return Ok(new BaseResponse<bool> { Error = ResponseErrors.CartasErrorGenerating });
        }


        //generate all the cartas liberacion

        [HttpPost]
        [Route("generateAllCartas/course/{courseId}")]
        [Authorize]

        public async Task<IActionResult> GenerateAllCartasByCourseId(string courseId)
        {

            //expedientes carta that could not generate
            List<string> FailedExpedientes = new List<string>();

            //check cours exist
            var course = await _coursesService.GetByIdAsync(courseId);

            //check if ended
            if (course == null) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.CourseNotFound});

            //return course is not over yet
            if (course.IsActive == true) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.CourseHasNotEnded});

            //get all ins for that course

            var inscriptions = await _inscriptionsService.GetAllAsync(i =>
                i.CourseId == courseId && 
                i.UnEnrolled == false &&
                i.Accredit == true,
                i => i.Course!,
                i => i.Student!,
                i => i.Course!.Instructor!);

            //return not found inscriptions related to this course
            if (!inscriptions.Any()) return NotFound(new BaseResponse<bool> { Error = ResponseErrors.CourseNoneInscription});

            foreach (var inscription in inscriptions)
            {

                if (inscription.CartaId == null)
                {
					var verificationCode = string.Empty;

					bool isUnique = false;

					do
					{
						verificationCode = GenerateCode();

						var verificationCodeExists = await _cartasLiberacionService.GetAllAsync(c => c.VerificationCode == verificationCode);

						isUnique = !verificationCodeExists.Any();
					}
					while (!isUnique);

					try
                    {
                        var data = new CartaModel
                        {
                            Expediente = inscription.Student!.Expediente,
                            StudentName = inscription.Student!.FullName,
                            Grupo = inscription.Student!.Group,
                            StudyPlan = inscription.Student!.StudyPlan,
                            CourseName = inscription.Course!.CourseName,
                            InstructorName = inscription.Course!.Instructor!.FullName,
							VerificationCode = verificationCode
						};

                        //Generate byteArray
                        byte[] streamBytes = GeneratePDf(data);

                        if (streamBytes.Length < 1) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Error generating carta" });

                        //convert to memory stream
                        MemoryStream stream = new MemoryStream(streamBytes);

						//generate fileaname
						// string filename = inscription.Student!.Expediente! + "_" + course!.CourseName +"_"+ course.Id + ".pdf";
						string filename = inscription.Student!.Expediente! + "_" + course.Id + ".pdf";

						string baseDirectory = _hostingEnvironment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");

						string folderPath = Path.Combine(baseDirectory, "constancias");
						string guidName = Guid.NewGuid().ToString();

						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(folderPath);
						}

						string filePath = Path.Combine(folderPath, inscription.Student!.Expediente! + "_" + course.Id + ".pdf");

						using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
						{
							await fileStream.WriteAsync(streamBytes, 0, streamBytes.Length);
							await fileStream.FlushAsync();
						}

						// var url = $"/constancias/{inscription.Student!.Expediente!}_{course!.CourseName}_{course.Id}.pdf";
						var url = $"/constancias/{inscription.Student!.Expediente!}_{course.Id}.pdf";


						//save in hangfire
						//string? url = await _cartasLiberacionService.UploadLetterAsync(stream, filename);


						//create bew object carta
						var carta = new CartaLiberacion
                        {

                            Id = Guid.NewGuid().ToString(),
                            CourseId = inscription.CourseId!,
                            StudentId = inscription.StudentId!,
                            Url = url,
                            InscriptionId = inscription.Id,
							VerificationCode = verificationCode
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
                else {
                    FailedExpedientes.Add(inscription.Student!.Expediente! + ": already has a carta liberacion");
                }

            }
            if (FailedExpedientes.Any()) return Ok(new BaseResponse<List<string>> { Data = FailedExpedientes, Error = ResponseErrors.CartasAlreadyExist });

            return Ok(new BaseResponse<bool> { Data = true });


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

					Paragraph verificationCode = new Paragraph
					{
						new Phrase("Código de Verificación:\n", subHeaderFont),
						new Phrase(data.VerificationCode + "\n\n", bodyFont) 
					};

					verificationCode.Alignment = Element.ALIGN_CENTER;
					document.Add(verificationCode);

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



		private string GenerateCode()
		{
			int length = 10;

			const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			StringBuilder codeBuilder = new StringBuilder();
			Random random = new Random();

			for (int i = 0; i < length; i++)
			{
				int index = random.Next(characters.Length);
				codeBuilder.Append(characters[index]);
			}

			return codeBuilder.ToString();
		}
	}
}
