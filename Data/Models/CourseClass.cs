using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
    public class CourseClass
    {
        [Key]
        [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }

        [Required]
        [StringLength(10)]
        public string? Day { get; set; }

        [Required]
        public string? Hour { get; set; }

        [Required]
        public int Quota { get; set; }

        [Required]
        public string? CourseId;

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }



    }
}
