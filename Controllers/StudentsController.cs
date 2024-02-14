using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using UniSportUAQ_API.Data.Services;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Consts;

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
				if (result.Count > 0) return Ok(new DataResponse{ Data = result[0].ToDictionaryForIdRequest(), ErrorMessage = null });
				//return in case result>0
				return Ok(new DataResponse { Data = result, ErrorMessage = null});

			}
			else {

				return Ok(new DataResponse { Data =  null, ErrorMessage= ResponseMessages.OBJECT_NOT_FOUND});

			}

			
			
		}
	}
}
