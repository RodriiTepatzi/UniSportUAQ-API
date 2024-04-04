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
		private readonly IStudentsService _studentsService;

        public InstructorsController(IInstructorsService instructorsService, IStudentsService studentsService)
        {
			_instructorsService = instructorsService;
			_studentsService = studentsService;
        }

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorById(string id)
		{
			var result = await _instructorsService.GetInstructorByIdAsync(id);

			if (result is not null) return Ok(new DataResponse { Data = result.InstructorToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
		}

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByExp(string exp)
		{
			var result = await _instructorsService.GetInstructorByExpAsync(exp);

			if (result is not null) return Ok(new DataResponse { Data = result.InstructorToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByEmail(string email)
        {
            var result = await _instructorsService.GetInstructorByEmailAsync(email);

			if (result is not null) return Ok(new DataResponse { Data = result.InstructorToDictionary(), ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorsByRange(int start, int end)
		{
			var result = await _instructorsService.GetAllInRangeAsync(start, end);

			var dictionaries = new List<Dictionary<string, object>>();

			if (result.Count > 0)
			{
                foreach (var item in result) dictionaries.Add(item.InstructorToDictionary());

				return Ok(new DataResponse { Data = dictionaries, ErrorMessage = null });
			}

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
		}

		[HttpGet]
		[Route("search/{searchTerm}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorSeacrhAsync(string searchTerm)
        {
            if (searchTerm is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _instructorsService.GetInstructorSeacrhAsync(searchTerm);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.ToDictionary());

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpPost]
		[Route("promote")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateInstructorasync([FromBody] InstructorSchema instructor)
		{

			if (instructor.Id is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var studentResult = await _studentsService.GetStudentByIdAsync(instructor.Id);

			if(studentResult is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ERROR_PROMOTING });



			var idEntity = await _instructorsService.GetInstructorByIdAsync(instructor.Id);

			if (idEntity is not null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
			

			var result = await _instructorsService.CreateInstructorAsync(instructor);

			return Ok(new DataResponse { Data = result.InstructorToDictionary(), ErrorMessage = null });
		}

		[HttpPut]
		[Route("update")]
		[Authorize]
		public async Task<IActionResult> UpdateInstructorAsync([FromBody] InstructorSchema instructorSchema) {

			if (instructorSchema.Id is null) return BadRequest(new DataResponse{Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (instructorSchema.Name is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (instructorSchema.LastName is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (instructorSchema.Expediente is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (instructorSchema.PhoneNumber is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (instructorSchema.Email is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var instructorUpdt = new ApplicationUser
			{
				Id = instructorSchema.Id,
				Name = instructorSchema.Name,
				Email = instructorSchema.Email,
				PhoneNumber = instructorSchema.PhoneNumber,
				Expediente = instructorSchema.Expediente,
				LastName = instructorSchema.LastName

			};

			var result = await _instructorsService.UpdateInstructorAsync(instructorUpdt);

			if(result is not null) return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null});

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
        }
	}
}
