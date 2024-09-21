using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.Schemas
{
    public class HorarioSchema
    {
        public string? Id { get; set; }
        
        public string? Day { get; set; }
        
        public TimeSpan StartHour { get; set; }
        
        public TimeSpan EndHour { get; set; }
        
        public string? CourseId { get; set; }
    }
}
