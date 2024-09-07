using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Base;
using System.Diagnostics;

namespace UniSportUAQ_API.Data.Models
{
    public class Subject : IEntityBase
    {
        [Key]
        [Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? InstructorId { get; set; }
        
        [Required]
        public string? CoursePictureUrl { get; set; }

        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }


        public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>
        {
            { nameof(Id), Id is not null ? Id:"" },
            { nameof(Name), Name is not null ? Name:""},
            { nameof(InstructorId), InstructorId is not null ?  Instructor!.ToDictionary : ""},
            { nameof(CoursePictureUrl), CoursePictureUrl is not null ? CoursePictureUrl : ""  },
        };




    }
}
