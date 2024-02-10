using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Instructor : ApplicationUser
    {
		[NotMapped]
        public List<Course>? Courses { get; set; }

    }
}
