using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.DTO;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using UniSportUAQ_API.Data;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/v1/subjects")]
    public class SubjectsController : Controller
    {
        private readonly ISubjectsService _subjectsService;
        private readonly IInstructorsService _instructorsService;

        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context, IWebHostEnvironment hostingEnvironment, ISubjectsService subjectsService, IInstructorsService instructorsService)
        {

            _subjectsService = subjectsService;
            _instructorsService = instructorsService;
            _hostingEnvironment = hostingEnvironment;
            _context = context;

        }

        [HttpGet]
        [Route("id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetSubjectById(string id)
        {

            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _subjectsService.GetByIdAsync(id);

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });


        }

        [HttpGet]
        [Route("count")]
        [Authorize]
        public async Task<IActionResult> GetCountAsync()
        {
            var SubjectCounts = await _subjectsService.GetAllAsync();
            var count = SubjectCounts.Count();
            return Ok(new BaseResponse<int> { Data = count });
        }
        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {

            var SubjectCounts = await _subjectsService.GetAllAsync();

            List<SubjectDTO> subjectList = new List<SubjectDTO>();

            foreach (var subject in SubjectCounts)
            {
                var item = new SubjectDTO
                {
                    Id = subject.Id,
                    Name = subject.Name,
                    CoursePictureUrl = subject.CoursePictureUrl
                };

                subjectList.Add(item);

            }

            return Ok(new BaseResponse<List<SubjectDTO>> { Data = subjectList });


        }


        [HttpPost]
        [Route("create")]
        [Authorize]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectSchema subject)
        {

            //attrib validations
            if (string.IsNullOrEmpty(subject.Name)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "name: " + ResponseMessages.BAD_REQUEST });

            //name existence
            var courseNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name!.ToLower());

            if (courseNameCheck.Count() < 0) return Ok(new DataResponse { Data = null, ErrorMessage = "subject name: " + ResponseMessages.ENTITY_EXISTS });

            //check picture
            string? url = null;

            if (!string.IsNullOrEmpty(subject.imageModel!.Base64Image) && !string.IsNullOrEmpty(subject.imageModel!.FileFormat))
            {

                try
                {
                    byte[] imageBytes;
                    try
                    {
                        imageBytes = Convert.FromBase64String(subject.imageModel.Base64Image);
                    }
                    catch (FormatException)
                    {
                        return BadRequest(new BaseResponse<bool> { Data = false });
                    }

                    string baseDirectory = _hostingEnvironment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");

                    string folderPath = Path.Combine(baseDirectory, "subject");
                    string concretePath = Path.Combine(folderPath, "picture");
                    string guidName = Guid.NewGuid().ToString();

                    if (!Directory.Exists(concretePath))
                    {
                        Directory.CreateDirectory(concretePath);
                    }

                    string filePath = Path.Combine(concretePath, $"{guidName}.{subject.imageModel.FileFormat}");

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                    {
                        await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                        await fileStream.FlushAsync();
                    }

                    url = $"/subject/picture/{guidName}.{subject.imageModel.FileFormat}";
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }

            }

            var newSubject = new Subject
            {
                Id = Guid.NewGuid().ToString(),
                Name = subject.Name,
                CoursePictureUrl = subject.CoursePictureUrl,
            };

            if (url != null)
            {

                //create new Subject object
                newSubject.CoursePictureUrl = url;


            }
            
            //add to EF database
            var registSubject = _subjectsService.AddAsync(newSubject);

            if (registSubject == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseError });


            

            return Ok(new BaseResponse<bool> { Data = true });
        }

        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectSchema subject)
        {

            //attrib val
            if (!Guid.TryParse(subject.Id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "Id: " + ResponseMessages.BAD_REQUEST });
            if (!string.IsNullOrEmpty(subject.Name)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "name: " + ResponseMessages.BAD_REQUEST });
            if (!string.IsNullOrEmpty(subject.CoursePictureUrl)) return BadRequest(new DataResponse { Data = null, ErrorMessage = "urlpicture: " + ResponseMessages.BAD_REQUEST });

            //verify name existene

            var subjectNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name!.ToLower());

            if (subjectNameCheck == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = "subject name: " + ResponseMessages.ENTITY_EXISTS });

            var ModifySubject = await _subjectsService.GetByIdAsync(subject.Id);

            //update object
            if (ModifySubject == null) return NotFound();

            ModifySubject.Name = subject.Name;

            var result = await _subjectsService.UpdateAsync(ModifySubject);

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = "subject:" + ResponseMessages.INTERNAL_ERROR });

            return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null });

        }

        [HttpPut]
        [Route("update/picture/subject/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePictureSubject([FromBody] ImageProfile imageData, string id)
        {
            if (imageData == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrWhiteSpace(imageData.Base64Image)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrWhiteSpace(imageData.FileFormat)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (id == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            var subject = await _subjectsService.GetByIdAsync(id);

            if (subject == null) return NotFound();

            if (subject.CoursePictureUrl != null || !string.IsNullOrEmpty(subject.CoursePictureUrl))
            {

                var currentFileName = Path.GetFileName(subject.CoursePictureUrl);
                var deletePath = Directory.GetCurrentDirectory();
                var deleteWwwroot = Path.Combine(deletePath, "wwwroot");
                var deleteSubjectFolder = Path.Combine(deleteWwwroot, "subject");
                var deletePictureFolder = Path.Combine(deleteSubjectFolder, "picture");
                var deleteConcretePath = Path.Combine(deletePictureFolder, currentFileName);

                if (System.IO.File.Exists(deleteConcretePath))
                {
                    System.IO.File.Delete(deleteConcretePath);
                }

            }

            try
            {
                byte[] imageBytes;
                try
                {
                    imageBytes = Convert.FromBase64String(imageData.Base64Image);
                }
                catch (FormatException)
                {
                    return BadRequest(new BaseResponse<bool> { Data = false });
                }

                string baseDirectory = _hostingEnvironment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");

                string folderPath = Path.Combine(baseDirectory, "subject");
                string concretePath = Path.Combine(folderPath, "picture");
                string guidName = Guid.NewGuid().ToString();

                if (!Directory.Exists(concretePath))
                {
                    Directory.CreateDirectory(concretePath);
                }

                string filePath = Path.Combine(concretePath, $"{guidName}.{imageData.FileFormat}");

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    await fileStream.FlushAsync();
                }

                var url = $"/subject/picture/{guidName}.{imageData.FileFormat}";
                await _context.Entry(subject).ReloadAsync();

                var entry = _context.Entry(subject);
                entry.Entity.CoursePictureUrl = url;
                _context.Entry(subject).Property(e => e.CoursePictureUrl).IsModified = true;

                await _context.SaveChangesAsync();


                return Ok(new BaseResponse<bool> { Data = true });
            }
            catch
            {

                return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ConvertImageError });
            }
        }



    }
}
