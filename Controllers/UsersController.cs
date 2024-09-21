using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.DTO;
using UniSportUAQ_API.Data.Interfaces;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
		public UsersController(IUsersService usersService, UserManager<ApplicationUser> userManager, ICoursesService coursesService, IInscriptionsService inscriptionsService)
		{
			_usersService = usersService;
			_userManager = userManager;
			_coursesService = coursesService;
			_inscriptionsService = inscriptionsService;
		}


		[HttpGet]
		[Route("all")]
		[Authorize]
		public async Task<IActionResult> GetAllUsers()
		{
			var result = await _usersService.GetAllAsync();

			if (result.Count() < 1) return NotFound(new BaseResponse<UserDTO> { Data = null });

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
		[Route("all/range/{start}/{end}")]
		[Authorize]
		public async Task<IActionResult> GetAllUsersInRange(int start, int end)
		{

            if (start < 0 || end < start) return BadRequest(new BaseResponse<List<UserDTO>> { Data = null, Error = ResponseErrors.FilterStartEndContradiction });

            var result = await _usersService.GetAllAsync();

            if (result.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

            var usersInRange = result.Skip(start).Take(end - start + 1).ToList();

            if (usersInRange.Count() < 1) return NotFound(new BaseResponse<List<UserDTO>> { Data = null });

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
		public async Task<IActionResult> UpdateUserAsync([FromBody]UserSchema user, string id) {

			if (user.Id == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.Name == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.LastName == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
			if (user.PhoneNumber == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.Email == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });
            if (user.PictureUrl == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.BAD_REQUEST });

            var oldUser = await _usersService.GetByIdAsync(user.Id);

            if (oldUser == null) return Ok(new DataResponse { Data = null , ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

			var userUpdt = new ApplicationUser { 

				Id = oldUser.Id,
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

			var result = await _usersService.UpdateAsync(userUpdt);

			if(result == null) return BadRequest(new DataResponse { Data = null, ErrorMessage = ResponseMessages.OBJECT_NOT_FOUND});

            return Ok(new DataResponse { Data = userUpdt.ToDictionary, ErrorMessage = null});
		}

    }
}

