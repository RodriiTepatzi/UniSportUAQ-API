using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
	[ApiController]
	[Route("users/students")]
	public class StudentsController : Controller
	{
		private readonly IStudentsService _studentsService;
        public StudentsController(IStudentsService studentsService)
        {
			_studentsService = studentsService;   
        }

		[HttpGet]
        [Route("email/{email}")]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var emailValidator = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

			if (!emailValidator.IsMatch(email)) return BadRequest("No contiene un email valido.");

			var result = await _studentsService.GetStudentByEmailAsync(email);

			if (result.Count > 0) return Ok(result[0].ToDictionaryForEmailRequest());

			return Ok(result);
		}
		

		//email
		


		[HttpGet]
		[Route("id/{id}")]

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
	}
}
