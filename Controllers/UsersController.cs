using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllAsync();

            return Ok(result);
        }
    }
}

