using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniSportUAQ_API.Data.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? IdStudent { get; set; }

        [Required]
        public int IdCourse { get; set; }

        [DefaultValue("True")]
        public bool AttendanceClass { get; set; }

        [ForeignKey("Id_Student")]
        public virtual Student? Student { get; set; }

        [ForeignKey("Id_Course")]
        public virtual Course? Course { get; set; }
    }
}
