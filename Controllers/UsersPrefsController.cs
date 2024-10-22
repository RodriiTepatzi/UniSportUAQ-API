using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.DTO;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;

namespace UniSportUAQ_API.Controllers
{
    [ApiController]
    [Route("users/prefs")]
    public class UsersPrefsController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UsersPrefsService _usersPrefsService;
        public UsersPrefsController(UserManager<ApplicationUser> userManager, UsersPrefsService usersPrefsService)
        {
            _userManager = userManager;
            _usersPrefsService = usersPrefsService;
        }



        [HttpGet]
        [Authorize]
        [Route("prefcurrent")]
        public async Task<IActionResult> GetUserPrefCurrentUser()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new BaseResponse<UserPrefDTO>
                {
                    Error = ResponseErrors.AuthInvalidToken
                });
            }
            // Find the user in the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Ok(new BaseResponse<UserPrefDTO>
                {
                    Error = ResponseErrors.AuthUserNotFound
                });
            }

            string id = user.Id;

            var UsersPrefs = await _usersPrefsService.GetAllAsync(i => i.UserId == id);

            if (UsersPrefs.Count() < 1) return Ok(new BaseResponse<UserPrefDTO>
            {
                Error = ResponseErrors.AuthUserNotFound
            });

            var UserPref = UsersPrefs.FirstOrDefault();

            var response = new UserPrefDTO
            {
                Id = UserPref!.Id,
                UserId = UserPref.UserId,
                Language = UserPref.Language,
            };

            return Ok(new BaseResponse<UserPrefDTO> { Data = response });
        }

        [HttpPost]
        [Authorize]
        [Route("current/create")]
        public async Task<IActionResult> CreateCurrentUserPrefs([FromBody] UserPrefsSchema userPrefsSchema)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new BaseResponse<bool>
                {
                    Error = ResponseErrors.AuthInvalidToken
                });
            }
            // Find the user in the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Ok(new BaseResponse<bool>
                {
                    Error = ResponseErrors.AuthUserNotFound
                });
            }

            string id = user.Id;

            var UsersPrefs = await _usersPrefsService.GetAllAsync(i => i.UserId == id);

            if (UsersPrefs.Any()) return Ok(new BaseResponse<bool>
            {
                Error = ResponseErrors.UserPrefAlreadyExist
            });

            var NewUserPref = new UserPreferences
            {

                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Language = userPrefsSchema.Language,

            };

            var create = await _usersPrefsService.AddAsync(NewUserPref);

            if (create == null) Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

            return Ok(new BaseResponse<bool> { Data = true });

        }


        [HttpPost]
        [Authorize]
        [Route("current/update")]
        public async Task<IActionResult> UpdateCurrentUserPrefs([FromBody] UserPrefsSchema userPrefsSchema)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new BaseResponse<bool>
                {
                    Error = ResponseErrors.AuthInvalidToken
                });
            }
            // Find the user in the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Ok(new BaseResponse<bool>
                {
                    Error = ResponseErrors.AuthUserNotFound
                });
            }

            string id = user.Id;

            var UsersPrefs = await _usersPrefsService.GetAllAsync(i => i.UserId == id);

            //if does not exist create one

            if (!UsersPrefs.Any())
            {

                var NewUserPref = new UserPreferences
                {

                    Id = Guid.NewGuid().ToString(),
                    UserId = user.Id,
                    Language = userPrefsSchema.Language,

                };

                var create = await _usersPrefsService.AddAsync(NewUserPref);

                if (create == null) Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

                return Ok(new BaseResponse<bool> { Data = true });

            }

            //if exist just update it

            var UpdUserPref = new UserPreferences
            {

                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                Language = userPrefsSchema.Language,

            };

            var update = await _usersPrefsService.UpdateAsync(UpdUserPref);

            if (update == null) Ok(new BaseResponse<bool> { Error = ResponseErrors.ServerDataBaseError });

            return Ok(new BaseResponse<bool> { Data = true });

        }


    }
}
