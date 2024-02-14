using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Student : ApplicationUser
    {
        

        [Required]
        public int Group { get; set; }

        [Required]
        public int Semester { get; set; }

        [DefaultValue("False")]
        public bool IsInOfficialGroup { get; set; }
        public bool IsSuscribed { get; set; }

        [StringLength(10)]
        public string? StudyPlan { get; set; }

        [NotMapped]
        public List<int>? FinishedCourses { get; set; }

        public string? FinishedCoursesJson { get; set; }
        public DateTime SuscribedDateTime { get; set; }


        //foreign key
        public string? CurrentCourse { get; set; }

        [ForeignKey("CurrentCourse")]
        public virtual CourseClass CourseClass { get; set; }

		[NotMapped]
		public List<Attendance>? Attendances { get; set; }
		[NotMapped]
		public List<Inscription>? Inscriptions { get; set; }

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

        public Dictionary<string, object> ToDictionaryForIdRequest()
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

        public Dictionary<string, object> ToDictionaryExpRequest()
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
