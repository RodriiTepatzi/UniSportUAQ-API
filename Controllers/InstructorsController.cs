using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
	[ApiController]
	[Route("api/users/instructors")]
	public class InstructorsController : Controller
	{
		private readonly IInstructorsService _instructorsService;

        public InstructorsController(IInstructorsService instructorsService)
        {
			_instructorsService = instructorsService;
        }

        [HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorById(string id)
		{
			var result = await _instructorsService.GetInstructorByIdAsync(id);

			if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
		}

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByExp(string exp)
		{
			var result = await _instructorsService.GetInstructorByExpAsync(exp);

			if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByEmail(string email)
        {
            var result = await _instructorsService.GetInstructorByEmailAsync(email);

			if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorsByRange(int start, int end)
		{
			var result = await _instructorsService.GetAllInRangeAsync(start, end);

			if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpPost]
		[Route("create")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateStudent([FromBody] InstructorSchema instructor)
		{

			if (!string.IsNullOrEmpty(instructor.Email))
			{
				var emailEntity = await _instructorsService.GetInstructorByEmailAsync(instructor.Email);

				if (emailEntity.Count > 0) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (!string.IsNullOrEmpty(instructor.Id))
			{
				var idEntity = await _instructorsService.GetInstructorByIdAsync(instructor.Id);

				if (idEntity.Count > 0) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (!string.IsNullOrEmpty(instructor.Expediente))
			{
				var idEntity = await _instructorsService.GetInstructorByExpAsync(instructor.Expediente);

				if (idEntity.Count > 0) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			}

			if (string.IsNullOrEmpty(instructor.Password)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _instructorsService.CreateInstructorAsync(instructor);

			return Ok(new DataResponse { Data = result, ErrorMessage = null });
		}
	}
}
