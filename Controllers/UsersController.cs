using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;

namespace UniSportUAQ_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

		[HttpGet]
		[Route("all")]
		[Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllAsync();

			if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null});
        }

		[HttpGet]
		[Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var result = await _usersService.GetUserByEmailAsync(email);

			if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null});
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetAllUsersInRange(int start, int end)
		{
			var result = await _usersService.GetAllInRangeAsync(start, end);

			if (result.Count == 0) return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
		}

		[HttpPut]
		[Route("update")]
		[Authorize]

		public async Task<IActionResult> UpdateUserAsync([FromBody]UserSchema user) {

			if (user.Id == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.PictureUrl == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

			var userUpdt = new ApplicationUser { 

				Id = user.Id,
				Name = user.Name,
				LastName = user.LastName,
				PhoneNumber = user.PhoneNumber,
				Email = user.Email,
				IsInFIF = user.IsInFIF,
				Semester = user.Semester,
				IsActive = user.IsActive,
				IsAdmin = user.IsAdmin,
				IsStudent = user.IsStudent,
				IsInstructor = user.IsInstructor,
				PictureUrl = user.PictureUrl,
				
			};

			var result = await _usersService.UpdateUserAsync(userUpdt);

			if(result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

            return Ok(new DataResponse { Data = userUpdt.ToDictionary, ErrorMessage = null});
		}

    }
}

