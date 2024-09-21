using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.DTO;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniSportUAQ_API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
        public UsersController(IUsersService usersService, UserManager<ApplicationUser> userManager, ICoursesService coursesService, IInscriptionsService inscriptionsService)
        {
            _usersService = usersService;
            _userManager = userManager;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
        }

        //********************************************STUDENTS********************************************//
        [HttpPost]
        [Route("students/create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateStudent([FromBody] StudentSchema student)
        {

            //validate register attrributes

            if (student.Expediente == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.Password == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.Semester < 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (student.Group < 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            //check existence 
            var result = await _usersService.GetAllAsync(i =>
            (((i.Expediente != null && i.Expediente.ToLower() == student.Expediente.ToLower()) ||
            (i.Email != null && i.Email.ToLower() == student.Email.ToLower()) ||
            (i.PhoneNumber != null && i.PhoneNumber == student.PhoneNumber.ToLower())) &&
            (i.IsStudent == true)
            ));

            if (result.Any()) return BadRequest(new BaseResponse<ApplicationUser> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat });

            var newStudent = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = student.Name,
                LastName = student.LastName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Expediente = student.Expediente,
                Semester = student.Semester,
                Group = student.Group,
                IsStudent = true,
                IsActive = true,
                UserName = student.Expediente,
            };

            await _userManager.CreateAsync(newStudent, student.Password!);

            var studentAdded = await _usersService.AddAsync(newStudent);

            if (studentAdded is not null) return Ok(new DataResponse { Data = studentAdded.ToDictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });
        }

        //get by email
        [HttpGet]
        [Route("students/email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetStudentByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if(!emailAttribute.IsValid(email)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat});

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsStudent == true);

            var student = result.FirstOrDefault();

            if (student != null) { 
            
                var response = new UserDTO {

                    Id = student.Id,
                    Expediente = student.Expediente,
                    PictureUrl = student.PictureUrl,
                    Name = student.Name,
                    LastName = student.LastName,
                    IsAdmin = student.IsAdmin,
                    IsInstructor = student.IsInstructor,
                    IsStudent = student.IsStudent,

                };

                return Ok(new BaseResponse<UserDTO> { Data = response });
            }

            return NotFound(new BaseResponse<UserDTO> { Data = null });

        }

        //get by id

        [HttpGet]
        [Route("students/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStudentById(string id)
        {
            if(string.IsNullOrEmpty(id)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmptyOrNull} );

            var result = await _usersService.GetByIdAsync(id);

            if (result != null) {

                if(result.IsStudent == false) return NotFound(new BaseResponse<UserDTO> { Data = null });

                var response = new UserDTO
                {

                    Id = result.Id,
                    Expediente = result.Expediente,
                    PictureUrl = result.PictureUrl,
                    Name = result.Name,
                    LastName = result.LastName,
                    IsAdmin = result.IsAdmin,
                    IsInstructor = result.IsInstructor,
                    IsStudent = result.IsStudent,

                };

                return Ok(new BaseResponse<UserDTO> { Data = response});

            }

            return NotFound(new BaseResponse<UserDTO> { Data = null });


        }

        //get user by exp

        [HttpGet]
        [Route("students/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetStudentByExp(string exp)
        {

            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeExpedienteInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var student = result.FirstOrDefault();

            if (student == null) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var response = new UserDTO
            {

                Id = student.Id,
                Expediente = student.Expediente,
                PictureUrl = student.PictureUrl,
                Name = student.Name,
                LastName = student.LastName,
                IsAdmin = student.IsAdmin,
                IsInstructor = student.IsInstructor,
                IsStudent = student.IsStudent,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response });

        }

        [HttpGet]
        [Route("students/exp/{exp}/email")]
        public async Task<IActionResult> GetStudentByExpAndReturnEmailOnly(string exp)
        {
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeExpedienteInvalidlFormat });

            var students = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (!students.Any()) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var student = students.FirstOrDefault();

            if (student == null) return NotFound(new BaseResponse<UserDTO> { Data = null});

            return Ok(new BaseResponse<string> { Data = student.Email });
        }

        [HttpGet]
        [Route("students/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsByRange(int start, int end)
        {

            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync(i => i.IsStudent == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var StudentsInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<UserDTO>();

            foreach (var item in StudentsInRange) {

                var stdnt = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(stdnt);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpGet]
        [Route("students/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsSeacrhAsync(string searchTerm)
        {

            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm });

            var search = searchTerm.ToLower();
            var result = new List<ApplicationUser>();

            
                var users = await _usersService.GetAllAsync(i => i.IsStudent == true &&
                (i.Expediente!.ToLower().Contains(search) ||
                i.Name!.ToLower().Contains(search) ||
                i.LastName!.ToLower().Contains(search) ||
                i.Email!.ToLower().Contains(search) ||
                i.PhoneNumber!.Contains(search)
                ));
                result = users.ToList();
            

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var students = result.ToList();

            var data = new List<UserDTO>();

            foreach (var item in students)
            {

                var stdnt = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(stdnt);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }

        [HttpGet]
        [Route("students/search/course/{courseId}/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsSearchAsync(string courseId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<UserDTO>> { Error = ResponseErrors.FilterInvalidSearchTerm });
            if (string.IsNullOrWhiteSpace(courseId)) return BadRequest(new BaseResponse<List<UserDTO>> { Error = ResponseErrors.AttributeEmptyOrNull });


            var resultCourse = await _coursesService.GetByIdAsync(courseId);

            if (resultCourse == null) return NotFound(new BaseResponse<List<UserDTO>> { Data = null});

            var studentsInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.Student != null, i => i.Student!);

            if (studentsInCourse.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });


            var searchLower = searchTerm.ToLower();
            var result = new List<Inscription>();

            try {

                var users = studentsInCourse.Where(i =>
                    i.Student!.Email!.ToLower().Contains(searchLower) ||
                    i.Student.FullName!.ToLower().Contains(searchLower) ||
                    i.Student.Expediente!.ToLower().Contains(searchLower) ||
                    i.Student.UserName!.ToLower().Contains(searchLower)
                );

                result = users.ToList();
            }
            catch {

                return StatusCode(500, new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.ServerDataBaseError });

            }

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });


            var data = new List<UserDTO>();

            foreach (var item in result)
            {
                if (item.Student != null) {

                    var stdnt = new UserDTO
                    {

                        Id = item.Student.Id,
                        Expediente = item.Student.Expediente,
                        PictureUrl = item.Student.PictureUrl,
                        Name = item.Student.Name,
                        LastName = item.Student.LastName,
                        IsAdmin = item.Student.IsAdmin,
                        IsInstructor = item.Student.IsInstructor,
                        IsStudent = item.Student.IsStudent,

                    };
                    data.Add(stdnt);
                }

            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }


        //********************************************INSTRUCTORS********************************************//
        [HttpGet]
        [Route("instructors/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorById(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmptyOrNull});

            var result = await _usersService.GetByIdAsync(id);

            if (result == null) return NotFound(new BaseResponse<UserDTO> { Data = null});

            if (result!.IsInstructor == false) return NotFound(new BaseResponse<UserDTO> { Data = null });


            var response = new UserDTO {

                Id = result.Id,
                Expediente = result.Expediente,
                PictureUrl = result.PictureUrl,
                Name = result.Name,
                LastName = result.LastName,
                IsAdmin = result.IsAdmin,
                IsStudent = result.IsStudent,
                IsInstructor = result.IsInstructor,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response });

        }

        [HttpGet]
        [Route("instructors/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorByExp(string exp)
        {
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeExpedienteInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsInstructor == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.EntityNotExist });

            var instructor = result.FirstOrDefault();

            if (instructor == null) return NotFound(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.EntityNotExist });

            var response = new UserDTO {

                Id = instructor.Id,
                Expediente = instructor.Expediente,
                PictureUrl = instructor.PictureUrl,
                Name = instructor.Name,
                LastName = instructor.LastName,
                IsInstructor = instructor.IsInstructor,
                IsStudent = instructor.IsStudent,
                IsAdmin = instructor.IsAdmin,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response });
        }

        [HttpGet]
        [Route("instructors/email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsInstructor == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<ApplicationUser> { Data = null });

            var instructor = result.FirstOrDefault();

            if (instructor == null) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var response = new UserDTO
            {

                Id = instructor.Id,
                Expediente = instructor.Expediente,
                PictureUrl = instructor.PictureUrl,
                Name = instructor.Name,
                LastName = instructor.LastName,
                IsInstructor = instructor.IsInstructor,
                IsStudent = instructor.IsStudent,
                IsAdmin = instructor.IsAdmin,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response });
        }

        [HttpGet]
        [Route("instructors/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorsByRange(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync(i => i.IsInstructor == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var InstrInRange = result.Skip(start).Take(end - start + 1).ToList();

            
            var data = new List<UserDTO>();

            foreach (var item in InstrInRange)
            {

                var instr = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(instr);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpGet]
        [Route("instructors/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm });

            var search = searchTerm.ToLower();

            var result = new List<ApplicationUser>();

            try
            {

                var users = await _usersService.GetAllAsync(i => i.IsInstructor == true &&
                ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
                (i.Name != null && i.Name.ToLower().Contains(search)) ||
                (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
                (i.Email != null && i.Email.ToLower().Contains(search)) ||
                (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
                ));
                result = users.ToList();
            }
            catch
            {

                return StatusCode(500, new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.ServerDataBaseError });


            }

            if (result.Count() < 1) return NotFound(new BaseResponse<ApplicationUser> { Data = null });

            var instructor = result.ToList();


            var data = new List<UserDTO>();

            foreach (var item in instructor)
            {
                var instr = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(instr);

            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }

        [HttpPut]
        [Route("instructors/promote/id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateInstructor(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.AttributeIdInvalidlFormat });

            var NewInstructor = await _usersService.GetByIdAsync(id);

            if (NewInstructor == null) return NotFound(new BaseResponse<bool> { Data = false, Error = ResponseErrors.EntityNotExist });

            if (NewInstructor!.IsInstructor == true) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.SysErrorUserAlredyThisRole });

            NewInstructor.IsInstructor = true;

            var registerInstructor = _usersService.UpdateAsync(NewInstructor);

            if (registerInstructor != null) return Ok(new BaseResponse<bool> { Data = true });

            return BadRequest(new BaseResponse<bool> { Data = false, Error = ResponseErrors.SysErrorPromoting });


        }

        //**********************************ADMINS**********************************//

        [HttpGet]
        [Route("admins/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAdminById(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeIdInvalidlFormat });

            var result = await _usersService.GetByIdAsync(id);

            if (result == null) return NotFound(new BaseResponse<UserDTO> { Data = null });

            if (result!.IsAdmin == false) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var response = new UserDTO
            {

                Id = result.Id,
                Expediente = result.Expediente,
                PictureUrl = result.PictureUrl,
                Name = result.Name,
                LastName = result.LastName,
                IsAdmin = result.IsAdmin,
                IsStudent = result.IsStudent,
                IsInstructor = result.IsInstructor,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response });


        }

        [HttpGet]
        [Route("admins/email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsAdmin == true);

            if (result.Count() < 1) return BadRequest(new BaseResponse<UserDTO> { Data = null });

            var admin = result.FirstOrDefault();

            if (admin == null) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var response = new UserDTO
            {

                Id = admin.Id,
                Expediente = admin.Expediente,
                PictureUrl = admin.PictureUrl,
                Name = admin.Name,
                LastName = admin.LastName,
                IsAdmin = admin.IsAdmin,
                IsStudent = admin.IsStudent,
                IsInstructor = admin.IsInstructor,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response});
        }

        [HttpGet]
        [Route("admins/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetAdminByExp(string exp)
        {
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeExpedienteInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsAdmin == true);

            if (result.Count() < 1) NotFound(new BaseResponse<UserDTO> { Data = null });

            var admin = result.FirstOrDefault();

            if (admin != null) { 

                var response = new UserDTO
                {

                    Id = admin.Id,
                    Expediente = admin.Expediente,
                    PictureUrl = admin.PictureUrl,
                    Name = admin.Name,
                    LastName = admin.LastName,
                    IsAdmin = admin.IsAdmin,
                    IsStudent = admin.IsStudent,
                    IsInstructor = admin.IsInstructor,

                };

                return Ok(new BaseResponse<UserDTO> { Data = response });
            }

            return NotFound(new BaseResponse<UserDTO> { Data = null });
        }

        [HttpGet]
        [Route("admins/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAdminsByRange(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync(i => i.IsAdmin == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var adminsInRange = result.Skip(start).Take(end - start + 1).ToList();

            if (adminsInRange.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var data = new List<UserDTO>();

            foreach (var item in adminsInRange)
            {

                var admns = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(admns);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpGet]
        [Route("admins/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetAdminSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm });

            var search = searchTerm.ToLower();
            var result = new List<ApplicationUser>();

            try
            {
                var users = await _usersService.GetAllAsync(i => i.IsAdmin == true &&
                ((i.Expediente!.ToLower().Contains(search)) ||
                (i.Name!.ToLower().Contains(search)) ||
                (i.LastName!.ToLower().Contains(search)) ||
                (i.Email!.ToLower().Contains(search)) ||
                (i.PhoneNumber!.Contains(search))
                ));
                result = users.ToList();
            }
            catch
            {
                return StatusCode(500, new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.ServerDataBaseError });
            }

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var admins = result.ToList();

            if (admins.Count() < 1) NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var data = new List<UserDTO>();

            foreach (var item in admins)
            {

                var admns = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(admns);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }



        [HttpPost]
        [Route("admins/create")]
        [Authorize]

        public async Task<IActionResult> CreateAdmin([FromBody] AdminSchema admin)
        {

            //validate register attrributes

            if (admin.Expediente == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Password == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });


            //check existence 
            var result = await _usersService.GetAllAsync(i =>
            ((i.Expediente != null && i.Expediente.ToLower() == admin.Expediente.ToLower()) ||
            (i.Email != null && i.Email.ToLower() == admin.Email.ToLower()) ||
            (i.PhoneNumber != null && i.PhoneNumber == admin.PhoneNumber.ToLower())
            ));

            if (result.Any()) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

            var newAdmin = new ApplicationUser
            {

                Id = Guid.NewGuid().ToString(),
                Name = admin.Name,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Expediente = admin.Expediente,
                IsAdmin = true,
                IsActive = true,
            };

            await _userManager.CreateAsync(newAdmin, admin.Password!);

            var adminAdded = await _usersService.AddAsync(newAdmin);

            if (adminAdded is not null) return Ok(new DataResponse { Data = adminAdded.ToDictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });



        }

        //**********************************GENERIC**********************************//
        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return NotFound(new BaseResponse<UserDTO> { Data = null });

            var data = new List<UserDTO>();

            foreach (var item in result)
            {

                var gnrc = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(gnrc);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }

        [HttpGet]
        [Route("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Email == email);

            if (result.Count() < 1) return NotFound(new BaseResponse<UserDTO> { Data = null});

            var user = result.FirstOrDefault();

            if (user == null) return NotFound(new BaseResponse<UserDTO> { Data = null});

            var response = new UserDTO
            {

                Id = user.Id,
                Expediente = user.Expediente,
                PictureUrl = user.PictureUrl,
                Name = user.Name,
                LastName = user.LastName,
                IsAdmin = user.IsAdmin,
                IsStudent = user.IsStudent,
                IsInstructor = user.IsInstructor,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response, Error = null });

        }
        

        [HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetAllUsersInRange(int start, int end)
		{

            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var usersInRange = result.Skip(start).Take(end - start + 1).ToList();

            if (usersInRange.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var data = new List<UserDTO>();

            foreach (var item in usersInRange)
            {

                var gnrc = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(gnrc);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpPut]
		[Route("update")]
		[Authorize]

		public async Task<IActionResult> UpdateUserAsync([FromBody]UserSchema user) {

			if (user.Id == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.PictureUrl == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var oldUser = await _usersService.GetByIdAsync(user.Id);

            if (oldUser == null) return Ok(new DataResponse { Data = null , ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
	
			oldUser.PhoneNumber = user.PhoneNumber;
            oldUser.Email = user.Email;
            oldUser.IsInFIF = user.IsInFIF;
            oldUser.Semester = user.Semester;
            oldUser.IsActive = user.IsActive;
            oldUser.IsAdmin = user.IsAdmin;
            oldUser.IsStudent = user.IsStudent;
            oldUser.IsInstructor = user.IsInstructor;
            oldUser.PictureUrl = user.PictureUrl;
				
			

			var result = await _usersService.UpdateAsync(oldUser);

			if(result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

            return Ok(new DataResponse { Data = oldUser.ToDictionary, ErrorMessage = null});
		}

    }
}

