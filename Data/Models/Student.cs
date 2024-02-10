using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Student : ApplicationUser
    {
        //pendiente key int

        [Required]
        public int Group { get; set; }

        [Required]
        public int Semester { get; set; }

        public bool IsInOfficialGroup { get; set; }
        public bool IsSuscribed { get; set; }

        public string? StudyPlan { get; set; }

        [NotMapped]
        public List<int>? FinishedCourses { get; set; }

        public string? FinishedCoursesJson { get; set; }
        public DateTime SuscribedDateTime { get; set; }
        public int CurrentCourse { get; set; }

		public Dictionary<string, object> ToDictionaryForEmailRequest()
		{
			return new Dictionary<string, object>
			{
				{ nameof(Id), Id },
				{ nameof(Name), Name is not null ? Name : "" },
				{ nameof(Email), Email is not null ? Email : "" },
				{ nameof(UserName), UserName is not null ? UserName : "" },
				{ nameof(Semester), Semester }
			};
		}
    }
}
