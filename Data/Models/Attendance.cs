using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniSportUAQ_API.Data.Models
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [Required]
        public string? CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        [DefaultValue("false")]
        public bool AttendanceClass { get; set; }

        //foreign keys

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
                {
                    { nameof(Id), Id is not null },
                    { nameof(StudentId), StudentId is not null ? StudentId : "" },
                    { nameof(CourseId), CourseId is not null ? CourseId : "" },
                    { nameof(Date),DateTime.Now.Date },
                    { nameof(AttendanceClass), AttendanceClass }
                };
        }
    }

     
}
