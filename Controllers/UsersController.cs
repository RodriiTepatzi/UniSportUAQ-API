using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Services;

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

			foreach (var item in result) data.Add(item.ToDictionary());

			return Ok(new DataResponse { Data = data, ErrorMessage = null});
        }

		[HttpGet]
		[Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetUserByEmail(string email)
		{
			var result = await _usersService.GetUserByEmailAsync(email);

			if (result is null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			return Ok(new DataResponse { Data = result.ToDictionary(), ErrorMessage = null});
		}

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetAllUsersInRange(int start, int end)
		{
			var result = await _usersService.GetAllInRangeAsync(start, end);

			if (result.Count == 0) return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary());

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
		}
	}
}

