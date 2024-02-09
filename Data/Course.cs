using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string? CourseName { get; set; }




        [Required]
        public string? IdInstructor { get; set; }


        //foreignkey
        [ForeignKey("IdInstructor")]
        public virtual Instructor Instructor { get; set; }
    }
}
