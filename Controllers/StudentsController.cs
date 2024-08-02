using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text.RegularExpressions;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;


namespace UniSportUAQ_API.Controllers
{
    [ApiController]
	[Route("api/users/students")]
	public class StudentsController : Controller
	{
		private readonly IStudentsService _studentsService;
		private readonly ICoursesService _coursesService;
		private readonly IInscriptionsService _inscriptionsService;
        public StudentsController(IStudentsService studentsService, ICoursesService coursesService, IInscriptionsService inscriptionsService)
        {
            _studentsService = studentsService;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
        }


        [HttpPost]
		[Route("create")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateStudent([FromBody] StudentSchema student)
		{

			if (!string.IsNullOrEmpty(student.Email))
			{
				var emailEntity = await _studentsService.GetStudentByEmailAsync(student.Email);

				if (emailEntity is not null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (!string.IsNullOrEmpty(student.Id))
			{
				var idEntity = await _studentsService.GetStudentByIdAsync(student.Id);

				if (idEntity is not null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (!string.IsNullOrEmpty(student.Expediente))
			{
				var expEntity = await _studentsService.GetStudentByExpAsync(student.Expediente);

				if (expEntity is not null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (string.IsNullOrEmpty(student.Password)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _studentsService.CreateStudentAsync(student);

			return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });
		}

		//get by email
		[HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var emailValidator = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

			if (!emailValidator.IsMatch(email)) return BadRequest("No contiene un email valido.");

			var result = await _studentsService.GetStudentByEmailAsync(email);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });
		}

		//get by id

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetUserById(string Id) 
        {
			var result = await _studentsService.GetStudentByIdAsync(Id);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
               
			return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null});
			
		}

		//get user by exp

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetUserByExp(string exp) {

			var result = await _studentsService.GetStudentByExpAsync(exp);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null }); ;
		}

        [HttpGet]
        [Route("exp/{exp}/email")]
        public async Task<IActionResult> GetUserByExpAndReturnEmailOnly(string exp)
        {

            var result = await _studentsService.GetStudentByExpAsync(exp);


            if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = result.Email, ErrorMessage = null }); ;
        }

        [HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetStudentsByRange(int start, int end)
		{
			var result = await _studentsService.GetAllInRangeAsync(start, end);

			var dictionaryResults = new List<Dictionary<string, object>>();

			result.ForEach((value) => dictionaryResults.Add(value.ToDictionary));

			if (result.Count > 0) return Ok(new DataResponse { Data = dictionaryResults, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND }); ;
		}

		[HttpGet]
		[Route("search/{searchTerm}")]
		[Authorize]
		public async Task<IActionResult> GetStudentsSeacrhAsync(string searchTerm) {

			if(searchTerm is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST});

			var result = await _studentsService.GetStudentsSeacrhAsync(searchTerm);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.ToDictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

		}

        [HttpGet]
        [Route("search/course/{courseId}/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetStudentsSeacrhAsync(string searchTerm, string courseId)
        {

            if (searchTerm is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			
			var resultCourse = await _coursesService.GetByIdAsync(courseId);

			if (resultCourse == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

			var studentsInCourse = await _inscriptionsService.GetAllAsync(i => i.CourseId == courseId, i => i.Student!);

            var result = studentsInCourse.Where(i => i.Student.Email.ToUpper().Contains(searchTerm.ToUpper()) ||
			i.Student.FullName.ToUpper().Contains(searchTerm.ToUpper()) ||
			i.Student.Expediente.ToUpper().Contains(searchTerm.ToUpper()) ||
            i.Student.UserName.ToUpper().Contains(searchTerm.ToUpper()));

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.Student.ToDictionary);

            if (result.Any()) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

    }
}
