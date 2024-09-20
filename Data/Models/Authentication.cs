using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
	public class Authentication
	{
		public class RegisterModel
		{
			[Required]
			public string? Email { get; set; }

			[Required]
			public string? Password { get; set; }

			[Required]
			public string? Name { get; set; }

			[Required]
			public string? LastName { get; set; }

			[Required]
			public string? Expediente { get; set; }

			[Required]
			public string? PhoneNumber { get; set; }

			[Required]
			public int Semester { get; set; }

			[Required]
			public int Group { get; set; }

		}

		public class LoginModel
		{
			[Required]
			public string? Expediente { get; set; }

			[Required]
			public string? Password { get; set; }

			[Required]
			public bool RememberMe{ get; set; }
		}

		public class TokenRefreshModel
		{
			[Required]
			public string? AccessToken { get; set; }

			[Required]
			public string? RefreshToken { get; set; }
		}
	}
}
