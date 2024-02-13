using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("api/users/admins")]
    public class AdminsController : Controller
    {
        private readonly IAdminsService _adminsService;

        public AdminsController(IAdminsService adminsService)
        {
            _adminsService = adminsService;
        }

        [HttpGet]
        [Route("id/{id}")]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var result = await _adminsService.GetAdminByIdAsync(id);

            if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRetrieve());

            return Ok(result);
        }

        [HttpGet]
        [Route("email/{email}")]
        public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var result = await _adminsService.GetAdminByEmailAsync(email);

            if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRetrieve());

            return Ok(result);
        }

        [HttpGet]
        [Route("exp/{exp}")]
        public async Task<IActionResult> GetAdminByExp(string exp)
        {
            var result = await _adminsService.GetAdminByExpAsync(exp);

            if (result.Count > 0) return Ok(result[0].ToDictionaryForIdRetrieve());

            return Ok(result);
        }
    }
}

