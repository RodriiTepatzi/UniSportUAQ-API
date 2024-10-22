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
using Microsoft.AspNetCore.Identity;
using System;

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

            if (!Guid.TryParse(id, out _)) return BadRequest(new BaseResponse<SubjectDTO> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            var result = await _subjectsService.GetByIdAsync(id);

            if (result == null) return Ok(new BaseResponse<SubjectDTO> { Error = ResponseErrors.DataNotFound });
            if (result.IsActive == false) return Ok(new BaseResponse<SubjectDTO> { Error = ResponseErrors.DataNotFound });

            var subjectDTO = new SubjectDTO
            {
                Id = result.Id,
                Name = result.Name,
                CoursePictureUrl = result.CoursePictureUrl
            };

            return Ok(new BaseResponse<SubjectDTO> { Data = subjectDTO });

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

            var SubjectCounts = await _subjectsService.GetAllAsync(i => i.IsActive == true);

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
            if (string.IsNullOrEmpty(subject.Name)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeNameEmpty });

            //name existence
            var courseNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name.ToLower());

            if (courseNameCheck.Count() < 0) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityExist });

            //check picture
            string? url = null;

            if (subject.imageModel != null)
            {

                if (!string.IsNullOrEmpty(subject.imageModel.Base64Image) && !string.IsNullOrEmpty(subject.imageModel.FileFormat))
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
                            return Ok(new BaseResponse<bool> { Data = false });
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                    }

                }

            }

            //create new Subject object
            var newSubject = new Subject
            {
                Id = Guid.NewGuid().ToString(),
                Name = subject.Name,
                CoursePictureUrl = url,
                IsActive = true,
            };

            //add to EF database
            var registSubject = _subjectsService.AddAsync(newSubject);

            if (registSubject == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

            if (url == null) Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ConvertImageError });

            return Ok(new BaseResponse<object> { Data = true });
        }

        [HttpPut]
        [Route("update")]
        [Authorize]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectSchema subject)
        {

            //attrib val
            if (!Guid.TryParse(subject.Id, out _)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });
            if (string.IsNullOrEmpty(subject.Name)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeNameEmpty });

            //verify name existene

            var subjectNameCheck = await _subjectsService.GetAllAsync(i => i.Name!.ToLower() == subject.Name!.ToLower());

            if (subjectNameCheck == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            var ModifySubject = await _subjectsService.GetByIdAsync(subject.Id);

            //update object
            if (ModifySubject == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

            ModifySubject.Name = subject.Name;

            var result = await _subjectsService.UpdateAsync(ModifySubject);

            if (result == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseErrorUpdating });

            return Ok(new BaseResponse<bool> { Data = true });

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

            if (subject == null) return Ok(new BaseResponse<bool> { Error = ResponseErrors.EntityNotExist });

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

        [HttpPut]
        [Route("remove/picture/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSubjectPictureAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });


            var subject = await _subjectsService.GetByIdAsync(id);
            if (subject == null) return NotFound();

            string? currentUserPicture = subject.CoursePictureUrl;


            if (currentUserPicture != null && !string.IsNullOrEmpty(currentUserPicture))
            {
                var currentFileName = Path.GetFileName(currentUserPicture);
                var deletePath = Directory.GetCurrentDirectory();
                var deleteWwwroot = Path.Combine(deletePath, "wwwroot");
                var deleteUsersFolder = Path.Combine(deleteWwwroot, "subject");
                var deleteProfileFolder = Path.Combine(deleteUsersFolder, "picture");
                var deleteConcretePath = Path.Combine(deleteProfileFolder, currentFileName);

                bool deleted = await DeleteFileAsync(deleteConcretePath);

                if (deleted == true)
                {
                    subject.CoursePictureUrl = null;

                    var updated = await _subjectsService.UpdateAsync(subject);

                    if (updated != null) return Ok(new BaseResponse<bool> { Data = true });

                    return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });


                }

                if (deleted == false) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.DeleteFileError });
            }


            return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist }); ;



        }

        [HttpPut]
        [Route("deactivate/{id}")]
        [Authorize]

        public async Task<IActionResult> DeactivateSubject(string id)
        {

            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeIdInvalidlFormat });

            var subject = await _subjectsService.GetByIdAsync(id, i => i.Courses!);

            if (subject == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            subject.IsActive = false;

            

            if (subject.Courses != null)
            {

                if (subject.Courses.Any(c => c.IsActive && c.CurrentUsers > 0)) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.SubjectCoursesNotEnded});

                var idList = subject.Courses.Select(x => x.Id).ToList();

                try
                {

                    _context.Courses
                        .Where(f => idList.Contains(f.Id))
                        .ExecuteUpdate(f => f.SetProperty(x => x.IsActive, x => false));

                }
                catch
                {
                    return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });
                }

            }

            var subjectUpdated = await _subjectsService.UpdateAsync(subject);

            if (subjectUpdated == null) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ServerDataBaseErrorUpdating });


            return Ok(new BaseResponse<bool> { Data = true });
        }

        private async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                });
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }



    }
}
