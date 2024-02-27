using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
        public string? Expediente { get; set; }
		public string? PictureUrl { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }

		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>
			{
				{ nameof(Id), Id },
				{ nameof(Name), Name is not null ? Name : "" },
				{ nameof(Email), Email is not null ? Email : "" },
				{ nameof(UserName), UserName is not null ? UserName : "" },
				{ nameof(PictureUrl), PictureUrl is not null ? PictureUrl : "" },
			};
		}

	}
}
