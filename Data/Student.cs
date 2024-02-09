using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data
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
	}
}
