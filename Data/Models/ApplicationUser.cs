using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
	public class ApplicationUser : IdentityUser
	{
        public string? Expediente { get; set; }
		public string? PictureUrl { get; set; }
		public string? Name { get; set; }
		public string? LastName { get; set; }
		public int Group { get; set; }
		public int Semester { get; set; }

		public bool IsStudent { get; set; }
		public bool IsInstructor { get; set; }
		public bool IsAdmin { get; set; }


		[DefaultValue("False")]
		public bool IsInFIF { get; set; }
		public bool IsActive { get; set; }

		[StringLength(10)]
		public string? StudyPlan { get; set; }
		public DateTime RegisteredDateTime { get; set; }

		public Dictionary<string, object> ToDictionary => new Dictionary<string, object>
		{
			{ nameof(Id), Id },
			{ nameof(Name), Name is not null ? Name : "" },
			{ nameof(LastName), LastName is not null ? LastName : "" },
			{ nameof(PhoneNumber), PhoneNumber is not null ? PhoneNumber : "" },
			{ nameof(Email), Email is not null ? Email : "" },
			{ nameof(UserName), UserName is not null ? UserName : "" },
			{ nameof(Expediente), Expediente is not null ? Expediente : "" },
			{ nameof(IsInFIF), IsInFIF },
			{ nameof(Semester), Semester },
			{ nameof(IsActive), IsActive },
			{ nameof(IsAdmin), IsAdmin },
			{ nameof(IsStudent), IsStudent },
			{ nameof(IsInstructor), IsInstructor },
			{ nameof(RegisteredDateTime), RegisteredDateTime },
			{ nameof(PictureUrl), PictureUrl is not null ? PictureUrl : "" },
		};
	}
}
