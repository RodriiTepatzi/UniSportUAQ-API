using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UniSportUAQ_API.Data.Base;
using UniSportUAQ_API.Data.Consts;
using UniSportUAQ_API.Data.Models;
using UniSportUAQ_API.Data.Schemas;
using static UniSportUAQ_API.Data.Models.Authentication;

namespace UniSportUAQ_API.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;

		public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
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
				Email = model.Email 
			};
			var result = await _userManager.CreateAsync(user, model.Password!);

			if (!result.Succeeded)
				return BadRequest(new BaseResponse<RegisterModel> { Data = null, Error = ResponseErrors.AuthErrorCreatingUser });

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
				var accessToken = GenerateJwtToken(user);
				var refreshToken = GenerateRefreshToken();

				if (model.RememberMe) user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(180);
				else user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(15);
				

				user.RefreshToken = refreshToken;

				await _userManager.UpdateAsync(user);

				return Ok(new BaseResponse<TokenRefreshModel>
				{
					Data = new TokenRefreshModel
					{
						AccessToken = accessToken,
						RefreshToken = refreshToken
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
			if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
			{
				return BadRequest(new BaseResponse<TokenRefreshModel>
				{
					Error = ResponseErrors.AuthInvalidRefreshToken
				});
			}

			var newAccessToken = GenerateJwtToken(user);
			var newRefreshToken = GenerateRefreshToken();

			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
			await _userManager.UpdateAsync(user);

			return Ok(new BaseResponse<TokenRefreshModel>
			{
				Data = new TokenRefreshModel
				{
					AccessToken = newAccessToken,
					RefreshToken = newRefreshToken
				}
			});
		}

		[HttpPut("update")]
		[Authorize] // Ensures the user is authenticated
		public async Task<IActionResult> UpdateCurrentUser([FromBody] UserSchema model)
		{
			// Retrieve the user ID from the JWT token claims
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			// Find the user in the database
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			// Update user fields based on the received model
			user.Name = model.Name;
			user.LastName = model.LastName;
			user.PhoneNumber = model.PhoneNumber;
			user.PictureUrl = model.PictureUrl;
			user.Semester = model.Semester;
			user.Group = model.Group;
			user.Expediente = model.Expediente;
			user.UserName = model.Expediente; // Make sure UserName is unique
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

			return Ok(new BaseResponse<string>
			{
				Data = "User information updated successfully"
			});
		}

		// Endpoint for getting the current user info
		[HttpGet("current")]
		[Authorize] // Ensures the user is authenticated
		public async Task<IActionResult> GetCurrentUser()
		{
			// Retrieve the user ID from the JWT token claims
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new BaseResponse<ApplicationUser>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			// Find the user in the database
			var user = await _userManager.FindByEmailAsync(userId);
			if (user == null)
			{
				return NotFound(new BaseResponse<ApplicationUser>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			// Return user details as a dictionary
			return Ok(new BaseResponse<Dictionary<string, object>>
			{
				Data = user.ToDictionary
			});
		}

		// Endpoint for changing the password
		[HttpPut("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
		{
			// Retrieve the user ID from the JWT token claims
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidToken
				});
			}

			// Find the user in the database
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthUserNotFound
				});
			}

			// Verify the current password
			var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
			if (!passwordCheck)
			{
				return BadRequest(new BaseResponse<string>
				{
					Error = ResponseErrors.AuthInvalidCurrentPassword
				});
			}

			// Change the password
			var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
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



		private string GenerateJwtToken(IdentityUser user)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id)
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
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
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
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
