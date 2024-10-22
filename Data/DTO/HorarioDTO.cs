using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.DTO
{
    public class HorarioDTO
    {
        public string? Id { get; set; }
        public string? Day { get; set; }
        
        public TimeSpan StartHour { get; set; }
        
        public TimeSpan EndHour { get; set; }
        
        public string? CourseId { get; set; }
    }
}
