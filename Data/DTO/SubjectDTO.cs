using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UniSportUAQ_API.Data.Models;

namespace UniSportUAQ_API.Data.DTO
{
    public class SubjectDTO
    {

        public string? Id { get; set; }
        
        public string? Name { get; set; }

        public string? CoursePictureUrl { get; set; }
    }
}
