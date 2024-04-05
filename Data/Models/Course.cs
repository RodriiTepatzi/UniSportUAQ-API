﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

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
        public string? Description { get; set; }

        [Required]
		public string? InstructorId { get; set; }

		[Required]
		[StringLength(20)]
		public bool VirtualOrHybrid { get; set; }

		[StringLength(50)]
		[Required]
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
        [DefaultValue(0)]
        public int PendingUsers { get; set; }

		[ForeignKey("InstructorId")]
		public ApplicationUser? Instructor { get; set; }

		public bool IsActive { get; set; }


		public Dictionary<string, object> Dictionary => new Dictionary<string, object> {
			{ nameof(Id), Id },
			{ nameof(CourseName), CourseName is not null ? CourseName : "" },
			{ nameof(Description), Description is not null ? Description : ""},
			{ nameof(Instructor), Instructor is not null ? Instructor.ToDictionary : "" },
			{ nameof(VirtualOrHybrid), VirtualOrHybrid},
			{ nameof(Location), Location},
			{ nameof(Link), Link},
			{ nameof(Day), Day },
			{ nameof(StartHour), StartHour },
			{ nameof(EndHour), EndHour },
			{ nameof(MaxUsers), MaxUsers },
			{ nameof(CurrentUsers), CurrentUsers },
			{ nameof(PendingUsers), PendingUsers },
		};
	}
}

