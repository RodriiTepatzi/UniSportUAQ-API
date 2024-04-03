using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.Security.Provider;
using System.Collections;
using System.Text.RegularExpressions;
using UniSportUAQ_API.Data.Consts;
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
        public StudentsController(IStudentsService studentsService)
        {
			_studentsService = studentsService;   
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

			return Ok(new DataResponse { Data = result.StudentToDictionary(), ErrorMessage = null });
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

			return Ok(new DataResponse { Data = result.StudentToDictionary(), ErrorMessage = null });
		}

		//get by id

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetUserById(string Id) 
        {
			var result = await _studentsService.GetStudentByIdAsync(Id);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
               
			return Ok(new DataResponse { Data = result.StudentToDictionary(), ErrorMessage = null});
			
		}

		//get user by exp

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetUserByExp(string exp) {

			var result = await _studentsService.GetStudentByExpAsync(exp);

			if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			return Ok(new DataResponse { Data = result.StudentToDictionary(), ErrorMessage = null }); ;
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

			result.ForEach((value) => dictionaryResults.Add(value.StudentToDictionary()));

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

            foreach (var item in result) data.Add(item.ToDictionary());

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

		}

	}
}
