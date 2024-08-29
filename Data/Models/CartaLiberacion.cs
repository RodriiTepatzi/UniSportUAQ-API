
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Base;



namespace UniSportUAQ_API.Data.Models
{
    public class CartaLiberacion : IEntityBase
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [Required]
        public string? CourseId { get; set; }

        [Required]
        public string? Url { get; set; }

        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

		public Dictionary<string, object> Dictionary => new Dictionary<string, object>
		{
			{ nameof(Id), Id ?? string.Empty },
			{ nameof(StudentId), StudentId ?? string.Empty },
			{ nameof(CourseId), CourseId ?? string.Empty },
			{ nameof(Url), Url ?? string.Empty }
		};

	}
}

