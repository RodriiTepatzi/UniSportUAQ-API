using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
	[Route("api/users/instructors")]
	public class InstructorsController : Controller
	{
		private readonly IInstructorsService _instructorsService;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IStudentsService _studentsService;


        public InstructorsController(IInstructorsService instructorsService, UserManager<ApplicationUser> userManager, IStudentsService studentsService)
        {
			_instructorsService = instructorsService;
            _userManager = userManager;
            _studentsService = studentsService;
        }

		[HttpGet]
		[Route("id/{id}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorById(string id)
		{
			if(!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _instructorsService.GetByIdAsync(id);

			if(result!.IsInstructor == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result is not null) return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null });

			return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});
		}

		[HttpGet]
		[Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByExp(string exp)
		{
            if (!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _instructorsService.GetAllAsync(i => i.Expediente == exp && i.IsInstructor == true);

            if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var instructor = result.FirstOrDefault();

            return Ok(new DataResponse { Data = instructor!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _instructorsService.GetAllAsync(i => i.Email == email && i.IsStudent == true);

			if (result is  null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var instructor = result.FirstOrDefault();

            return Ok(new DataResponse { Data = instructor!.ToDictionary, ErrorMessage = null });
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorsByRange(int start, int end)
		{
            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var result = await _instructorsService.GetAllAsync(i => i.IsInstructor == true);

			if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var InstrInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in InstrInRange) data.Add(item.ToDictionary);

            return Ok(new DataResponse { Data = data, ErrorMessage = null });
        }

		[HttpGet]
		[Route("search/{searchTerm}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _instructorsService.GetAllAsync(i => i.IsInstructor == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var instructor = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in instructor) data.Add(item.ToDictionary);

            if (instructor.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

        }

        [HttpPut]
		[Route("promote/id/{id}")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateInstructor(string id)
		{
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var NewInstructor = await _studentsService.GetByIdAsync(id);

            if (NewInstructor!.IsInstructor == true) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ERROR_PROMOTING });

            if (NewInstructor is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            NewInstructor.IsInstructor = true;

            var registerInstructor = _instructorsService.UpdateAsync(NewInstructor);

            if (registerInstructor is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ERROR_PROMOTING });

            return Ok(new DataResponse { Data = NewInstructor.ToDictionary, ErrorMessage = null });


        }
    }
}
