using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSportUAQ_API.Data
{
    public class Inscription
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime DateInscription { get; set; }


        //liberado

        public bool released {get; set;}


        [Required]
        public bool In_Info {  get; set; }

        
        [Required]
        //foreign key
        public string? Id_Student { get; set; }

        //foreign key
        [Required]
        public int Id_course { get; set; }

        [ForeignKey("Id_Student")]
        public virtual Student Student { get; set; }

        [ForeignKey("Id_course")]
        public virtual Course Course { get; set; }



    }
}
