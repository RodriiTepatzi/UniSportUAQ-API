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
		[StringLength(20)]
		public string? Hour { get; set; }

		[Required]
		public int MaxUsers { get; set; }
		public int CurrentUsers { get; set; }
		public int PendingUsers { get; set; }

		[NotMapped]
		public List<Student>? CurrentUsersList { get; set; }

		[NotMapped]
		public List<Student>? PendingUsersList { get; set; }

		[ForeignKey("InstructorId")]
		public Instructor? Instructor { get; set; }

		public bool IsActive { get; set; }


		public Dictionary<string, object> Dictionary => new Dictionary<string, object> {
			{ nameof(Id), Id },
			{ nameof(CourseName), CourseName is not null ? CourseName : "" },
			{ nameof(Instructor), Instructor is not null ? Instructor.ToDictionary() : null },
			{ nameof(Day), Day },
			{ nameof(Hour), Hour },
			{ nameof(MaxUsers), MaxUsers },
			{ nameof(CurrentUsers), CurrentUsers },
			{ nameof(PendingUsers), PendingUsers },
			{ nameof(CurrentUsersList), CurrentUsersList is not null ? CurrentUsersList : new List<Student>() },
			{ nameof(PendingUsersList), PendingUsersList is not null ? PendingUsersList : new List<Student>() },
		};
	}
}

