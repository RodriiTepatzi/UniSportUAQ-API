
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Scripting.Interpreter;
using UniSportUAQ_API.Data.Schemas;



namespace UniSportUAQ_API.Data.Models
{
    public class CartaLiberacion
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }

        [Required]
        public string? StudentId { get; set; }

        [Required]
        public string? CourseId { get; set; }

        [Required]
        public string? Url { get; set; }

        [ForeignKey("StudentId")]
        public ApplicationUser? Student { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public Dictionary<string, object> Dictionary => new Dictionary<string, object> {

            { nameof(Id), Id },
            { nameof(StudentId), StudentId},
            { nameof(CourseId), CourseId},
            { nameof(Url), Url },

        };
    }
}

