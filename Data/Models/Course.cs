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
        public string? CourseName { get; set; }


        [Required]
        public string? InstructorId { get; set; }

        public virtual Instructor? Instructor { get; set; }
    }
}
