using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

		[HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var emailValidator = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

			if (!emailValidator.IsMatch(email)) return BadRequest("No contiene un email valido.");

			var result = await _studentsService.GetStudentByEmailAsync(email);

			if (result.Count > 0) return Ok(result[0].ToDictionaryForEmailRequest());

			return Ok(result);
		}

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetUserById(string Id) {
			//validation
			if (Guid.TryParse(Id, out _))
			{
				//asign
				var result = await _studentsService.GetStudentByIdAsync(Id);
				if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRequest());
				//return in case result>0
				return Ok(result);

			}
			else {
				return BadRequest("It is not a valid Id");
			}
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetStudentsByRange(int start, int end)
		{
			var result = await _studentsService.GetAllInRangeAsync(start, end);
			if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRequest());
			//return in case result>0
			return Ok(result);
		}

		[HttpPost]
		[Route("create")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateStudent([FromBody] StudentSchema student)
		{
			// TODO: more work to do here... such as validations and stuff.

			if (!string.IsNullOrEmpty(student.Email))
			{
				var emailEntity = await _studentsService.GetStudentByEmailAsync(student.Email);

				if (emailEntity.Count > 0) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS});
			}

			if (!string.IsNullOrEmpty(student.Id))
			{
				var idEntity = await _studentsService.GetStudentByIdAsync(student.Id);

				if (idEntity.Count > 0) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			var result = await _studentsService.CreateStudentAsync(student);

			return Ok(new DataResponse { Data = result, ErrorMessage = null});
		}
	}
}
