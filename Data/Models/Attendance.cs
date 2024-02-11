using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniSportUAQ_API.Data.Models
{
    public class Attendance
    {
        [Key]
        public string? Id { get; set; }

        [Required]
        public string? IdStudent { get; set; }

        [Required]
        public string? IdCourse { get; set; }

        
        public bool AttendanceClass { get; set; }

        //foreign keys

        [ForeignKey("Id_Student")]
        public virtual Student? Student { get; set; }

        [ForeignKey("Id_Course")]
        public virtual Course? Course { get; set; }
    }
}
