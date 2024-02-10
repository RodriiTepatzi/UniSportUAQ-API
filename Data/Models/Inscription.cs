using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data.Models
{
    public class Inscription
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public DateTime DateInscription { get; set; }


        //liberado

        public bool Released { get; set; }


        [Required]
        public bool InInfo { get; set; }


        [Required]
        //foreign key
        public string? StudentId { get; set; }

        //foreign key
        [Required]
        public string? CourseId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }



    }
}
