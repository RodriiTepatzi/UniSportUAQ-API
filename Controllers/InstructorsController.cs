using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
	[ApiController]
	[Route("users/instructors")]
	public class InstructorsController : Controller
	{
		private readonly IInstructorsService _instructorsService;

        public InstructorsController(IInstructorsService instructorsService)
        {
			_instructorsService = instructorsService;
        }

        [HttpGet]
		[Route("id/{id}")]
		public async Task<IActionResult> GetInstructorById(string id)
		{
			var result = await _instructorsService.GetInstructorByIdAsync(id);

			if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRetrieve());

			return Ok(result);
		}
	}
}
