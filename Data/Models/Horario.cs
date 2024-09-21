using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Horario
    {

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? Day { get; set; }
        [Required]
        public TimeSpan StartHour { get; set; }
        [Required]
        public TimeSpan EndHour { get; set; }
        [Required]
        public string? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }


    }
}
