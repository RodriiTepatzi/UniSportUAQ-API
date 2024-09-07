using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UniSportUAQ_API.Controllers
{
    [ApiController] 
    [Route("api/users/admins")]
    public class AdminsController : Controller
    {
        private readonly IAdminsService _adminsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminsController(IAdminsService adminsService, UserManager<ApplicationUser> userManager)
        {
            _adminsService = adminsService;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("id/{id}")]
		[Authorize]
        public async Task<IActionResult> GetAdminById(string id)
        {
            if (!Guid.TryParse(id, out _)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.GetByIdAsync(id);

            if(result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            if (result!.IsAdmin == false) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = result.ToDictionary, ErrorMessage = null});

            
        }

        [HttpGet]
        [Route("email/{email}")]
		[Authorize]
		public async Task<IActionResult> GetAdminByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.GetAllAsync(i => i.Email == email && i.IsAdmin == true);

            if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var admin = result.FirstOrDefault();

            return Ok(new DataResponse { Data = admin!.ToDictionary, ErrorMessage = null });
        }

        [HttpGet]
        [Route("exp/{exp}")]
		[Authorize]
		public async Task<IActionResult> GetAdminByExp(string exp)
        {
            if(!Regex.IsMatch(exp, @"^\d+$")) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.GetAllAsync(i => i.Expediente == exp && i.IsAdmin == true);

            if (result is null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var admin = result.FirstOrDefault();
            
            return Ok(new DataResponse { Data = admin!.ToDictionary, ErrorMessage = null });
        }

		[HttpGet]
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetInstructorsByRange(int start, int end)
		{
            if (start < 0 || end < start) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var result = await _adminsService.GetAllAsync(i => i.IsAdmin == true);

			if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var adminsInRange = result.Skip(start).Take(end - start + 1).ToList();

            var data = new List<Dictionary<string, object>>();

			foreach (var item in adminsInRange) data.Add(item.ToDictionary);

			return Ok(new DataResponse { Data = data, ErrorMessage = null });
		}

        [HttpGet]
        [Route("search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetAdminSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var search = searchTerm.ToLower();

            var result = await _adminsService.GetAllAsync(i => i.IsAdmin == true &&
            ((i.Expediente != null && i.Expediente.ToLower().Contains(search)) ||
            (i.Name != null && i.Name.ToLower().Contains(search)) ||
            (i.LastName != null && i.LastName.ToLower().Contains(search)) ||
            (i.Email != null && i.Email.ToLower().Contains(search)) ||
            (i.PhoneNumber != null && i.PhoneNumber.Contains(search))
            ));

            if (result == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            var admins = result.ToList();

            var data = new List<Dictionary<string, object>>();

            foreach (var item in admins) data.Add(item.ToDictionary);

            if (admins.Count > 0) return Ok(new DataResponse { Data = data, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });
        }

        [HttpPost]
        [Route("create")]
        [Authorize]

        public async Task<IActionResult> CreateAdmin([FromBody] AdminSchema admin) {

            //validate register attrributes

            if (admin.Expediente == null) return BadRequest( new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Password == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });


            //check existence 
           var result = await _adminsService.GetAllAsync(i =>
           ((i.Expediente != null && i.Expediente.ToLower() == admin.Expediente.ToLower()) ||
           (i.Email != null && i.Email.ToLower() == admin.Email.ToLower()) ||
           (i.PhoneNumber != null && i.PhoneNumber == admin.PhoneNumber.ToLower())
           ));

            if (result.Any()) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS});

            var newAdmin = new ApplicationUser { 

                Id = Guid.NewGuid().ToString(),
                Name = admin.Name,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Expediente = admin.Expediente,
                IsAdmin = true,
                IsActive = true,
            };

            await _userManager.CreateAsync(newAdmin, admin.Password!);

            var adminAdded = await _adminsService.AddAsync(newAdmin);

            if (adminAdded is not null) return Ok(new DataResponse { Data = adminAdded.ToDictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });

            /*public string? Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Expediente { get; set; }
        public string? Password { get; set; }*/

        }
    }
}

