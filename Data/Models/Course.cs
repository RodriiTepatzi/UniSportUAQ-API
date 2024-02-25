using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Models
{
    public class Course
    {
        [Key]
        [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Id { get; set; }


        [Required]
        [StringLength(50)]
        public string? CourseName { get; set; }


        [Required]
        public string? InstructorId { get; set; }

        [Required]
        [StringLength(20)]
        public string? Day {  get; set; }
        [Required]
        [StringLength(20)]
        public string? Hour { get; set; }

        [Required]
        public int MaxUsers { get; set; }
        public int CurrentUsers { get; set; }
        public int PendingUsers { get; set; }

        [NotMapped]
        public List<Student> CurrentUsersList { get; set; }
        
        [NotMapped]
        public List<Student> PendingUsersList { get; set; }


        [ForeignKey("InstructorId")]
        public virtual Instructor? Instructor { get; set; }


        /****************************************************************/

        public Dictionary<string, object> ToDictionaryForIdRequest() {

            return new Dictionary<string, object>
            {
                { nameof(Id), Id},
                { nameof(CourseName), CourseName is not null? CourseName: ""},
                { nameof(InstructorId), InstructorId is not null? InstructorId: ""}
            };

        }

    }
}

