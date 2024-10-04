using System.ComponentModel.DataAnnotations;

namespace UniSportUAQ_API.Data.DTO
{
    public class TimePeriodDTO
    {
        public string? Id { get; set; }
        public string? Period { get; set; }
        public string? Type { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
