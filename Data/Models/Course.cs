using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
    public class Course
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }


        [Required]
        [StringLength(50)]
        public string? CourseName { get; set; }


        [Required]
        public string? InstructorId { get; set; }


        [ForeignKey("InstructorId")]
        public virtual Instructor? Instructor { get; set; }
    }
}
