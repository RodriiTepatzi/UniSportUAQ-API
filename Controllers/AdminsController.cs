using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Interfaces;

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
		[Authorize]
        public async Task<IActionResult> GetAdminById(string id)
        {
            var result = await _adminsService.GetAdminByIdAsync(id);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary, ErrorMessage = null});

            return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var result = await _adminsService.GetAdminByEmailAsync(email);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpGet]
        [Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetAdminByExp(string exp)
        {
            var result = await _adminsService.GetAdminByExpAsync(exp);

            if (result.Count > 0) return Ok(new DataResponse { Data = result[0].ToDictionary, ErrorMessage = null });

            return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorsByRange(int start, int end)
		{
			var result = await _adminsService.GetAllInRangeAsync(start, end);

			if (result.Count == 0) return Ok(new DataResponse { Data = result, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

			var data = new List<Dictionary<string, object>>();

			foreach (var item in result) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
		}

        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetAdminSeacrhAsync(string searchTerm)
        {
            if (searchTerm is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.GetAdminSeacrhAsync(searchTerm);

            var data = new List<Dictionary<string, object>>();

            foreach (var item in result) data.Add(item.ToDictionary);

            if (result.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateAdmin([FromBody] AdminSchema admin) {

            //validate register attrributes

            if (!string.IsNullOrEmpty(admin.Email)) { 

                var emailEntity = await _adminsService.GetAdminByEmailAsync(admin.Email);

                if (emailEntity.Count() > 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
            }

            if (!string.IsNullOrEmpty(admin.Expediente)) {

                var expEntity = await _adminsService.GetAdminByExpAsync(admin.Expediente);

                if (expEntity.Count() > 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
            }

            if (!string.IsNullOrEmpty(admin.Id)) { 
                var idEntity = await _adminsService.GetAdminByIdAsync(admin.Id);

                if (idEntity.Count() > 0) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });
            }

            if (!string.IsNullOrEmpty(admin.Password)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.CreateAdminAsync(admin);

            return Ok(new DataResponse { Data = result, ErrorMessage = null });

        }
	}
}

