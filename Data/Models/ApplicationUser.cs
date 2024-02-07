using Microsoft.AspNetCore.Identity;

namespace UniSportUAQ_API.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string? Expediente { get; set; }
	}
}
