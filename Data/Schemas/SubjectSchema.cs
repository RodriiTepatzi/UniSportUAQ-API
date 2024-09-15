using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.Schemas
{
	public class SubjectSchema
	{
		public string? Id { get; set; }

		public string? Name { get; set; }

        
        public string? CoursePictureUrl { get; set; }

    }
}
