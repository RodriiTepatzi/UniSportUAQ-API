using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Instructor : ApplicationUser
    {
		[NotMapped]
        public List<Course>? Courses { get; set; }

		public Dictionary<string, object> Dictionary => new Dictionary<string, object>
		{
			{ nameof(Id), Id },
			{ nameof(Name), Name is not null ? Name : "" },
			{ nameof(Email), Email is not null ? Email : "" },
			{ nameof(UserName), UserName is not null ? UserName : "" },
			{ nameof(Courses), Courses is not null ? Courses : new List<Course>() },
			{ nameof(PictureUrl), PictureUrl is not null ? PictureUrl : "" },
		};
	}
}
