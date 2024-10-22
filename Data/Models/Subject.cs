using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Base;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
        public bool IsActive { get; set; }

        [AllowNull]
        public string? CoursePictureUrl { get; set; }



		// Relationships
		public IEnumerable<Course>? Courses { get; set; }


        public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>
        {
            { nameof(Id), Id is not null ? Id:"" },
            { nameof(Name), Name is not null ? Name:""},
            { nameof(CoursePictureUrl), CoursePictureUrl is not null ? CoursePictureUrl : ""  },
        };




    }
}
