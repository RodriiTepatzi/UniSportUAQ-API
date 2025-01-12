using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.DTO;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using static UniSportUAQ_API.Data.Schemas.Authentication;

namespace UniSportUAQ_API.Controllers
{
    [Route("api/v1/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterModel model)
		{
			if (model == null || !ModelState.IsValid)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthInvalidData });

			if (await _userManager.FindByEmailAsync(model.Email!) != null)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthUserEmailAlreadyExists });

			if (await _userManager.FindByNameAsync(model.Expediente!) != null)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthUserExpedienteAlreadyExists });

			var user = new ApplicationUser { 
				Expediente = model.Expediente,
				Name = model.Name,
				LastName = model.LastName,
				PhoneNumber = model.PhoneNumber,
				UserName = model.Expediente,
				Group = model.Group,
				Semester = model.Semester,
				Email = model.Email,
				IsStudent = true,
			};

			var result = await _userManager.CreateAsync(user, model.Password!);

			if (!result.Succeeded)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthErrorCreatingUser });

			var registeredUser = _userManager.FindByEmailAsync(model.Email!).Result;

			if (registeredUser is not null) await _userManager.AddToRoleAsync(registeredUser, UserRoles.User);

			return Ok();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			if (model == null || !ModelState.IsValid)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidData
				});
			}

			var user = await _userManager.FindByNameAsync(model.Expediente!);

			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password!))
			{
				var accessToken = await GenerateJwtToken(user);
				var refreshToken = GenerateRefreshToken();

				
                if (model.RememberMe == true) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(180);
                if (model.RememberMe == false) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(120);
				

				user.RefreshToken = refreshToken;

				await _userManager.UpdateAsync(user);

                return Ok(new BaseResponse<TokenRefreshModel>
				{
					Data = new TokenRefreshModel
					{
						AccessToken = accessToken,
						RefreshToken = refreshToken,
						ExpiryDate = user.RefreshTokenExpiryTime,
					}
				});
			}

			return Unauthorized(new BaseResponse<TokenRefreshModel>
			{
				Data = null,
				Error = ResponseErrors.AuthInvalidCredentials
			});
		}


		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] TokenRefreshModel model)
		{
			if (model == null || !ModelState.IsValid)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidData
				});
			}

			var principal = GetPrincipalFromExpiredToken(model.AccessToken!);
			var userId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.FindByEmailAsync(userId);
			if (user == null || user.RefreshToken != model.RefreshToken)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidRefreshToken
				});
			}

			if (user.RefreshTokenExpiryTime <= DateTime.Now)
			{
				return Unauthorized(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthRefreshTokenExpired
				});
			}

			var newAccessToken = await GenerateJwtToken(user);

			await _userManager.UpdateAsync(user);

			return Ok(new BaseResponse<TokenRefreshModel>
			{
				Data = new TokenRefreshModel
				{
					AccessToken = newAccessToken,
					RefreshToken = user.RefreshToken
				}
			});
		}


		[HttpPut("update")]
		[Authorize]
		public async Task<IActionResult> UpdateCurrentUser([FromBody] UserSchema model)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userEmail == null)
			{
				return Unauthorized(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.FindByEmailAsync(userEmail);
			if (user == null)
			{
				return NotFound(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			user.Name = model.Name;
			user.LastName = model.LastName;
			user.PhoneNumber = model.PhoneNumber;
			user.PictureUrl = model.PictureUrl;
			user.Semester = model.Semester;
			user.Group = model.Group;
			user.Expediente = model.Expediente;
			user.UserName = model.Expediente;
			user.IsAdmin = model.IsAdmin;
			user.IsStudent = model.IsStudent;
			user.Email = model.Email;

			// Save changes to the database
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthErrorUpdatingUser
				});
			}

			return Ok(new BaseResponse<bool>
			{
				Data = true
			});
		}

		[HttpGet("current")]
		[Authorize]
		public async Task<IActionResult> GetCurrentUser()
		{
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
				return NotFound(new BaseResponse<ApplicationUser>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			return Ok(new BaseResponse<Dictionary<string, object>>
			{
				Data = user.ToDictionary
			});
		}

		[HttpPut("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userEmail == null)
			{
				return Unauthorized(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			var user = await _userManager.FindByEmailAsync(userEmail);
			if (user == null)
			{
				return NotFound(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
			if (!passwordCheck)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidCurrentPassword
				});
			}

			var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword!);
			if (!result.Succeeded)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthErrorChangingPassword
				});
			}

			return Ok(new BaseResponse<string>
			{
				Data = "Password changed successfully"
			});
		}



		private async Task<string> GenerateJwtToken(IdentityUser user)
		{
			var userApplication = await _userManager.FindByIdAsync(user.Id);
			var userRoles = await _userManager.GetRolesAsync(userApplication!);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id)
			};

			claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
				audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(Environment.GetEnvironmentVariable("JWT_EXPIRES"))),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false,
				ClockSkew = TimeSpan.Zero
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}
	}
}
