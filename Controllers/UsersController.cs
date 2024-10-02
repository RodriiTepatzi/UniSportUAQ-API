
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.DTO;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using System.IO;
using System.Security.Claims;
using static iTextSharp.text.pdf.events.IndexEvents;
using UniSportUAQ_API.Data;


namespace UniSportUAQ_API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICoursesService _coursesService;
        private readonly IInscriptionsService _inscriptionsService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<UsersController> _logger;
        private readonly AppDbContext _context;

        public UsersController(IUsersService usersService, UserManager<ApplicationUser> userManager, ICoursesService coursesService, IInscriptionsService inscriptionsService,
            IWebHostEnvironment hostingEnvironment, ILogger<UsersController> logger, AppDbContext appDbContext)
        {
            _usersService = usersService;
            _userManager = userManager;
            _coursesService = coursesService;
            _inscriptionsService = inscriptionsService;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _context = appDbContext;
        }


        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return NotFound(new BaseResponse<object> { Data = result });

            var data = new List<UserDTO>();

            foreach (var item in result)
            {

                var gnrc = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(gnrc);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }

        [HttpGet]
        [Route("count")]
        [Authorize]
        public async Task<IActionResult> GetUsersCount()
        {

            var usercount = await _usersService.GetAllAsync(i => i.IsActive == true);
            var count = usercount.Count();

            return Ok(new BaseResponse<int> { Data = count });
        }

        [HttpGet]
        [Route("filter")]
        [Authorize]
        public async Task<IActionResult> GetUsersByFilter(
            [FromQuery] string? q,
            [FromQuery] bool? admin,
            [FromQuery] bool? student,
            [FromQuery] bool? instructor)
        {
            var users = new List<UserDTO>();

            var result = await _usersService.GetAllAsync(u =>
                (!admin.HasValue || u.IsAdmin == admin.Value) &&
                (!student.HasValue || u.IsStudent == student.Value) &&
                (!instructor.HasValue || u.IsInstructor == instructor.Value) &&
                (string.IsNullOrEmpty(q) ||
                 u.Name!.Contains(q) ||
                 u.LastName!.Contains(q) ||
                 u.Expediente!.Contains(q) ||
                 u.Email!.Contains(q) ||
                 u.PhoneNumber!.Contains(q))
            );

            foreach (var item in result)
            {
                users.Add(new UserDTO
                {
                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,
                    PhoneNumber = item.PhoneNumber,
                });
            }


            if (result == null) return Ok(new BaseResponse<List<UserDTO>> { Data = new List<UserDTO>() });
            else return Ok(new BaseResponse<List<UserDTO>> { Data = users });
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userManager.FindByIdAsync(id);

            if (result == null) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.DataNotFound });

            var user = new UserDTO
            {
                Id = result.Id,
                Expediente = result.Expediente,
                PictureUrl = result.PictureUrl,
                Name = result.Name,
                LastName = result.LastName,
                IsAdmin = result.IsAdmin,
                IsInstructor = result.IsInstructor,
                IsStudent = result.IsStudent,
                PhoneNumber = result.PhoneNumber,
            };

            return Ok(new BaseResponse<UserDTO> { Data = user });
        }


        [HttpGet]
        [HttpGet]
        [Route("admins/all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAdminsByRange(int start, int end)
        {
            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync(i => i.IsAdmin == true);

            if (result.Count() < 1) return NotFound(new BaseResponse<object> { Data = result });

            var adminsInRange = result.Skip(start).Take(end - start + 1).ToList();

            if (adminsInRange.Count() < 1) return NotFound(new BaseResponse<object> { Data = adminsInRange });

            var data = new List<UserDTO>();

            foreach (var item in adminsInRange)
            {

                var admns = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(admns);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpGet]
        [Route("admins/search/{searchTerm}")]
        [Authorize]
        public async Task<IActionResult> GetAdminSeacrhAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterInvalidSearchTerm });

            var search = searchTerm.ToLower();
            var result = new List<ApplicationUser>();

            try
            {
                var users = await _usersService.GetAllAsync(i => i.IsAdmin == true &&
                ((i.Expediente!.ToLower().Contains(search)) ||
                (i.Name!.ToLower().Contains(search)) ||
                (i.LastName!.ToLower().Contains(search)) ||
                (i.Email!.ToLower().Contains(search)) ||
                (i.PhoneNumber!.Contains(search))
                ));
                result = users.ToList();
            }
            catch
            {
                return StatusCode(500, new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.ServerDataBaseError });
            }

            if (result.Count() < 1) return NotFound(new BaseResponse<object> { Data = result });

            var admins = result.ToList();

            if (admins.Count() < 1) NotFound(new BaseResponse<object> { Data = admins });

            var data = new List<UserDTO>();

            foreach (var item in admins)
            {

                var admns = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(admns);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });

        }



        [HttpPost]
        [Route("admins/create")]
        [Authorize]

        public async Task<IActionResult> CreateAdmin([FromBody] AdminSchema admin)
        {

            //validate register attrributes

            if (admin.Expediente == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (admin.Password == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });


            //check existence 
            var result = await _usersService.GetAllAsync(i =>
            ((i.Expediente != null && i.Expediente.ToLower() == admin.Expediente.ToLower()) ||
            (i.Email != null && i.Email.ToLower() == admin.Email.ToLower()) ||
            (i.PhoneNumber != null && i.PhoneNumber == admin.PhoneNumber.ToLower())
            ));

            if (result.Any()) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.ENTITY_EXISTS });

            var newAdmin = new ApplicationUser
            {

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

            var adminAdded = await _usersService.AddAsync(newAdmin);

            if (adminAdded is not null) return Ok(new DataResponse { Data = adminAdded.ToDictionary, ErrorMessage = null });

            return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.INTERNAL_ERROR });



        }

        //**********************************PROFILE-PICTURE**********************************//
        [HttpPut]
        [Route("update/image")]
        [Authorize]
        public async Task<IActionResult> UpdateImage([FromBody] ImageProfile Data)
        {
            if (Data == null) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrEmpty(Data!.Base64Image)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });
            if (string.IsNullOrEmpty(Data.FileFormat)) return BadRequest(new BaseResponse<bool> { Error = ResponseErrors.AttributeEmptyOrNull });

            //get current user
            var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userEmail == null)
            {
                return Unauthorized(new BaseResponse<ApplicationUser>
                {
                    Error = ResponseErrors.AuthInvalidToken
                });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return BadRequest(new BaseResponse<ApplicationUser>
                {
                    Error = ResponseErrors.AuthUserNotFound
                });
            }
            var currentUserPicture = user.PictureUrl;
            //if current user has a photo delete it


            try
            {
                if (string.IsNullOrWhiteSpace(Data.Base64Image) || string.IsNullOrWhiteSpace(Data.FileFormat))
                {
                    return BadRequest(new BaseResponse<bool> { Data = false });
                }

                byte[] imageBytes;
                try
                {
                    imageBytes = Convert.FromBase64String(Data.Base64Image);
                }
                catch (FormatException)
                {
                    return BadRequest(new BaseResponse<bool> { Data = false });
                }

                string baseDirectory = _hostingEnvironment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");

                string folderPath = Path.Combine(baseDirectory, "users");
                string concretePath = Path.Combine(folderPath, "profile");
                string guidName = Guid.NewGuid().ToString();


                if (!Directory.Exists(concretePath))
                {
                    Directory.CreateDirectory(concretePath);
                }

                string filePath = Path.Combine(concretePath, $"{guidName}.{Data.FileFormat}");

                if (currentUserPicture != null)
                {

                    var deleted = await DeleteFileAsync(currentUserPicture);

                    if (deleted == false) return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.DeleteFileError });

                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                {
                    await fileStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                    await fileStream.FlushAsync();
                }


                var url = $"/users/profile/{guidName}.{Data.FileFormat}";

                await _context.Entry(user).ReloadAsync();

                var entry = _context.Entry(user);

                entry.Entity.PictureUrl = url;

                _context.Entry(user).Property(e => e.PictureUrl).IsModified = true;

                await _context.SaveChangesAsync();


                return Ok(new BaseResponse<bool> { Data = true });


            }
            catch
            {

                return Ok(new BaseResponse<bool> { Data = false, Error = ResponseErrors.ConvertImageError });
            }

        }

        private async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }


        //**********************************GENERIC**********************************//


        [HttpGet]
        [Route("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();

            if (!emailAttribute.IsValid(email)) return BadRequest(new BaseResponse<UserDTO> { Data = null, Error = ResponseErrors.AttributeEmaiInvalidlFormat });

            var result = await _usersService.GetAllAsync(i => i.Email == email);

            if (result.Count() < 1) return NotFound(new BaseResponse<object> { Data = result });

            var user = result.FirstOrDefault();

            if (user == null) return NotFound(new BaseResponse<object> { Data = user });

            var response = new UserDTO
            {

                Id = user.Id,
                Expediente = user.Expediente,
                PictureUrl = user.PictureUrl,
                Name = user.Name,
                LastName = user.LastName,
                IsAdmin = user.IsAdmin,
                IsStudent = user.IsStudent,
                IsInstructor = user.IsInstructor,

            };

            return Ok(new BaseResponse<UserDTO> { Data = response, Error = null });

        }


        [HttpGet]
        [Route("all/range/{start}/{end}")]
        [Authorize]
        public async Task<IActionResult> GetAllUsersInRange(int start, int end)
        {

            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return NotFound(new BaseResponse<object> { Data = result });

            var usersInRange = result.Skip(start).Take(end - start + 1).ToList();

            if (usersInRange.Count() < 1) return NotFound(new BaseResponse<object> { Data = usersInRange });

            var data = new List<UserDTO>();

            foreach (var item in usersInRange)
            {

                var gnrc = new UserDTO
                {

                    Id = item.Id,
                    Expediente = item.Expediente,
                    PictureUrl = item.PictureUrl,
                    Name = item.Name,
                    LastName = item.LastName,
                    IsAdmin = item.IsAdmin,
                    IsInstructor = item.IsInstructor,
                    IsStudent = item.IsStudent,

                };

                data.Add(gnrc);
            }

            return Ok(new BaseResponse<List<UserDTO>> { Data = data });
        }

        [HttpPut]
        [Route("{id}/update")]
        [Authorize]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UserSchema user, string id)
        {

            if (user.Id == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.PictureUrl == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var oldUser = await _usersService.GetByIdAsync(user.Id);

            if (oldUser == null) return Ok(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            oldUser.PhoneNumber = user.PhoneNumber;
            oldUser.Email = user.Email;
            oldUser.IsInFIF = user.IsInFIF;
            oldUser.Semester = user.Semester;
            oldUser.IsActive = user.IsActive;
            oldUser.IsAdmin = user.IsAdmin;
            oldUser.IsStudent = user.IsStudent;
            oldUser.IsInstructor = user.IsInstructor;
            oldUser.PictureUrl = user.PictureUrl;

            var result = await _usersService.UpdateAsync(oldUser);

            if (result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND });

            return Ok(new DataResponse { Data = oldUser.ToDictionary, ErrorMessage = null });
        }



    }
}

