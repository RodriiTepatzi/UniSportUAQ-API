using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{
	public class Course : IEntityBase
	{
		[Key]
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }

		[Required]
		[StringLength(50)]
		public string? CourseName { get; set; }


		[Required]
		public string? Description { get; set; }

		[Required]
		public string? InstructorId { get; set; }

		[Required]
		public string? SubjectId { get; set; }

		[Required]
		[DefaultValue(false)]
		public bool VirtualOrHybrid { get; set; }


		[Required]
		[StringLength(50)]
        public string? Location { get; set; }

        [AllowNull]
        public string? Link { get; set; }


		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }
        [Required]
		public int MaxUsers { get; set; }

		[DefaultValue(0)]
		public int CurrentUsers { get; set; }

		[Required]
		public int MinAttendances { get; set; }

		[DefaultValue(true)]
		public bool IsActive { get; set; }

        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }

        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }
		public string? CoursePictureUrl { get; set; }

		
		
		// Relationships
		public IEnumerable<Attendance>? Attendances { get; set; }
		public IEnumerable<CartaLiberacion>? CartaLiberacions { get; set; }
		public IEnumerable<Inscription>? Inscriptions { get; set; }
		public IEnumerable<Horario>? Horarios { get; set; }



		public Dictionary<string, object> Dictionary => new Dictionary<string, object> {
		{ nameof(Id), Id ?? "n/a" },
		{ nameof(CourseName), CourseName ?? "n/a" },
		{ nameof(Description), Description ?? "n/a" },
		{ nameof(InstructorId), InstructorId != null ? InstructorId : "n/a" },
		{ nameof(SubjectId), SubjectId != null ? SubjectId: "n/a" },
		{ nameof(VirtualOrHybrid), VirtualOrHybrid },
		{ nameof(Location), Location ?? string.Empty },
		{ nameof(Link), Link ?? string.Empty },
		{ nameof(MaxUsers), MaxUsers },
		{ nameof(CurrentUsers), CurrentUsers },
		{ nameof(IsActive), IsActive },
};
    }
}

