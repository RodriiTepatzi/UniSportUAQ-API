using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
	public class Course
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }

		[Required]
		[StringLength(50)]
		public string? CourseName { get; set; }

		[Required]
		public string? InstructorId { get; set; }

		[Required]
		[StringLength(20)]
		public string? Day { get; set; }

		[Required]
		[StringLength(6)]
		public string? StartHour { get; set; }

        [Required]
        [StringLength(6)]
        public string? EndHour { get; set; }

        [Required]
		public int MaxUsers { get; set; }
		public int CurrentUsers { get; set; }
		public int PendingUsers { get; set; }

		[ForeignKey("InstructorId")]
		public ApplicationUser? Instructor { get; set; }

		public bool IsActive { get; set; }


		public Dictionary<string, object> Dictionary => new Dictionary<string, object> {
			{ nameof(Id), Id },
			{ nameof(CourseName), CourseName is not null ? CourseName : "" },
			{ nameof(Instructor), Instructor is not null ? Instructor.InstructorToDictionary() : null },
			{ nameof(Day), Day },
			{ nameof(StartHour), StartHour },
			{ nameof(EndHour), EndHour },
			{ nameof(MaxUsers), MaxUsers },
			{ nameof(CurrentUsers), CurrentUsers },
			{ nameof(PendingUsers), PendingUsers },
		};
	}
}

