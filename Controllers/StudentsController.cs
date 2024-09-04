using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;


namespace UniSportUAQ_API.Controllers
{
    [ApiController]
	[Route("api/users/students")]
	public class StudentsController : Controller
	{
		private readonly IStudentsService _studentsService;
		private readonly ICoursesService _coursesService;
		private readonly IInscriptionsService _inscriptionsService;
        private readonly UserManager<ApplicationUser> _userManager;


        public StudentsController(IStudentsService studentsService, ICoursesService coursesService, IInscriptionsService inscriptionsService, UserManager<ApplicationUser> userManager)
        {
            _studentsService = studentsService;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
            _userManager = userManager;
        }


        [HttpPost]
		[Route("create")]
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
            var result = await _studentsService.GetAllAsync(i =>
            ((i.Expediente != null && i.Expediente.ToLower() == student.Expediente.ToLower()) ||
            (i.Email != null && i.Email.ToLower() == student.Email.ToLower()) ||
            (i.PhoneNumber != null && i.PhoneNumber == student.PhoneNumber.ToLower())
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

            var studentAdded = await _studentsService.AddAsync(newStudent);

            if (studentAdded is not null) return Ok(new DataResponse { Data = studentAdded.ToDictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });
        }

        //get by email
        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var emailAttribute = new EmailAddressAttribute();

			if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST } );

			var emailValidator = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

			if (!emailValidator.IsMatch(email)) return BadRequest("No contiene un email valido.");

			var result = await _studentsService.GetAllAsync(i => i.Email == email && i.IsStudent == true);

            if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var student = result.FirstOrDefault();

			return Ok(new DataResponse { Data = student!.ToDictionary, ErrorMessage = null });
		}

		//get by id

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetUserById(string id) 
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _studentsService.GetByIdAsync(id);

            if (result!.IsStudent == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

		//get user by exp

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetUserByExp(string exp) {

            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _studentsService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var student = result.FirstOrDefault();

            return Ok(new DataResponse { Data = student!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("exp/{exp}/email")]
        public async Task<IActionResult> GetUserByExpAndReturnEmailOnly(string exp)
        {
            var students = await _studentsService.GetAllAsync(i => i.Expediente == exp && i.IsStudent == true);

            if (students is null || !students.Any())
                return NotFound(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var student = students.FirstOrDefault();

            return Ok(new DataResponse { Data = student!.Email, ErrorMessage = null });
        }


        [HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetStudentsByRange(int start, int end)
		{

            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _studentsService.GetAllAsync(i => i.IsStudent == true);

			if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var StudentsInRange = result.Skip(start).Take(end - start + 1).ToList();

			var data = new List<Dictionary<String, object>>();

			foreach (var item in StudentsInRange) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
		}

		[HttpGet]
		[Route("search/{searchTerm}")]
		[Authorize]
		public async Task<IActionResult> GetStudentsSeacrhAsync(string searchTerm) 
		{

            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _studentsService.GetAllAsync(i => i.IsStudent == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var students = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in students) data.Add(item.ToDictionary);

            if (students.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpGet]
        [Route("search/course/{courseId}/{searchTerm}")]
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


    }
}
