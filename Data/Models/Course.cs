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

		[DefaultValue(0)]
		public int CurrentUsers { get; set; }

		[DefaultValue(true)]
		public bool IsActive { get; set; }

        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }

        [ForeignKey("SubjectId")]
        public Subject? Subject { get; set; }

        [NotMapped]
		public string? CoursePictureUrl { get; set; }



        public Dictionary<string, object> Dictionary => new Dictionary<string, object> {
		{ nameof(Id), Id ?? "n/a" },
		{ nameof(CourseName), CourseName ?? "n/a" },
		{ nameof(Description), Description ?? "n/a" },
		{ nameof(InstructorId), InstructorId != null ? InstructorId : "n/a" },
		{ nameof(SubjectId), SubjectId != null ? SubjectId: "n/a" },
		{ nameof(VirtualOrHybrid), VirtualOrHybrid },
		{ nameof(Location), Location ?? string.Empty },
		{ nameof(Link), Link ?? string.Empty },
		{ nameof(Day), Day ?? string.Empty },
		{ nameof(StartHour), StartHour ?? string.Empty },
		{ nameof(EndHour), EndHour ?? string.Empty },
		{ nameof(MaxUsers), MaxUsers },
		{ nameof(CurrentUsers), CurrentUsers },
		{ nameof(IsActive), IsActive },
};
    }
}

