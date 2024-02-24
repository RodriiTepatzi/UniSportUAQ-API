using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Student : ApplicationUser
    {
        public int Group { get; set; }
        public int Semester { get; set; }


        [DefaultValue("False")]
        public bool IsInFIF { get; set; }
        public bool IsActive { get; set; }

        [StringLength(10)]
        public string? StudyPlan { get; set; }

        [NotMapped]
        public List<Course>? FinishedCourses { get; set; }
        public DateTime RegisteredDateTime { get; set; }


        public string? CurrentCourseId { get; set; }

        [ForeignKey("CurrentCourseId")]
        [NotMapped]
        public Course? CurrentCourse { get; set; }

		[NotMapped]
		public List<Attendance>? Attendances { get; set; }
		[NotMapped]
		public List<Inscription>? Inscriptions { get; set; }

		public Dictionary<string, object> ToDictionary()
		{
			return new Dictionary<string, object>
			{
				{ nameof(Id), Id },
				{ nameof(Name), Name is not null ? Name : "" },
				{ nameof(Email), Email is not null ? Email : "" },
				{ nameof(UserName), UserName is not null ? UserName : "" },
                { nameof(Expediente), Expediente is not null ? Expediente : "" },
                { nameof(IsInFIF), IsInFIF },
                { nameof(Semester), Semester },
                { nameof(IsActive), IsActive },
                { nameof(RegisteredDateTime), RegisteredDateTime },
				{ nameof(PictureUrl), PictureUrl is not null ? PictureUrl : "" },
				{ nameof(CurrentCourse), CurrentCourse },
				{ nameof(FinishedCourses), FinishedCourses is not null ? FinishedCourses : new List<Course>() }
			};
		}
    }
}
