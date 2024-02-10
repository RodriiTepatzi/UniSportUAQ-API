using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
        public string? Expediente { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
	}
}
