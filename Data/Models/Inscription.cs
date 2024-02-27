using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Inscription
    {
        [Required]
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }

        [Required]
        public DateTime DateInscription { get; set; }

        public bool Accredit { get; set; }


        [Required]
        public bool InInfo { get; set; }


        [Required]
        public string? StudentId { get; set; }

        [Required]
        public string? CourseId { get; set; }


        [ForeignKey("StudentId")]
        public Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

		public Dictionary<string, object> Dictionary => new Dictionary<string, object>
		{
			{"Id", Id },
			{"DateInscription", DateInscription },
			{"Accredit", Accredit },
			{"InInfo", InInfo },
			{"Student", Student is not null ? Student.ToDictionary() : null },
			{"Course", Course is not null ? Course.Dictionary : null  }
		};

	}
}
