using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Instructor : ApplicationUser
    {
		[NotMapped]
        public List<Course>? Courses { get; set; }

		public Dictionary<string, object> ToDictionaryForIdRetrieve()
		{
			return new Dictionary<string, object>
			{
				{ nameof(Id), Id },
				{ nameof(Name), Name is not null ? Name : "" },
				{ nameof(Email), Email is not null ? Email : "" },
				{ nameof(UserName), UserName is not null ? UserName : "" },
				// TODO :: To keep working on here.
			};
		}
	}
}
