
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Schemas;
using UniSportUAQ_API.Data.Base;
using System.Diagnostics.CodeAnalysis;



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

        [AllowNull]
        public string? InscriptionId { get; set; }

		[Required]
		public string? VerificationCode { get; set; }

        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        [ForeignKey("InscriptionId")]
        public Inscription? Inscription { get; set; }
	}
}

