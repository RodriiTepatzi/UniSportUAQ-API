using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniSportUAQ_API.Data
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Id_Student { get; set; }

        [Required]
        public int Id_Course { get; set;}

        [DefaultValue("True")]
        public bool Attendance_Class { get; set; }

        [ForeignKey("Id_Student")]
        public virtual Student Student { get; set; }

        [ForeignKey("Id_Course")]
        public virtual Course Course { get; set; }



    }
}
