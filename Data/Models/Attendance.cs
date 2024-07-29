using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using UniSportUAQ_API.Data.Base;

namespace UniSportUAQ_API.Data.Models
{
    public class Attendance : IEntityBase
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
        public virtual ApplicationUser? Student { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        public Dictionary<string, object> Dictionary => new Dictionary<string, object>{


                    { nameof(Id), Id ?? string.Empty },
                    { nameof(Student), Student is not null ? Student!.ToDictionary : "" },
                    { nameof(Course), Course is not null ? Course!.Dictionary : "" },
                    { nameof(Date),Date },
                    { nameof(AttendanceClass), AttendanceClass }

        };
    }

     
}
