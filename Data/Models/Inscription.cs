using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{
    public class Inscription : IEntityBase
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public DateTime DateInscription { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [Required]
        public string? CourseId { get; set; }

        public int Grade { get; set; }

        public bool Accredit { get; set; }

        public bool UnEnrolled { get; set; }

        [AllowNull]
        public string? CartaId { get; set; }

        [Required]
		[DefaultValue(false)]
		public bool IsFinished { get; set; }

        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [ForeignKey("CartaId")]
        public CartaLiberacion? CartaLiberacion { get; set; }

        public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>
		{
			{ nameof(Id), Id is not null ? Id:"" },
			{ nameof(DateInscription), DateInscription },
			{ nameof(Student), Student is not null ? Student.ToDictionary : "" },
			{ nameof(Course), Course is not null ? Course.Dictionary : ""  },
			{ nameof(Grade), Grade},
			{ nameof(Accredit), Accredit},
            { nameof(IsFinished), IsFinished}
		};

		public Dictionary<string, object> ToDictionaryWithNoStudent() => new Dictionary<string, object>
		{
			{"Id", Id ?? string.Empty },
			{"DateInscription", DateInscription },
			{"Accredit", Accredit },
			{ "Course", Course?.Dictionary! }
		};
	}
}
