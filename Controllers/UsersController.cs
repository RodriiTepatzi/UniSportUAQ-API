using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
    [Route("api/users")]
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
            (i.PhoneNumber != null && i.PhoneNumber == student.PhoneNumber.ToLower()) ) &&
            (i.IsStudent == true)
            ));

            if (result.Any()) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

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

            if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var emailValidator = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            if (!emailValidator.IsMatch(email)) return BadRequest("No contiene un email valido.");

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsStudent == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var student = result.FirstOrDefault();

            return Ok(new DataResponse { Data = student!.ToDictionary, ErrorMessage = null });
        }

        //get by id

        [HttpGet]
        [Route("students/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetStudentById(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetByIdAsync(id);

            if (result!.IsStudent == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        //get user by exp

        [HttpGet]
        [Route("students/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetStudentByExp(string exp)
        {

            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var student = result.FirstOrDefault();

            return Ok(new DataResponse { Data = student!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("students/exp/{exp}/email")]
        public async Task<IActionResult> GetStudentByExpAndReturnEmailOnly(string exp)
        {
            var students = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (students is null || !students.Any()) return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var student = students.FirstOrDefault();

            return Ok(new DataResponse { Data = student!.Email, ErrorMessage = null });
        }

        [HttpGet]
        [Route("students/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsByRange(int start, int end)
        {

            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.IsStudent == true);

            if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var StudentsInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<String, object>>();

            foreach (var item in StudentsInRange) data.Add(item.ToDictionary);

            return Ok(new DataResponse { Data = data, ErrorMessage = null });
        }

        [HttpGet]
        [Route("students/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsSeacrhAsync(string searchTerm)
        {

            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _usersService.GetAllAsync(i => i.IsStudent == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var students = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in students) data.Add(item.ToDictionary);

            if (students.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("students/search/course/{courseId}/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsSearchAsync(string courseId, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var resultCourse = await _coursesService.GetByIdAsync(courseId);

            if (resultCourse == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var studentsInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId && i.Student != null, i => i.Student!);

            var searchLower = searchTerm.ToLower();
            var result = studentsInCourse.Where(i =>
                (i.Student!.Email != null && i.Student.Email.ToLower().Contains(searchLower)) ||
                (i.Student.FullName != null && i.Student.FullName.ToLower().Contains(searchLower)) ||
                (i.Student.Expediente != null && i.Student.Expediente.ToLower().Contains(searchLower)) ||
                (i.Student.UserName != null && i.Student.UserName.ToLower().Contains(searchLower))
            );

            var data = new List<Dictionary<string, object>>();
            foreach (var item in result)
            {
                if (item.Student != null)
                {
                    data.Add(item.Student.ToDictionary);
                }
            }

            if (data.Count > 0)
                return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }


        //********************************************INSTRUCTORS********************************************//
        [HttpGet]
        [Route("instructors/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorById(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetByIdAsync(id);

            if (result!.IsInstructor == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("instructors/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorByExp(string exp)
        {
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsInstructor == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var instructor = result.FirstOrDefault();

            return Ok(new DataResponse { Data = instructor!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("instructors/email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsStudent == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var instructor = result.FirstOrDefault();

            return Ok(new DataResponse { Data = instructor!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("instructors/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorsByRange(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.IsInstructor == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var InstrInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in InstrInRange) data.Add(item.ToDictionary);

            return Ok(new DataResponse { Data = data, ErrorMessage = null });
        }

        [HttpGet]
        [Route("instructors/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetInstructorSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _usersService.GetAllAsync(i => i.IsInstructor == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var instructor = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in instructor) data.Add(item.ToDictionary);

            if (instructor.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpPut]
        [Route("instructors/promote/id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateInstructor(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var NewInstructor = await _usersService.GetByIdAsync(id);

            if (NewInstructor!.IsInstructor == true) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ERROR_PROMOTING });

            if (NewInstructor is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            NewInstructor.IsInstructor = true;

            var registerInstructor = _usersService.UpdateAsync(NewInstructor);

            if (registerInstructor is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ERROR_PROMOTING });

            return Ok(new DataResponse { Data = NewInstructor.ToDictionary, ErrorMessage = null });


        }
        //**********************************ADMINS**********************************//

        [HttpGet]
        [Route("admins/id/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAdminById(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetByIdAsync(id);

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result!.IsAdmin == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });


        }

        [HttpGet]
        [Route("admins/email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.Email == email && i.IsAdmin == true);

            if (result.Count() < 1) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var admin = result.FirstOrDefault();

            return Ok(new DataResponse { Data = admin!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("admins/exp/{exp}")]
        [Authorize]
        public async Task<IActionResult> GetAdminByExp(string exp)
        {
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.Expediente == exp && i.IsAdmin == true);

            if (result.Count() < 1) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var admin = result.FirstOrDefault();

            return Ok(new DataResponse { Data = admin!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("admins/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAdminsByRange(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync(i => i.IsAdmin == true);

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var adminsInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in adminsInRange) data.Add(item.ToDictionary);

            return Ok(new DataResponse { Data = data, ErrorMessage = null });
        }

        [HttpGet]
        [Route("admins/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetAdminSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _usersService.GetAllAsync(i => i.IsAdmin == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var admins = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in admins) data.Add(item.ToDictionary);

            if (admins.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
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

			if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null});
        }

		[HttpGet]
		[Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
            var result = await _usersService.GetAllAsync(i => i.Email == email);

			if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var user = result.FirstOrDefault();

			return Ok(new DataResponse { Data = user.ToDictionary, ErrorMessage = null});
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetAllUsersInRange(int start, int end)
		{


            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var usersInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
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

			var userUpdt = new ApplicationUser { 

				Id = oldUser.Id,
				Name = user.Name,
				LastName = user.LastName,
				PhoneNumber = user.PhoneNumber,
				Email = user.Email,
				IsInFIF = user.IsInFIF,
				Semester = user.Semester,
				IsActive = user.IsActive,
				IsAdmin = user.IsAdmin,
				IsStudent = user.IsStudent,
				IsInstructor = user.IsInstructor,
				PictureUrl = user.PictureUrl,
				
			};

			var result = await _usersService.UpdateAsync(userUpdt);

			if(result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

            return Ok(new DataResponse { Data = userUpdt.ToDictionary, ErrorMessage = null});
		}

    }
}

